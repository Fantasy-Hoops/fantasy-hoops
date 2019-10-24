using fantasy_hoops.Database;
using fantasy_hoops.Models.ViewModels;
using fantasy_hoops.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fantasy_hoops.Services
{
    public class BlogService : IBlogService
    {
        private readonly IBlogRepository _blogRepository;

        public BlogService(IBlogRepository blogRepository)
        {
            _blogRepository = blogRepository;
        }
        public void SubmitPost(SubmitPostViewModel model)
        {
            _blogRepository.AddPost(model);
        }

        public void DeletePost(int id)
        {
            _blogRepository.DeletePost(id);
        }
    }
}
