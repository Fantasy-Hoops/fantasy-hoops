using System.Collections.Generic;
using fantasy_hoops.Dtos;
using fantasy_hoops.Models;
using fantasy_hoops.Models.Achievements;
using fantasy_hoops.Repositories;
using fantasy_hoops.Repositories.Interfaces;
using fantasy_hoops.Services.Interfaces;

namespace fantasy_hoops.Services
{
    public class AchievementsService : IAchievementsService
    {
        private readonly IUserRepository _userRepository;
        private readonly IAchievementsRepository _achievementsRepository;

        public AchievementsService(IAchievementsRepository achievementsRepository)
        {
            _achievementsRepository = achievementsRepository;
            _userRepository = new UserRepository();
        }
        
        public bool AssignAchievements(string userName)
        {
            
            User user = _userRepository.GetUserByName(userName);
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

            var users = _userRepository.GetAllUsers();
            foreach (var user in users)
            {
                if (_achievementsRepository.UserAchievementExists(user.UserId, achievement))
                {
                    continue;
                }
                
                UserAchievement userAchievement = new UserAchievement
                {
                    AchievementID = achievement.Id,
                    UserID = user.UserId,
                    Level = 1,
                    LevelUpGoal = achievement.GoalBase
                };
                _achievementsRepository.AddUserAchievement(userAchievement);
            }
            return true;
        }
    }
}