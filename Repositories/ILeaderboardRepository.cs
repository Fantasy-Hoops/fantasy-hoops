using System;
using System.Collections.Generic;
using System.Linq;

namespace fantasy_hoops.Repositories
{
    interface ILeaderboardRepository
    {

        IEnumerable<Object> GetPlayerLeaderboard(int from, int limit, string type);
        IEnumerable<Object> GetUserLeaderboard(int from, int limit, string type);
        IEnumerable<Object> GetFriendsLeaderboard(string id, int from, int limit, string type);
        IQueryable<Object> GetSeasonLineups();
        IQueryable<Object> GetSeasonPlayers();
    }
}
