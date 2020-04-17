using fantasy_hoops.Helpers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using fantasy_hoops.Enums;
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
            LeaderboardType leaderboardType = CommonFunctions.ParseLeaderboardType(type);
            return _repository.GetPlayerLeaderboard(from, limit, leaderboardType).ToList();
        }


        [HttpGet("user")]
        public IEnumerable<Object> GetUserLeaderboard(int from = 0, int limit = 10, string type = "weekly", string date = "", int weekNumber = -1, int year = -1)
        {
            LeaderboardType leaderboardType = CommonFunctions.ParseLeaderboardType(type);
            return _repository.GetUserLeaderboard(from, limit, leaderboardType, date, weekNumber, year).ToList();
        }

        [HttpGet("user/{id}")]
        public IEnumerable<Object> GetFriendsLeaderboard(string id, int from = 0, int limit = 10, string type = "weekly", string date = "", int weekNumber = -1, int year = -1)
        {
            LeaderboardType leaderboardType = CommonFunctions.ParseLeaderboardType(type);
            return _repository.GetFriendsLeaderboard(id, from, limit, leaderboardType, date, weekNumber, year).ToList();
        }

        [HttpGet("season/lineups")]
        public Object GetSeasonLineups(int from = 0, int limit = 10, int year = -1)
        {
            return _repository.GetSeasonLineups(from, limit, year);
        }

        [HttpGet("season/players")]
        public Object GetSeasonPlayers(int from = 0, int limit = 10, int year = -1)
        {
            return _repository.GetSeasonPlayers(from, limit, year);
        }

        [HttpGet("selected/players")]
        public Object GetMostSelectedPlayers(int from = 0, int limit = 10)
        {
            return _repository.GetMostSelectedPlayers(from, limit);
        }
    }
}
