using System;
using System.Collections.Generic;
using fantasy_hoops.Dtos;
using fantasy_hoops.Models;
using fantasy_hoops.Models.Achievements;

namespace fantasy_hoops.Repositories.Interfaces
{
    public interface IAchievementsRepository
    {
        public List<AchievementDto> GetExistingAchievements();
        public Dictionary<String, List<UserAchievementDto>> GetAllUserAchievements();
        public List<UserAchievementDto> GetUserAchievements(String userId);
        public bool AchievementExists(Achievement achievement);
        public bool SaveAchievement(Achievement achievement);
        bool UserAchievementExists(User user, Achievement achievement);
        bool AddUserAchievement(UserAchievement userAchievement);
    }
}