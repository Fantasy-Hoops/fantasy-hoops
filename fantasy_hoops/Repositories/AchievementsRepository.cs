using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using fantasy_hoops.Database;
using fantasy_hoops.Dtos;
using fantasy_hoops.Models;
using fantasy_hoops.Models.Achievements;
using fantasy_hoops.Repositories.Interfaces;

namespace fantasy_hoops.Repositories
{
    public class AchievementsRepository : IAchievementsRepository
    {
        private readonly GameContext _context;

        public AchievementsRepository(GameContext context = null)
        {
            _context = context ?? new GameContext();
        }
        
        public List<AchievementDto> GetExistingAchievements()
        {
            return new GameContext().Achievements
                .Select(achievement => new AchievementDto
                {
                    Id = achievement.Id,
                    Type = achievement.Type,
                    Title = achievement.Title,
                    Description = achievement.Description
                        .Replace("{}", achievement.GoalBase.ToString()),
                    CompletedMessage = achievement.CompletedMessage,
                    Icon = achievement.Icon,
                    GoalBase = achievement.GoalBase
                })
                .ToList();
        }

        public Dictionary<String, List<UserAchievementDto>> GetAllUserAchievements()
        {
            return new GameContext().UserAchievements
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
            return new GameContext().UserAchievements
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
                .OrderByDescending(ua => ua.Level)
                .ThenByDescending(ua => ua.IsAchieved)
                .ThenByDescending(ua => ua.Progress / ua.LevelUpGoal)
                .ToList();
        }

        public bool AchievementExists(Achievement achievement)
        {
            return new GameContext().Achievements.Any(a => a.Title.Equals(achievement.Title)
                                                  && a.Type == achievement.Type);
        }

        public bool SaveAchievement(Achievement achievement)
        {
            GameContext context = new GameContext();
            if (AchievementExists(achievement))
            {
                return false;
            }
            
            context.Achievements.Add(achievement);
            int entitiesWritten = context.SaveChanges();
            return entitiesWritten > 0;
        }
        

        public bool UserAchievementExists(User user, Achievement achievement)
        {
            return UserAchievementExists(user.Id, achievement);
        }

        public bool UserAchievementExists(string userId, Achievement achievement)
        {
            return new GameContext().UserAchievements.Any(ua => ua.UserID == userId
                                                       && ua.Achievement.Title.Equals(achievement.Title)
                                                       && ua.Achievement.Type == achievement.Type);
        }

        public bool AddUserAchievement(UserAchievement userAchievement)
        {
            GameContext context = new GameContext();
            context.UserAchievements.Add(userAchievement);
            return context.SaveChanges() != 0;
        }
    }
}