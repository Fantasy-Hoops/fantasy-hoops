using System;
using System.Collections.Generic;
using Castle.Core;
using fantasy_hoops.Helpers;
using fantasy_hoops.Models.Tournaments;
using fantasy_hoops.Models.ViewModels;
using fantasy_hoops.Repositories.Interfaces;
using fantasy_hoops.Services.Interfaces;
using Google.Apis.Requests;
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
        public List<Tournament.TournamentType> GetTypes()
        {
            return _tournamentsRepository.GetTournamentTypes();
        }
        
        [HttpGet("start-dates")]
        public List<DateTime> GetStartDates()
        {
            return _tournamentsRepository.GetUpcomingStartDates();
        }

        [HttpGet("user/{userId}")]
        public Dictionary<string, List<Tournament>> GetUserTournaments([FromRoute] string userId)
        {
            return _tournamentsRepository.GetUserTournaments(userId);
        }

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
                return StatusCode(StatusCodes.Status406NotAcceptable, "Unable to handle request. Server error.");
            }
            
            return Ok(new
            {
                message =  "Tournament created",
                inviteUrl = result.Second
            });
        }
    }
}