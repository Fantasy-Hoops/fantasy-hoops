using fantasy_hoops.Models.ViewModels;
using fantasy_hoops.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using fantasy_hoops.Dtos;
using fantasy_hoops.Helpers;
using fantasy_hoops.Repositories.Interfaces;
using fantasy_hoops.Services.Interfaces;
using Microsoft.AspNetCore.Http;

namespace fantasy_hoops.Controllers
{
    [Route("api/[controller]")]
    public class LineupController : Controller
    {
        private readonly ILineupService _lineupService;
        private readonly ILineupRepository _lineupRepository;

        public LineupController(ILineupService lineupService, ILineupRepository lineupRepository)
        {
            _lineupService = lineupService;
            _lineupRepository = lineupRepository;
        }

        [Authorize]
        [HttpGet("{id}")]
        public IActionResult GetUserLineups(String id)
        {
            String userId = CommonFunctions.Instance.GetUserIdFromClaims(User);
            if (!userId.Equals(id))
            {
                return StatusCode(StatusCodes.Status403Forbidden, "Unauthorized access to the resource.");
            }
            
            return Ok(_lineupRepository.GetLineup(id));
        }

        [Authorize]
        [HttpPost("submit")]
        public IActionResult SubmitLineup([FromBody]SubmitLineupViewModel model)
        {
            string userId = User.Claims.ToList()[0].Value;
            model.UserID = userId;
            if (_lineupRepository.GetLineupPrice(model) > LineupService.MAX_PRICE)
                return StatusCode(StatusCodes.Status422UnprocessableEntity, "Lineup price exceeds the budget! Lineup was not submitted.");
            if (!_lineupRepository.ArePricesCorrect(model))
                return StatusCode(StatusCodes.Status422UnprocessableEntity, "Wrong player prices! Lineup was not submitted.");
            if (!RuntimeUtils.PLAYER_POOL_DATE.Equals(RuntimeUtils.NEXT_GAME))
                return StatusCode(StatusCodes.Status422UnprocessableEntity, "Player pool not updated! Try again in a moment.");
            if (_lineupRepository.AreNotPlayingPlayers(model))
                return StatusCode(StatusCodes.Status422UnprocessableEntity, "Player pool is outdated! Refresh the page.");

            _lineupService.SubmitLineup(model);

            return Ok("Lineup was updated successfully");
        }

        [HttpGet("nextGame")]
        public IActionResult NextGame()
        {
            return Ok(new
            {
                nextGame = RuntimeUtils.NEXT_GAME_CLIENT,
                serverTime = DateTime.Now,
                playerPoolDate = RuntimeUtils.PLAYER_POOL_DATE
            });
        }

        [Authorize]
        [HttpGet("current")]
        public UserLeaderboardRecordDto GetUserCurrentLineup()
        {
            string userId = User.Claims.ToList()[0].Value;
            return _lineupRepository.GetUserCurrentLineup(userId);
        }

        [Authorize]
        [HttpGet("recent/{userId}")]
        public ActionResult<List<UserLeaderboardRecordDto>> GetRecentLineups([FromRoute] string userId, [FromQuery] int start = 0, [FromQuery] int count = 5)
        {
            return _lineupRepository.GetRecentLineups(userId, start, count);
        }
        
        [HttpGet("bestLineups")]
        public List<BestLineupDto> GetBestLineups(string date, int from = 0, int limit = 10)
        {
            return _lineupRepository.GetBestLineups(date, from, limit);
        }
    }
}
