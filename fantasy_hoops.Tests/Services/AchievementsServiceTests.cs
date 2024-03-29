using System.Collections.Generic;
using System.Linq;
using fantasy_hoops.Models;
using fantasy_hoops.Repositories;
using fantasy_hoops.Services;
using fantasy_hoops.Services.Interfaces;
using fantasy_hoops.Tests.Mocks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using NUnit.Framework;

namespace fantasy_hoops.Tests.Services
{
    public class AchievementsServiceTests
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
        public void TestAssignAchievements()
        {
            var context = _contextBuilder
                .SetUsers(new List<User>
                {
                    new User
                    {
                        Id = "zzz",
                        UserName = "zUser",
                        Email = "zzz@test.com"
                    }
                })
                .SetAchievements()
                .SetUserAchievements()
                .Build();

            var userRepositoryMock = new UserRepository(context);
            var achievementRepository = new AchievementsRepository(context);

            IAchievementsService achievementsService =
                new AchievementsService(achievementRepository, userRepositoryMock);
            bool result = achievementsService.AssignAchievements("zUser");
            
            Assert.IsTrue(result);
            Assert.AreEqual(2, context.UserAchievements.Count(x => x.UserID.Equals("zzz")));
        }
    }
}