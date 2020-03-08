using System;
using System.Threading.Tasks;
using fantasy_hoops.Models.Achievements;

namespace fantasy_hoops.Services.Interfaces
{
    public interface IAchievementsService
    {
        public Task AssignAchievements(String userName);
        public bool CreateAchievement(Achievement achievement);
    }
}