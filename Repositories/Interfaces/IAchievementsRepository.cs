using System;
using System.Collections.Generic;
using fantasy_hoops.Dtos;

namespace fantasy_hoops.Repositories.Interfaces
{
    public interface IAchievementsRepository
    {
        public List<AchievementDto> GetExistingAchievements();
        public Dictionary<String, List<UserAchievementDto>> GetAllUserAchievements();
        public List<UserAchievementDto> GetUserAchievements(String userId);
    }
}