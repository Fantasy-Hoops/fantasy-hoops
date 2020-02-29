using System;
using fantasy_hoops.Models.Achievements;

namespace fantasy_hoops.Services.Interfaces
{
    public interface IAchievementsService
    {
        public void AssignAchievements(String userName);
        public bool CreateAchievement(Achievement achievement);
    }
}