using System;
using System.Collections.Generic;
using System.Linq;

namespace fantasy_hoops.Repositories
{
    interface ILeaderboardRepository
    {

        IEnumerable<Object> GetPlayerLeaderboard(int from, int limit, string type);
        IEnumerable<Object> GetUserLeaderboard(int from, int limit, string type, string date);
        IEnumerable<Object> GetFriendsLeaderboard(string id, int from, int limit, string type, string date);
        IQueryable<Object> GetSeasonLineups();
        IQueryable<Object> GetSeasonPlayers();
    }
}
