using System.Linq;
using System.Threading.Tasks;
using fantasy_hoops.Database;
using fantasy_hoops.Models;
using fantasy_hoops.Models.Achievements;
using fantasy_hoops.Repositories.Interfaces;
using fantasy_hoops.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace fantasy_hoops.Services
{
    public class AchievementsService : IAchievementsService
    {
        private readonly GameContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IAchievementsRepository _achievementsRepository;

        public AchievementsService(UserManager<User> userManager, IAchievementsRepository achievementsRepository)
        {
            _context = new GameContext();
            _userManager = userManager;
            _achievementsRepository = achievementsRepository;
        }
        
        public async Task AssignAchievements(string userName)
        {
            User user = await _userManager.FindByNameAsync(userName);
            _achievementsRepository.GetExistingAchievements().ToList()
            .ForEach(achievement =>
            {
                _context.UserAchievements.Add(new UserAchievement
                {
                    AchievementID = achievement.Id,
                    UserID = user.Id,
                    Level = 1,
                    LevelUpGoal = achievement.GoalBase
                });
            });
            _context.SaveChanges();
        }

        public bool CreateAchievement(Achievement achievement)
        {
            if (!_achievementsRepository.SaveAchievement(achievement))
            {
                return false;
            }

            var users = _userManager.Users;
            foreach (var user in users)
            {
                if (UserAchievementExists(user, achievement))
                {
                    continue;
                }
                
                UserAchievement userAchievement = new UserAchievement
                {
                    AchievementID = achievement.Id,
                    UserID = user.Id,
                    Level = 1,
                    LevelUpGoal = achievement.GoalBase
                };
                _context.UserAchievements.Add(userAchievement);
            }
            int entitiesWritten = _context.SaveChanges();
            return entitiesWritten == users.Count();
        }

        private bool UserAchievementExists(User user, Achievement achievement)
        {
            return _context.UserAchievements.Any(ua => ua.UserID == user.Id
                                                       && ua.Achievement.Title.Equals(achievement.Title)
                                                       && ua.Achievement.Type == achievement.Type);
        }
    }
}