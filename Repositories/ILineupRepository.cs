using fantasy_hoops.Models;
using fantasy_hoops.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace fantasy_hoops.Repositories
{
    public interface ILineupRepository
    {

        object GetLineup(String id);
        void AddLineup(SubmitLineupViewModel model);
        void UpdateLineup(SubmitLineupViewModel mode);
        int GetLineupPrice(SubmitLineupViewModel model);
        bool ArePricesCorrect(SubmitLineupViewModel model);
        bool IsUpdating(String userID);
        bool AreNotPlayingPlayers(SubmitLineupViewModel model);
        List<string> GetUserSelectedIds();
        List<User> UsersNotSelected();
    }
}
