using fantasy_hoops.Models.ViewModels;

namespace fantasy_hoops.Services.Interfaces
{
    public interface ILineupService
    {

        void SubmitLineup(SubmitLineupViewModel model);

    }
}
