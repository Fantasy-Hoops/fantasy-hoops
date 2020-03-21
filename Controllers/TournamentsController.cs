using System;
using System.Collections.Generic;
using Castle.Core;
using fantasy_hoops.Models.Tournaments;
using fantasy_hoops.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace fantasy_hoops.Controllers
{
    [Route("api/[controller]")]
    public class TournamentsController : Controller
    {
        private readonly ITournamentsRepository _tournamentsRepository;

        public TournamentsController(ITournamentsRepository tournamentsRepository)
        {
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
    }
}