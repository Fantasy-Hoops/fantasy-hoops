using System;
using System.Collections.Generic;
using Castle.Core;
using fantasy_hoops.Dtos;
using fantasy_hoops.Helpers;
using fantasy_hoops.Models.Tournaments;
using fantasy_hoops.Models.ViewModels;
using fantasy_hoops.Repositories.Interfaces;
using fantasy_hoops.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace fantasy_hoops.Controllers
{
    [Route("api/[controller]")]
    public class TournamentsController : Controller
    {
        private readonly ITournamentsService _tournamentsService;
        private readonly ITournamentsRepository _tournamentsRepository;

        public TournamentsController(ITournamentsService tournamentsService, ITournamentsRepository tournamentsRepository)
        {
            _tournamentsService = tournamentsService;
            _tournamentsRepository = tournamentsRepository;
        }
        
        [HttpGet("types")]
        public List<TournamentTypeDto> GetTypes()
        {
            return _tournamentsRepository.GetTournamentTypes();
        }
        
        [HttpGet("start-dates")]
        public List<DateTime> GetStartDates()
        {
            return _tournamentsRepository.GetUpcomingStartDates();
        }

        [HttpGet("user/{userId}")]
        public Dictionary<string, List<TournamentDto>> GetUserTournaments([FromRoute] string userId)
        {
            return _tournamentsRepository.GetUserTournaments(userId);
        }

        [Authorize]
        [HttpGet("{tournamentId}")]
        public ActionResult<Tournament> GetTournamentById([FromRoute] string tournamentId)
        {
            if (!_tournamentsRepository.TournamentExists(tournamentId))
            {
                return NotFound($"Tournament with id '{tournamentId}' does not exist.");
            }
            
            string userId = CommonFunctions.GetUserIdFromClaims(User);
            if (!_tournamentsRepository.IsUserInTournament(userId, tournamentId))
            {
                return Unauthorized("Unauthorized attempt to reach the tournament.");
            }

            return _tournamentsRepository.GetTournamentById(tournamentId);
        }

        [Authorize]
        [HttpGet("{tournamentId}/details")]
        public ActionResult<TournamentDetailsDto> GetTournamentDetails([FromRoute] string tournamentId)
        {
            if (!_tournamentsRepository.TournamentExists(tournamentId))
            {
                return NotFound($"Tournament with id '{tournamentId}' does not exist.");
            }
            
            string userId = CommonFunctions.GetUserIdFromClaims(User);
            if (!_tournamentsRepository.IsUserInTournament(userId, tournamentId))
            {
                return Unauthorized("Unauthorized attempt to reach the tournament.");
            }

            return _tournamentsRepository.GetTournamentDetails(userId, tournamentId);
        }

        [Authorize]
        [HttpPost]
        public IActionResult CreateTournament([FromBody] CreateTournamentViewModel model)
        {
            if (_tournamentsRepository.TournamentNameExists(model.TournamentTitle))
            {
                return UnprocessableEntity("Tournament title already exists.");
            }
            if (model.StartDate < CommonFunctions.UTCToEastern(DateTime.UtcNow))
            {
                return UnprocessableEntity("Illegal Date. Date must be in the future.");
            }

            if (string.IsNullOrEmpty(model.TournamentIcon))
            {
                return UnprocessableEntity("No tournament icon selected.");
            }
            
            Pair<bool, string> result = _tournamentsService.CreateTournament(model);
            if (!result.First)
            {
                return StatusCode(StatusCodes.Status406NotAcceptable, "Unable to handle request.");
            }
            
            return Ok(new
            {
                message =  "Tournament created",
                inviteUrl = result.Second
            });
        }

        [Authorize]
        [HttpDelete("{tournamentId}")]
        public IActionResult DeleteTournament([FromRoute] string tournamentId)
        {
            if (!_tournamentsRepository.TournamentExists(tournamentId))
            {
                return UnprocessableEntity("Tournament doesn't exist.");
            }

            bool succeeded = _tournamentsRepository.DeleteTournament(tournamentId);
            if (!succeeded)
            {
                StatusCode(StatusCodes.Status406NotAcceptable, "Unable to delete tournament.");
            }

            return Ok("Tournament deleted successfully");
        }
    }
}