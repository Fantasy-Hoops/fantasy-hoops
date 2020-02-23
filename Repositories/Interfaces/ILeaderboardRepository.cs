using System.Collections.Generic;
using fantasy_hoops.Dtos;

namespace fantasy_hoops.Repositories.Interfaces
{
    public interface ILeaderboardRepository
    {
        List<PlayerLeaderboardRecordDto> GetPlayerLeaderboard(int from, int limit, string type);

        List<UserLeaderboardRecordDto>
            GetUserLeaderboard(int from, int limit, string type, string date, int weekNumber, int year);

        List<UserLeaderboardRecordDto> GetFriendsLeaderboard(string id, int from, int limit, string type, string date,
            int weekNumber, int year);

        List<UserLeaderboardRecordDto> GetSeasonLineups(int from, int limit, int year);
        List<PlayerLeaderboardRecordDto> GetSeasonPlayers(int from, int limit, int year);
        List<PlayerLeaderboardRecordDto> GetMostSelectedPlayers(int from, int limit);
    }
}