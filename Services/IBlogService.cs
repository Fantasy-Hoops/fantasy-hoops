using fantasy_hoops.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fantasy_hoops.Services
{
    public interface IBlogService
    {
        void SubmitPost(SubmitPostViewModel model);
        void DeletePost(int id);
    }
}
