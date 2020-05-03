using System;
using System.Collections.Generic;
using System.Linq;
using fantasy_hoops.Dtos;
using fantasy_hoops.Models.Achievements;
using fantasy_hoops.Models.Enums;
using fantasy_hoops.Repositories;
using fantasy_hoops.Repositories.Interfaces;
using fantasy_hoops.Tests.Mocks;
using NUnit.Framework;

namespace fantasy_hoops.Tests.Repositories
{
    public class AchievementsRepositoryTests
    {
        private readonly ContextMock.Builder _contextBuilder = new ContextMock.Builder();

        [Test]
        public void TestGetExistingAchievements()
        {
           var context = _contextBuilder
                .SetAchievements()
                .Build();
            
            IAchievementsRepository achievementsRepository = new AchievementsRepository(context);
            List<AchievementDto> achievements = achievementsRepository.GetExistingAchievements();
            
            Assert.NotNull(achievements);
            Assert.AreEqual(2, achievements.Count);
            Assert.AreEqual(123, achievements[0].Id);
            Assert.AreEqual("Achievement 1", achievements[0].Title);
            Assert.AreEqual(456, achievements[1].Id);
            Assert.AreEqual("Achievement 2", achievements[1].Title);
        }
        
        [Test]
        public void TestGetAllUserAchievements()
        {
            var context = _contextBuilder
                .SetAchievements()
                .SetUserAchievements()
                .Build();
            
            IAchievementsRepository achievementsRepository = new AchievementsRepository(context);
            Dictionary<String, List<UserAchievementDto>> userAchievements = achievementsRepository.GetAllUserAchievements();
            
            Assert.NotNull(userAchievements);
            Assert.AreEqual(2, userAchievements.Keys.Count);
            Assert.AreEqual(2, userAchievements.Values.Count);
            Assert.AreEqual(2, userAchievements["xUser"].Count);
            Assert.AreEqual("Achievement 1", userAchievements["xUser"][0].Achievement.Title);
            Assert.AreEqual(1, userAchievements["yUser"].Count);
            Assert.AreEqual("Achievement 2", userAchievements["yUser"][0].Achievement.Title);
        }

        [Test]
        public void TestGetUserAchievements()
        {
            var context = _contextBuilder
                .SetAchievements()
                .SetUserAchievements()
                .Build();
            
            IAchievementsRepository achievementsRepository = new AchievementsRepository(context);
            List<UserAchievementDto> userAchievements = achievementsRepository.GetUserAchievements("xxx");
            
            Assert.NotNull(userAchievements);
            Assert.AreEqual(2, userAchievements.Count);
            Assert.AreEqual("Achievement 1", userAchievements[0].Achievement.Title);
        }

        [Test]
        public void TestAchievementExistsTrue()
        {
            var context = _contextBuilder
                .SetAchievements()
                .Build();
            
            IAchievementsRepository achievementsRepository = new AchievementsRepository(context);

            Achievement newAchievement = new Achievement
            {
                Title = "Achievement 1",
                Type = AchievementType.SINGLE_LEVEL
            };
            
            bool achievementExists = achievementsRepository.AchievementExists(newAchievement);
            
            Assert.IsTrue(achievementExists);
        }
        
        [Test]
        public void TestAchievementExistsFalse()
        {
            var context = _contextBuilder
                .SetAchievements()
                .Build();
            
            IAchievementsRepository achievementsRepository = new AchievementsRepository(context);

            Achievement newAchievement = new Achievement
            {
                Title = "Random Title",
                Type = AchievementType.SINGLE_LEVEL
            };
            
            bool achievementExists = achievementsRepository.AchievementExists(newAchievement);
            
            Assert.IsFalse(achievementExists);
        }

        [Test]
        public void TestSaveAchievementSuccess()
        {
            var context = _contextBuilder
                .SetAchievements()
                .Build();
            
            IAchievementsRepository achievementsRepository = new AchievementsRepository(context);

            int initialAchievementsCount = fantasy_hoops.Mocks.Achievements.MockedAchievements.Count;
            Achievement newAchievement = new Achievement
            {
                Title = "Random Title",
                Type = AchievementType.SINGLE_LEVEL
            };
            
            bool achievementSaved = achievementsRepository.SaveAchievement(newAchievement);
            
            Assert.IsTrue(achievementSaved);
            Assert.AreEqual(initialAchievementsCount + 1, context.Achievements.Count());
        }

        [Test]
        public void TestSaveExistingAchievement()
        {
            var context = _contextBuilder
                .SetAchievements()
                .Build();
            
            IAchievementsRepository achievementsRepository = new AchievementsRepository(context);

            int initialAchievementsCount = fantasy_hoops.Mocks.Achievements.MockedAchievements.Count;
            Achievement newAchievement = new Achievement
            {
                Title = "Achievement 2",
                Type = AchievementType.MULTI_LEVEL
            };
            
            bool achievementSaved = achievementsRepository.SaveAchievement(newAchievement);
            
            Assert.IsFalse(achievementSaved);
            Assert.AreEqual(initialAchievementsCount, context.Achievements.Count());
        }

        [Test]
        public void TestUserAchievementExistsSuccess()
        {
            var context = _contextBuilder
                .SetAchievements()
                .SetUserAchievements()
                .Build();
            
            IAchievementsRepository achievementsRepository = new AchievementsRepository(context);

            Achievement testAchievement = fantasy_hoops.Mocks.Achievements.MockedAchievements[0];
            
            bool userAchievementExists = achievementsRepository.UserAchievementExists("xxx", testAchievement);
            
            Assert.IsTrue(userAchievementExists);
        }

        [Test]
        public void TestUserAchievementExistsFail()
        {
            var context = _contextBuilder
                .SetAchievements()
                .SetUserAchievements()
                .Build();
            
            IAchievementsRepository achievementsRepository = new AchievementsRepository(context);

            Achievement testAchievement = fantasy_hoops.Mocks.Achievements.MockedAchievements[0];
            
            bool userAchievementExists = achievementsRepository.UserAchievementExists("yyy", testAchievement);
            
            Assert.IsFalse(userAchievementExists);
        }

        [Test]
        public void TestAddUserAchievement()
        {
            var context = _contextBuilder
                .SetAchievements()
                .SetUserAchievements()
                .Build();
            
            IAchievementsRepository achievementsRepository = new AchievementsRepository(context);

            int initialUserAchievementsCount = context.UserAchievements.Count();
            UserAchievement newUserAchievement = new UserAchievement
            {
                UserID = "xxx",
                AchievementID = 456
            };
            
            bool userAchievementAdded = achievementsRepository.AddUserAchievement(newUserAchievement);
            
            Assert.IsTrue(userAchievementAdded);
            Assert.AreEqual(initialUserAchievementsCount + 1, context.UserAchievements.Count());
        }
    }
}