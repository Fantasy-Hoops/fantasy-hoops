using System.Collections.Generic;
using System.Linq;
using fantasy_hoops.Models.Enums;
using fantasy_hoops.Repositories;
using fantasy_hoops.Repositories.Interfaces;
using fantasy_hoops.Services;
using fantasy_hoops.Services.Interfaces;
using fantasy_hoops.Tests.Mocks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using NUnit.Framework;

namespace fantasy_hoops.Tests.Services
{
    public class BlogServiceTests
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
        public void TestApprovePostSuccess()
        {
            var context = _contextBuilder
                .SetBlogPosts()
                .Build();

            IBlogRepository blogRepository = new BlogRepository(context);
            
            IBlogService blogService =new BlogService(blogRepository);
            bool result = blogService.ApprovePost(200);
            
            Assert.IsTrue(result);
            Assert.AreEqual(PostStatus.APPROVED, context.Posts.Find(200).Status);
        }
        
        [Test]
        public void TestApprovePostFail()
        {
            var context = _contextBuilder
                .SetBlogPosts()
                .Build();

            IBlogRepository blogRepository = new BlogRepository(context);
            
            IBlogService blogService =new BlogService(blogRepository);
            bool result = blogService.ApprovePost(999);
            
            Assert.IsFalse(result);
        }
    }
}