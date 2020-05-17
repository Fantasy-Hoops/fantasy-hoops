using System;
using System.Collections.Generic;
using Castle.Core;
using fantasy_hoops.Dtos;
using fantasy_hoops.Helpers;
using fantasy_hoops.Jobs;
using fantasy_hoops.Models.Tournaments;
using fantasy_hoops.Models.ViewModels;
using fantasy_hoops.Repositories;
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
        private readonly IFriendRepository _friendRepository = new FriendRepository();

        public TournamentsController(ITournamentsService tournamentsService,
            ITournamentsRepository tournamentsRepository)
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

        [Authorize]
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

            string userId = CommonFunctions.Instance.GetUserIdFromClaims(User);
            if (!_tournamentsRepository.IsUserInTournament(userId, tournamentId)
                && !_tournamentsRepository.IsUserInvited(userId, tournamentId))
            {
                return Unauthorized("Unauthorized attempt to reach the tournament.");
            }

            return _tournamentsRepository.GetTournamentById(tournamentId);
        }

        [Authorize]
        [HttpGet("{tournamentId}/details")]
        public ActionResult<TournamentDetailsDto> GetTournamentDetails([FromRoute] string tournamentId,
            bool checkForFriends)
        {
            if (!_tournamentsRepository.TournamentExists(tournamentId))
            {
                return NotFound($"Tournament with id '{tournamentId}' does not exist.");
            }

            string userId = CommonFunctions.Instance.GetUserIdFromClaims(User);
            bool isUserInTournament = _tournamentsRepository.IsUserInTournament(userId, tournamentId);
            if (checkForFriends && isUserInTournament)
            {
                return StatusCode(StatusCodes.Status302Found, "You already joined this tournament.");
            }

            TournamentDetailsDto tournamentDetails = _tournamentsRepository.GetTournamentDetails(userId, tournamentId, checkForFriends);
            if (tournamentDetails == null)
            {
                return StatusCode(StatusCodes.Status302Found, "You declined to join this tournament.");
            }
            
            if (checkForFriends)
            {
                bool friendCheckStatus = userId.Equals(tournamentDetails.Creator.UserId) ||
                                         _friendRepository.AreUsersFriends(userId, tournamentDetails.Creator.UserId);

                return friendCheckStatus
                    ? Ok(tournamentDetails)
                    : StatusCode(StatusCodes.Status403Forbidden, "You are not friends with the tournament creator.");
            }

            if (!isUserInTournament && !_tournamentsRepository.IsUserInvited(userId, tournamentId))
            {
                return Unauthorized("Unauthorized attempt to reach the tournament.");
            }

            return Ok(tournamentDetails);
        }

        [Authorize]
        [HttpPost]
        public IActionResult CreateTournament([FromBody] CreateTournamentViewModel model)
        {
            if (_tournamentsRepository.TournamentNameExists(model.TournamentTitle))
            {
                return UnprocessableEntity("Tournament title already exists.");
            }

            if (model.StartDate < CommonFunctions.Instance.UTCToEastern(DateTime.UtcNow))
            {
                return UnprocessableEntity("Illegal Date. Date must be in the future.");
            }

            if (model.Entrants <= model.DroppedContests)
            {
                return UnprocessableEntity("Number of dropped contests must be less than number of entrants.");
            }

            if (string.IsNullOrEmpty(model.TournamentIcon))
            {
                return UnprocessableEntity("No tournament icon selected.");
            }

            if ((Tournament.TournamentType) model.TournamentType == Tournament.TournamentType.MATCHUPS
                && model.Entrants % 2 != 0)
            {
                return UnprocessableEntity("For Matchups tournament number of entrants must be even.");
            }

            Pair<bool, string> result = _tournamentsService.CreateTournament(model);
            if (!result.First)
            {
                return StatusCode(StatusCodes.Status406NotAcceptable, "Unable to handle request.");
            }

            return Ok(new
            {
                message = "Tournament created",
                inviteUrl = result.Second
            });
        }

        [Authorize]
        [HttpPut("{tournamentId}")]
        public IActionResult UpdateTournament([FromBody] CreateTournamentViewModel model,
            [FromRoute] string tournamentId)
        {
            Tournament tournament = _tournamentsRepository.GetTournamentById(tournamentId);
            if (tournament == null)
            {
                return NotFound("Tournament doesn't exist.");
            }

            string userIdFromClaims = CommonFunctions.Instance.GetUserIdFromClaims(User);
            if (!tournament.CreatorID.Equals(userIdFromClaims))
            {
                return StatusCode(StatusCodes.Status403Forbidden, "Access forbidden.");
            }

            if (!_tournamentsRepository.UpdateTournament(tournament, model))
            {
                return StatusCode(StatusCodes.Status422UnprocessableEntity, "Failed updating tournament.");
            }

            return Ok("Tournament updated successfully.");
        }

        [Authorize]
        [HttpDelete("{tournamentId}")]
        public IActionResult DeleteTournament([FromRoute] string tournamentId)
        {
            Tournament tournament = _tournamentsRepository.GetTournamentById(tournamentId);
            if (tournament == null)
            {
                return UnprocessableEntity("Tournament doesn't exist.");
            }

            string userIdFromClaims = CommonFunctions.Instance.GetUserIdFromClaims(User);
            if (!tournament.CreatorID.Equals(userIdFromClaims))
            {
                return StatusCode(StatusCodes.Status403Forbidden, "Access forbidden.");
            }

            bool succeeded = _tournamentsRepository.DeleteTournament(tournamentId);
            if (!succeeded)
            {
                StatusCode(StatusCodes.Status406NotAcceptable, "Unable to delete tournament.");
            }

            return Ok("Tournament deleted successfully");
        }


        [Authorize]
        [HttpGet("invitations")]
        public List<TournamentDto> GetTournamentInvitations()
        {
            string userId = CommonFunctions.Instance.GetUserIdFromClaims(User);
            return _tournamentsRepository.GetTournamentInvitations(userId);
        }

        [Authorize]
        [HttpPost("invitations/accept")]
        public IActionResult AcceptInvitation([FromBody] TournamentIdViewModel model)
        {
            string userId = CommonFunctions.Instance.GetUserIdFromClaims(User);
            if (!_tournamentsService.AcceptInvitation(model.TournamentId, userId))
            {
                return StatusCode(StatusCodes.Status422UnprocessableEntity, "Tournament is full");
            }

            return Ok("Successfully joined the tournament.");
        }

        [Authorize]
        [HttpPost("invitations/decline")]
        public IActionResult DeclineInvitation([FromBody] TournamentIdViewModel model)
        {
            string userId = CommonFunctions.Instance.GetUserIdFromClaims(User);
            if (!_tournamentsService.DeclineInvitation(model.TournamentId, userId))
            {
                return StatusCode(StatusCodes.Status422UnprocessableEntity, "Unable to decline invitation");
            }

            return Ok("Invitation declined.");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("start-tournament/{tournamentId}")]
        public IActionResult StartTournament([FromRoute] string tournamentId)
        {
            Tournament tournament = _tournamentsRepository.GetTournamentById(tournamentId);
            if (tournament == null)
            {
                return NotFound("Tournament not found.");
            }

            TournamentsJob tournamentsJob = new TournamentsJob();
            if ((Tournament.TournamentType) tournament.Type == Tournament.TournamentType.ONE_FOR_ALL)
            {
                tournamentsJob.StartOneForAllTournament(tournament);
            }
            else
            {
                tournamentsJob.StartMatchupsTournament(tournament);
            }

            return Ok("Tournament started.");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("simulate-tournament/{tournamentId}")]
        public IActionResult SimulateTournament([FromRoute] string tournamentId)
        {
            Tournament tournament = _tournamentsRepository.GetTournamentById(tournamentId);
            if (tournament == null)
            {
                return NotFound("Tournament not found.");
            }

            TournamentsJob tournamentsJob = new TournamentsJob();
            if ((Tournament.TournamentType) tournament.Type == Tournament.TournamentType.ONE_FOR_ALL)
            {
                tournamentsJob.SimulateOneForAllTournament(tournament);
            }
            else
            {
                tournamentsJob.SimulateMatchupsTournament(tournament);
            }

            return Ok("Tournament simulated.");
        }
    }
}