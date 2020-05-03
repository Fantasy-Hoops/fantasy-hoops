using System.Collections.Generic;
using System.Linq;
using fantasy_hoops.Models.ViewModels;
using fantasy_hoops.Repositories;
using fantasy_hoops.Repositories.Interfaces;
using fantasy_hoops.Tests.Mocks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using NUnit.Framework;

namespace fantasy_hoops.Tests.Repositories
{
    public class BlogRepositoryTests
    {
        
        private readonly ContextMock.Builder _contextBuilder = new ContextMock.Builder();

        [SetUp]
        public void SetUp()
        {
            var config = new MemoryConfigurationProvider(new MemoryConfigurationSource());
            config.Add("TimeZone", "EST");
            Startup.Configuration = new ConfigurationRoot(new List<IConfigurationProvider>
            {
                config
            });
        }
        
        [Test]
        public void TestGetApprovedPosts()
        {
            var context = _contextBuilder
                .SetBlogPosts()
                .Build();
            
            IBlogRepository blogRepository = new BlogRepository(context);
            var approvedPosts = blogRepository.GetApprovedPosts();
            
            Assert.NotNull(approvedPosts);
            Assert.AreEqual(1, approvedPosts.Count);
        }
        [Test]
        public void TestGetUnapprovedPosts()
        {
            var context = _contextBuilder
                .SetBlogPosts()
                .Build();
            
            IBlogRepository blogRepository = new BlogRepository(context);
            var unapprovedPosts = blogRepository.GetUnapprovedPosts();
            
            Assert.NotNull(unapprovedPosts);
            Assert.AreEqual(1, unapprovedPosts.Count);
        }

        [Test]
        public void TestGetPostById()
        {
            var context = _contextBuilder
                .SetBlogPosts()
                .Build();
            
            IBlogRepository blogRepository = new BlogRepository(context);
            var post200 = blogRepository.GetPostById(200);
            
            Assert.NotNull(post200);
            Assert.AreEqual("Title 200", post200.Title);
            Assert.AreEqual("Body 200", post200.Body);
        }

        [Test]
        public void TestPostExistsSuccess()
        {
            var context = _contextBuilder
                .SetBlogPosts()
                .Build();
            
            IBlogRepository blogRepository = new BlogRepository(context);
            var is200Exist = blogRepository.PostExists(200);
            
            Assert.AreEqual(true, is200Exist);
        }

        [Test]
        public void TestPostExistsFail()
        {
            var context = _contextBuilder
                .SetBlogPosts()
                .Build();
            
            IBlogRepository blogRepository = new BlogRepository(context);
            bool is50Exist = blogRepository.PostExists(50);
            
            Assert.AreEqual(false, is50Exist);
        }

        [Test]
        public void TestAddPostSuccess()
        {
            var context = _contextBuilder
                .SetBlogPosts()
                .Build();
            
            IBlogRepository blogRepository = new BlogRepository(context);
            bool result = blogRepository.AddPost(new SubmitPostViewModel
            {
                Title = "Random title",
                Body = "Random body",
                AuthorID = "xxx"
            });
            
            Assert.AreEqual(true, result);
            Assert.AreEqual(3, context.Posts.ToList().Count);
        }

        [Test]
        public void TestUpdatePostSuccess()
        {
            var context = _contextBuilder
                .SetBlogPosts()
                .Build();
            
            IBlogRepository blogRepository = new BlogRepository(context);
            bool result = blogRepository.UpdatePost(new SubmitPostViewModel
            {
                Id = 200,
                Title = "Updated title"
            });
            var post = blogRepository.GetPostById(200);
            
            Assert.AreEqual(true, result);
            Assert.AreEqual("Updated title", post.Title);
            Assert.AreEqual("Body 200", post.Body);
            Assert.AreEqual(2, context.Posts.ToList().Count);
        }

        [Test]
        public void TestUpdatePostFail()
        {
            var context = _contextBuilder
                .SetBlogPosts()
                .Build();
            
            IBlogRepository blogRepository = new BlogRepository(context);
            bool result = blogRepository.UpdatePost(new SubmitPostViewModel
            {
                Id = 500,
                Title = "Random title"
            });
            
            Assert.AreEqual(false, result);
            Assert.AreEqual(2, context.Posts.ToList().Count);
        }

        [Test]
        public void TestDeletePostSuccess()
        {
            var context = _contextBuilder
                .SetBlogPosts()
                .Build();
            
            IBlogRepository blogRepository = new BlogRepository(context);
            bool result = blogRepository.DeletePost(200);
            
            Assert.AreEqual(true, result);
            Assert.AreEqual(1, context.Posts.ToList().Count);
        }

        [Test]
        public void TestDeletePostFail()
        {
            var context = _contextBuilder
                .SetBlogPosts()
                .Build();
            
            IBlogRepository blogRepository = new BlogRepository(context);
            bool result = blogRepository.DeletePost(500);
            context.SaveChanges();
            
            Assert.AreEqual(false, result);
            Assert.AreEqual(2, context.Posts.ToList().Count);
        }
    }
}