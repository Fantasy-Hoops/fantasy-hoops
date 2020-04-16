using System;
using System.Linq;
using fantasy_hoops.Models.ViewModels;

namespace fantasy_hoops.Repositories.Interfaces
{
    public interface IBlogRepository
    {
        IQueryable<Object> GetPosts();
        bool PostExists(int id);
        void AddPost(SubmitPostViewModel model);
        void DeletePost(int id);
    }
}
