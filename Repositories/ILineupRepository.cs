using fantasy_hoops.Models.ViewModels;
using System;
using System.Linq;

namespace fantasy_hoops.Repositories
{
    interface ILineupRepository
    {

        object GetLineup(String id);
        void AddLineup(SubmitLineupViewModel model);
        void UpdateLineup(SubmitLineupViewModel mode);
        int GetLineupPrice(SubmitLineupViewModel model);
        bool ArePricesCorrect(SubmitLineupViewModel model);
        bool IsUpdating(String userID);


    }
}
