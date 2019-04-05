using fantasy_hoops.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fantasy_hoops.Repositories
{
    interface IBlogRepository
    {
        IQueryable<Object> GetPosts();
        bool PostExists(int id);
        void AddPost(SubmitPostViewModel model);
        void DeletePost(int id);
    }
}
