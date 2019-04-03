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
        void AddPost(SubmitPostViewModel model);
    }
}
