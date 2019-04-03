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
        private readonly GameContext _context;
        private readonly BlogRepository _repository;

        public BlogService(GameContext context)
        {
            _context = context;
            _repository = new BlogRepository(_context);
        }
        public async void SubmitPost(SubmitPostViewModel model)
        {
            _repository.AddPost(model);

            await _context.SaveChangesAsync();
        }
    }
}
