using System;
using System.Data.Entity;
using System.Linq;
using fantasy_hoops.Database;
using fantasy_hoops.Models;
using fantasy_hoops.Models.Achievements;
using fantasy_hoops.Repositories.Interfaces;
using fantasy_hoops.Services.Interfaces;

namespace fantasy_hoops.Jobs
{
    public class AchievementsJob
    {
        private readonly GameContext _context;
        private readonly IPushService _pushService;
        private readonly IAchievementsService _achievementsService;
        private readonly IAchievementsRepository _achievementsRepository;

        public AchievementsJob(IPushService pushService, IAchievementsService achievementsService, IAchievementsRepository achievementsRepository)
        {
            _context = new GameContext();
            _pushService = pushService;
            _achievementsService = achievementsService;
            _achievementsRepository = achievementsRepository;
        }

        public void ExecuteStreakAchievements()
        {
            Achievement achievement = _context.Achievements
                .FirstOrDefault(a => a.Title.Equals("Wildfire"));

            if (achievement == null)
            {
                return;
            }
            
            var userAchievements = _context.UserAchievements
                .Include(userAchievement => userAchievement.Achievement)
                .Include(userAchievement => userAchievement.User)
                .Where(userAchievement => achievement.Id.Equals(userAchievement.AchievementID))
                .ToList();

            foreach (var userAchievement in userAchievements)
            {
                if (userAchievement.Progress > userAchievement.User.Streak)
                {
                    continue;
                }

                // Add Progress
                if (userAchievement.Progress.CompareTo(userAchievement.LevelUpGoal) == -1)
                {
                    userAchievement.Progress = userAchievement.User.Streak;
                    continue;
                }

                // Level Up
                if (userAchievement.Progress.CompareTo(userAchievement.LevelUpGoal - 1) == 0)
                {
                    userAchievement.Progress = userAchievement.User.Streak;
                    userAchievement.Level++;
                    userAchievement.LevelUpGoal *= 2;
                    
                    _pushService.SendAchievementLevelUpNotification(Tuple.Create(
                        userAchievement.UserID, userAchievement.Achievement.Title, userAchievement.Level
                        ));
                }
            }
            
            _context.SaveChanges();
        }
    }
}