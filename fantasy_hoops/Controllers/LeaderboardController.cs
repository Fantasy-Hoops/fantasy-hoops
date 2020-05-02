using fantasy_hoops.Helpers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using fantasy_hoops.Models.Enums;
using fantasy_hoops.Repositories.Interfaces;

namespace fantasy_hoops.Controllers
{
    [Route("api/[controller]")]
    public class LeaderboardController : Controller
    {
        private readonly ILeaderboardRepository _leaderboardRepository;

        public LeaderboardController(ILeaderboardRepository leaderboardRepository)
        {
            _leaderboardRepository = leaderboardRepository;
        }

        [HttpGet("player")]
        public IEnumerable<Object> GetPlayerLeaderboard(int from = 0, int limit = 30, string type = "weekly")
        {
            LeaderboardType leaderboardType = CommonFunctions.Instance.ParseLeaderboardType(type);
            return _leaderboardRepository.GetPlayerLeaderboard(from, limit, leaderboardType).ToList();
        }


        [HttpGet("user")]
        public IEnumerable<Object> GetUserLeaderboard(int from = 0, int limit = 10, string type = "weekly", string date = "", int weekNumber = -1, int year = -1)
        {
            LeaderboardType leaderboardType = CommonFunctions.Instance.ParseLeaderboardType(type);
            return _leaderboardRepository.GetUserLeaderboard(from, limit, leaderboardType, date, weekNumber, year).ToList();
        }

        [HttpGet("user/{id}")]
        public IEnumerable<Object> GetFriendsLeaderboard(string id, int from = 0, int limit = 10, string type = "weekly", string date = "", int weekNumber = -1, int year = -1)
        {
            LeaderboardType leaderboardType = CommonFunctions.Instance.ParseLeaderboardType(type);
            return _leaderboardRepository.GetFriendsLeaderboard(id, from, limit, leaderboardType, date, weekNumber, year).ToList();
        }

        [HttpGet("season/lineups")]
        public Object GetSeasonLineups(int from = 0, int limit = 10, int year = -1)
        {
            return _leaderboardRepository.GetSeasonLineups(from, limit, year);
        }

        [HttpGet("season/players")]
        public Object GetSeasonPlayers(int from = 0, int limit = 10, int year = -1)
        {
            return _leaderboardRepository.GetSeasonPlayers(from, limit, year);
        }

        [HttpGet("selected/players")]
        public Object GetMostSelectedPlayers(int from = 0, int limit = 10)
        {
            return _leaderboardRepository.GetMostSelectedPlayers(from, limit);
        }
    }
}
