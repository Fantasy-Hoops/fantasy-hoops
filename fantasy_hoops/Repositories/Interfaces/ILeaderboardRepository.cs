using System.Collections.Generic;
using fantasy_hoops.Dtos;
using fantasy_hoops.Enums;

namespace fantasy_hoops.Repositories.Interfaces
{
    public interface ILeaderboardRepository
    {
        List<PlayerLeaderboardRecordDto> GetPlayerLeaderboard(int from, int limit, LeaderboardType type);

        List<UserLeaderboardRecordDto>
            GetUserLeaderboard(int from, int limit, LeaderboardType type, string date, int weekNumber, int year);

        List<UserLeaderboardRecordDto> GetFriendsLeaderboard(string id, int from, int limit, LeaderboardType type, string date,
            int weekNumber, int year);

        List<UserLeaderboardRecordDto> GetSeasonLineups(int from, int limit, int year);
        List<PlayerLeaderboardRecordDto> GetSeasonPlayers(int from, int limit, int year);
        List<PlayerLeaderboardRecordDto> GetMostSelectedPlayers(int from, int limit);
    }
}