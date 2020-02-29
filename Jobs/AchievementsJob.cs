using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using fantasy_hoops.Database;
using fantasy_hoops.Helpers;
using fantasy_hoops.Models;
using fantasy_hoops.Models.Achievements;
using fantasy_hoops.Repositories.Interfaces;
using fantasy_hoops.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

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

        public void ExecuteAllAchievements()
        {
            Task.Run(() => ExecuteStreakAchievements());
            if (CommonFunctions.UTCToEastern(NextGameJob.PREVIOUS_GAME).DayOfWeek == DayOfWeek.Sunday)
            {
                Task.Run(() => ExecuteWeeklyWinners());
            }
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
                User user = _context.Users.Find(userAchievement.UserID);
                if (user == null || userAchievement.Progress > user.Streak)
                {
                    continue;
                }

                // Add Progress
                if (userAchievement.Progress.CompareTo(userAchievement.LevelUpGoal) == -1)
                {
                    userAchievement.Progress = user.Streak;
                    continue;
                }

                // Level Up
                if (userAchievement.Progress.CompareTo(userAchievement.LevelUpGoal - 1) == 0)
                {
                    userAchievement.Progress = user.Streak;
                    userAchievement.Level++;
                    userAchievement.LevelUpGoal *= 2;
                    
                    _pushService.SendAchievementLevelUpNotification(Tuple.Create(
                        userAchievement.UserID, userAchievement.Achievement.Title, userAchievement.Level
                        ));
                }
            }
            
            _context.SaveChanges();
        }

        public void ExecuteWeeklyWinners()
        {
            DateTime previousECT = CommonFunctions.UTCToEastern(NextGameJob.PREVIOUS_GAME);
            if (previousECT.Date.DayOfWeek != DayOfWeek.Sunday)
            {
                return;
            }
            
            var winnerTuple = _context.UserLineups
                .Where(lineup => lineup.Date > previousECT.AddDays(-6).Date
                                 && lineup.Date <= CommonFunctions.UTCToEastern(NextGameJob.PREVIOUS_GAME).Date)
                .ToList()
                .GroupBy(lineup => lineup.UserID)
                .Select(lineup => (lineup.Max(x => x.UserID), lineup.Sum(x => x.FP)))
                .OrderByDescending(lineup => lineup.Item2)
                .FirstOrDefault();
            User winner = _context.Users.FirstOrDefault(user => user.Id.Equals(winnerTuple.Item1));
            
            if (winner == null)
            {
                return;
            }

            UserAchievement userAchievement = _context.UserAchievements
                .Include(ua => ua.Achievement)
                .FirstOrDefault(ua => ua.UserID.Equals(winner.Id) && ua.Achievement.Title.Equals("Winner"));

            if (userAchievement == null)
            {
                Achievement achievement = _context.Achievements
                    .FirstOrDefault(a => a.Title.Equals("Winner"));
                if (achievement == null)
                {
                    return;
                }

                _context.UserAchievements
                    .Add(new UserAchievement
                    {
                        UserID = winner.Id,
                        AchievementID = achievement.Id,
                        Progress = 1,
                        LevelUpGoal = 1,
                        IsAchieved = true
                    });
            }
            else
            {
                userAchievement.IsAchieved = true;
                userAchievement.Progress = userAchievement.LevelUpGoal;
            }

            _context.SaveChanges();
            
            _pushService.SendAchievementUnlockedNotification(Tuple.Create(
                userAchievement.UserID, userAchievement.Achievement.Title, userAchievement.Achievement.CompletedMessage
                ));
        }
    }
}