using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using fantasy_hoops.Database;
using fantasy_hoops.Dtos;
using fantasy_hoops.Models.Achievements;
using fantasy_hoops.Repositories.Interfaces;

namespace fantasy_hoops.Repositories
{
    public class AchievementsRepository : IAchievementsRepository
    {
        private readonly GameContext _context;

        public AchievementsRepository()
        {
            _context = new GameContext();
        }
        
        public List<Achievement> GetExistingAchievements()
        {
            return _context.Achievements
                .ToList();
        }

        public Dictionary<String, List<UserAchievementDto>> GetAllUserAchievements()
        {
            return _context.UserAchievements
                .Include(achievement => achievement.Achievement)
                .Include(achievement => achievement.User)
                .Select(achievement => new UserAchievementDto
                {
                    UserId = achievement.UserID,
                    UserName = achievement.User.UserName,
                    Progress = achievement.Progress,
                    Level = achievement.Level,
                    LevelUpGoal = achievement.LevelUpGoal,
                    IsAchieved = achievement.IsAchieved,
                    Achievement = new AchievementDto
                    {
                        Id = achievement.AchievementID,
                        Type = achievement.Achievement.Type,
                        Title = achievement.Achievement.Title,
                        Description = achievement.Achievement.Description
                            .Replace("{}", achievement.LevelUpGoal.ToString()),
                        CompletedMessage = achievement.Achievement.CompletedMessage,
                        Icon = achievement.Achievement.Icon,
                        GoalBase = achievement.Achievement.GoalBase
                    }
                })
                .ToList()
                .GroupBy(achievement => achievement.UserName)
                .ToDictionary(group => group.Key, group => group.ToList());
        }

        public List<UserAchievementDto> GetUserAchievements(string userId)
        {
            return _context.UserAchievements
                .Include(achievement => achievement.Achievement)
                .Include(achievement => achievement.User)
                .Where(achievement => achievement.UserID.Equals(userId))
                .Select(achievement => new UserAchievementDto
                {
                    Progress = achievement.Progress,
                    Level = achievement.Level,
                    LevelUpGoal = achievement.LevelUpGoal,
                    IsAchieved = achievement.IsAchieved,
                    Achievement = new AchievementDto
                    {
                        Id = achievement.AchievementID,
                        Type = achievement.Achievement.Type,
                        Title = achievement.Achievement.Title,
                        Description = achievement.Achievement.Description
                            .Replace("{}", achievement.LevelUpGoal.ToString()),
                        CompletedMessage = achievement.Achievement.CompletedMessage,
                        Icon = achievement.Achievement.Icon,
                        GoalBase = achievement.Achievement.GoalBase
                    }
                })
                .OrderByDescending(ua => ua.IsAchieved)
                .ThenByDescending(ua => ua.Level)
                .ToList();
        }

        public bool AchievementExists(Achievement achievement)
        {
            return _context.Achievements.Any(a => a.Title.Equals(achievement.Title)
                                                  && a.Type == achievement.Type);
        }

        public bool SaveAchievement(Achievement achievement)
        {
            if (AchievementExists(achievement))
            {
                return false;
            }
            
            _context.Achievements.Add(achievement);
            int entitiesWritten = _context.SaveChanges();
            return entitiesWritten > 0;
        }
    }
}