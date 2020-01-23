using fantasy_hoops.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;

namespace fantasy_hoops.Repositories
{
    public interface ILeaderboardRepository
    {

        List<PlayerDto> GetPlayerLeaderboard(int from, int limit, string type);
        List<UserLeaderboardRecordDto> GetUserLeaderboard(int from, int limit, string type, string date, int weekNumber);
        List<UserLeaderboardRecordDto> GetFriendsLeaderboard(string id, int from, int limit, string type, string date, int weekNumber);
        List<UserLeaderboardRecordDto> GetSeasonLineups(int year);
        IQueryable<Object> GetSeasonPlayers(int year);
        IQueryable<Object> GetMostSelectedPlayers(int from, int count);
    }
}
