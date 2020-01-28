﻿using fantasy_hoops.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;

namespace fantasy_hoops.Repositories
{
    public interface ILeaderboardRepository
    {

        List<PlayerLeaderboardRecordDto> GetPlayerLeaderboard(int from, int limit, string type);
        List<UserLeaderboardRecordDto> GetUserLeaderboard(int from, int limit, string type, string date, int weekNumber);
        List<UserLeaderboardRecordDto> GetFriendsLeaderboard(string id, int from, int limit, string type, string date, int weekNumber);
        List<UserLeaderboardRecordDto> GetSeasonLineups(int year);
        List<PlayerLeaderboardRecordDto> GetSeasonPlayers(int year);
        List<PlayerLeaderboardRecordDto> GetMostSelectedPlayers(int from, int count);
    }
}
