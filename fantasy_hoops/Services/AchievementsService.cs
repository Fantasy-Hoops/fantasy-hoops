using System.Collections.Generic;
using fantasy_hoops.Dtos;
using fantasy_hoops.Models;
using fantasy_hoops.Models.Achievements;
using fantasy_hoops.Repositories.Interfaces;
using fantasy_hoops.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace fantasy_hoops.Services
{
    public class AchievementsService : IAchievementsService
    {
        private readonly UserManager<User> _userManager;
        private readonly IAchievementsRepository _achievementsRepository;

        public AchievementsService(UserManager<User> userManager, IAchievementsRepository achievementsRepository)
        {
            _userManager = userManager;
            _achievementsRepository = achievementsRepository;
        }
        
        public bool AssignAchievements(string userName)
        {
            User user = _userManager.FindByNameAsync(userName).Result;
            if (user == null)
            {
                return false;
            }
            
            List<AchievementDto> achievements = _achievementsRepository.GetExistingAchievements();
            foreach (AchievementDto achievement in achievements)
            {
                UserAchievement userAchievement = new UserAchievement
                {
                    AchievementID = achievement.Id,
                    UserID = user.Id,
                    Level = 1,
                    LevelUpGoal = achievement.GoalBase
                };
                
                if (!_achievementsRepository.AddUserAchievement(userAchievement))
                {
                    return false;
                }
            }

            return true;
        }

        public bool CreateAchievement(Achievement achievement)
        {
            if (!_achievementsRepository.SaveAchievement(achievement))
            {
                return false;
            }

            var users = _userManager.Users;
            foreach (var user in users)
            {
                if (_achievementsRepository.UserAchievementExists(user, achievement))
                {
                    continue;
                }
                
                UserAchievement userAchievement = new UserAchievement
                {
                    AchievementID = achievement.Id,
                    UserID = user.Id,
                    Level = 1,
                    LevelUpGoal = achievement.GoalBase
                };
                _achievementsRepository.AddUserAchievement(userAchievement);
            }
            return true;
        }
    }
}