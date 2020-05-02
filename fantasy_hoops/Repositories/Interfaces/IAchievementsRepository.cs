using System;
using System.Collections.Generic;
using fantasy_hoops.Dtos;
using fantasy_hoops.Models;
using fantasy_hoops.Models.Achievements;

namespace fantasy_hoops.Repositories.Interfaces
{
    public interface IAchievementsRepository
    {
        List<AchievementDto> GetExistingAchievements();
        Dictionary<String, List<UserAchievementDto>> GetAllUserAchievements();
        List<UserAchievementDto> GetUserAchievements(String userId);
        bool AchievementExists(Achievement achievement);
        bool SaveAchievement(Achievement achievement);
        bool UserAchievementExists(String userId, Achievement achievement);
        bool UserAchievementExists(User user, Achievement achievement);
        bool AddUserAchievement(UserAchievement userAchievement);
    }
}