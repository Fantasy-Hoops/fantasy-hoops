using System;
using System.Collections.Generic;
using fantasy_hoops.Dtos;
using fantasy_hoops.Models;
using fantasy_hoops.Models.ViewModels;

namespace fantasy_hoops.Repositories.Interfaces
{
    public interface ILineupRepository
    {

        UserLineup GetLineup(String id);
        void AddLineup(SubmitLineupViewModel model);
        void UpdateLineup(SubmitLineupViewModel mode);
        int GetLineupPrice(SubmitLineupViewModel model);
        bool ArePricesCorrect(SubmitLineupViewModel model);
        bool IsUpdating(String userID);
        bool AreNotPlayingPlayers(SubmitLineupViewModel model);
        List<string> GetUserSelectedIds();
        List<User> UsersNotSelected();
        UserLeaderboardRecordDto GetUserCurrentLineup(string userId);
        List<UserLeaderboardRecordDto> GetRecentLineups(string userId, int start, int count);
    }
}
