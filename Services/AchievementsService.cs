using System.Linq;
using System.Threading.Tasks;
using fantasy_hoops.Database;
using fantasy_hoops.Models;
using fantasy_hoops.Models.Achievements;
using fantasy_hoops.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace fantasy_hoops.Services
{
    public class AchievementsService : IAchievementsService
    {
        private readonly GameContext _context;
        private readonly UserManager<User> _userManager;

        public AchievementsService(UserManager<User> userManager)
        {
            _context = new GameContext();
            _userManager = userManager;
        }
        
        public async void AssignAchievements(string userName)
        {
            User user = await _userManager.FindByNameAsync(userName);
            _context.Achievements.ToList()
            .ForEach(achievement =>
            {
                _context.UserAchievements.Add(new UserAchievement
                {
                    Achievement = achievement,
                    User = user,
                    Level = 1,
                    LevelUpGoal = achievement.GoalBase
                });
            });
            _context.SaveChanges();
        }
    }
}