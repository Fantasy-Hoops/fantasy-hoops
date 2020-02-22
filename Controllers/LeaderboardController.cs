using fantasy_hoops.Database;
using fantasy_hoops.Helpers;
using fantasy_hoops.Repositories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using fantasy_hoops.Repositories.Interfaces;

namespace fantasy_hoops.Controllers
{
    [Route("api/[controller]")]
    public class LeaderboardController : Controller
    {
        private readonly ILeaderboardRepository _repository;

        public LeaderboardController(ILeaderboardRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("player")]
        public IEnumerable<Object> GetPlayerLeaderboard(int from = 0, int limit = 30, string type = "weekly")
        {
            return _repository.GetPlayerLeaderboard(from, limit, type).ToList();
        }


        [HttpGet("user")]
        public IEnumerable<Object> GetUserLeaderboard(int from = 0, int limit = 10, string type = "weekly", string date = "", int weekNumber = -1, int year = -1)
        {
            return _repository.GetUserLeaderboard(from, limit, type, date, weekNumber, year).ToList();
        }

        [HttpGet("user/{id}")]
        public IEnumerable<Object> GetFriendsLeaderboard(string id, int from = 0, int limit = 10, string type = "weekly", string date = "", int weekNumber = -1, int year = -1)
        {
            return _repository.GetFriendsLeaderboard(id, from, limit, type, date, weekNumber, year).ToList();
        }

        [HttpGet("season/lineups")]
        public Object GetSeasonLineups(int year = -1)
        {
            return _repository.GetSeasonLineups(year);
        }

        [HttpGet("season/players")]
        public Object GetSeasonPlayers(int year = -1)
        {
            return _repository.GetSeasonPlayers(year);
        }

        [HttpGet("selected/players")]
        public Object GetMostSelectedPlayers(int from = 0, int count = 10)
        {
            return _repository.GetMostSelectedPlayers(from, count);
        }
    }
}
