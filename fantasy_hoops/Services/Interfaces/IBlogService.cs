using fantasy_hoops.Models.ViewModels;

namespace fantasy_hoops.Services.Interfaces
{
    public interface IBlogService
    {
        void SubmitPost(SubmitPostViewModel model);
        void DeletePost(int id);
    }
}
