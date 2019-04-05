using fantasy_hoops.Database;
using fantasy_hoops.Helpers;
using fantasy_hoops.Models;
using fantasy_hoops.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fantasy_hoops.Repositories
{
    public class BlogRepository : IBlogRepository
    {
        private readonly GameContext _context;

        public BlogRepository(GameContext context)
        {
            _context = context;
        }

        public IQueryable<Object> GetPosts()
        {
            return _context.Posts
                .Select(post => new
                {
                    id = post.PostID,
                    post.Title,
                    post.Body,
                    Author = new
                    {
                        post.Author.UserName,
                        post.Author.Id
                    },
                    post.CreatedAt,
                    post.ModifiedAt
                })
                .OrderByDescending(post => post.CreatedAt);
        }

        public void AddPost(SubmitPostViewModel model)
        {
            _context.Posts.Add(
                    new Post
                    {
                        Title = model.Title,
                        Body = model.Body,
                        AuthorID = model.AuthorID,
                        CreatedAt = CommonFunctions.UTCToEastern(DateTime.UtcNow),
                        ModifiedAt = CommonFunctions.UTCToEastern(DateTime.UtcNow)
                    });
        }

        public bool PostExists(int id)
        {
            return _context.Posts.Any(post => post.PostID == id);
        }

        public void DeletePost(int id)
        {
            Post postToDelete = _context.Posts
                .Where(post => post.PostID == id)
                .FirstOrDefault();
            _context.Posts.Remove(postToDelete);
        }
    }
}
