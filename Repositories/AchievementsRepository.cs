using System.Collections.Generic;
using System.Linq;
using fantasy_hoops.Database;
using fantasy_hoops.Dtos;
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
        
        public List<AchievementDto> GetExistingAchievements()
        {
            return _context.Achievements
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
    }
}