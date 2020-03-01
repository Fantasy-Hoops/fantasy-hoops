using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using fantasy_hoops.Database;
using fantasy_hoops.Helpers;
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
        private readonly DateTime _ectPrevious;

        public AchievementsJob(IPushService pushService, IAchievementsService achievementsService, IAchievementsRepository achievementsRepository)
        {
            _context = new GameContext();
            _pushService = pushService;
            _achievementsService = achievementsService;
            _achievementsRepository = achievementsRepository;
            _ectPrevious = CommonFunctions.UTCToEastern(NextGameJob.PREVIOUS_GAME);
        }

        public void ExecuteAllAchievements()
        {
            Task.Run(() => ExecuteStreakAchievements());
            Task.Run(() => ExecuteKobeAchievements());
            
            if (_ectPrevious.DayOfWeek == DayOfWeek.Sunday)
            {
                Task.Run(() => ExecuteWeeklyWinners());
            }

            if (DateTime.DaysInMonth(_ectPrevious.Year, _ectPrevious.Month) == _ectPrevious.Day)
            {
                Task.Run(() => ExecuteMonthWinners());
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
            if (_ectPrevious.Date.DayOfWeek != DayOfWeek.Sunday)
            {
                return;
            }
            
            var winnerTuple = _context.UserLineups
                .Where(lineup => lineup.Date >= _ectPrevious.AddDays(-6).Date
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

                userAchievement = new UserAchievement
                {
                    UserID = winner.Id,
                    AchievementID = achievement.Id,
                    Progress = 1,
                    LevelUpGoal = 1,
                    IsAchieved = true
                };
                _context.UserAchievements
                    .Add(userAchievement);
            }
            else
            {
                if (userAchievement.IsAchieved)
                {
                    return;
                }
                
                userAchievement.IsAchieved = true;
                userAchievement.Progress = userAchievement.LevelUpGoal;
            }

            _context.SaveChanges();
            
            _pushService.SendAchievementUnlockedNotification(Tuple.Create(
                userAchievement.UserID, userAchievement.Achievement.Title, userAchievement.Achievement.CompletedMessage
                ));
        }

        public void ExecuteKobeAchievements()
        {
            List<String> kobeLineupUserIds = _context.UserLineups
                .Where(lineup => lineup.Date.Equals(_ectPrevious.Date))
                .Where(lineup => isKobeNumber(lineup.Pg) || isKobeNumber(lineup.Sg) || isKobeNumber(lineup.Sf)
                                 || isKobeNumber(lineup.Pf) || isKobeNumber(lineup.C))
                .Select(lineup => lineup.UserID)
                .ToList();

            if (kobeLineupUserIds.Count == 0)
            {
                return;
            }

            foreach (var userId in kobeLineupUserIds)
            {
                User kobeWinner = _context.Users.FirstOrDefault(user => user.Id.Equals(userId));
                if (kobeWinner == null)
                {
                    continue;
                }
                
                UserAchievement userAchievement = _context.UserAchievements
                    .Include(ua => ua.Achievement)
                    .FirstOrDefault(ua => ua.UserID.Equals(userId) && ua.Achievement.Title.Equals("Kobe"));

                if (userAchievement == null)
                {
                    Achievement achievement = _context.Achievements
                        .FirstOrDefault(a => a.Title.Equals("Kobe"));
                    if (achievement == null)
                    {
                        return;
                    }

                    userAchievement = new UserAchievement
                    {
                        UserID = userId,
                        AchievementID = achievement.Id,
                        Progress = 1,
                        LevelUpGoal = 1,
                        IsAchieved = true
                    };
                    
                    _context.UserAchievements
                        .Add(userAchievement);
                }
                else
                {
                    if (userAchievement.IsAchieved)
                    {
                        continue;
                    }

                    userAchievement.IsAchieved = true;
                    userAchievement.Progress = userAchievement.LevelUpGoal;
                }
                
                _context.SaveChanges();
                _pushService.SendAchievementUnlockedNotification(Tuple.Create(
                    userId, userAchievement.Achievement.Title, userAchievement.Achievement.CompletedMessage
                ));
            }
        }

        private bool isKobeNumber(Player player)
        {
            int playerNumber = int.Parse(player.Number);
            return playerNumber == 8 || playerNumber == 24;
        }

        public void ExecuteMonthWinners()
        {
            if (DateTime.DaysInMonth(_ectPrevious.Year, _ectPrevious.Month) != _ectPrevious.Day)
            {
                return;
            }
            
            var firstDayOfMonth = new DateTime(_ectPrevious.Year, _ectPrevious.Month, 1);
            var winnerTuple = _context.UserLineups
                .Where(lineup => lineup.Date >= firstDayOfMonth.Date
                                 && lineup.Date <= _ectPrevious.Date)
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
                .FirstOrDefault(ua => ua.UserID.Equals(winner.Id) && ua.Achievement.Title.Equals("Month King"));

            if (userAchievement == null)
            {
                Achievement achievement = _context.Achievements
                    .FirstOrDefault(a => a.Title.Equals("Month King"));
                if (achievement == null)
                {
                    return;
                }

                userAchievement = new UserAchievement
                {
                    UserID = winner.Id,
                    AchievementID = achievement.Id,
                    Progress = 1,
                    LevelUpGoal = 1,
                    IsAchieved = true
                };
                _context.UserAchievements
                    .Add(userAchievement);
            }
            else
            {
                if (userAchievement.IsAchieved)
                {
                    return;
                }
                
                userAchievement.IsAchieved = true;
                userAchievement.Progress = userAchievement.LevelUpGoal;
            }

            _context.SaveChanges();
            
            _pushService.SendAchievementUnlockedNotification(Tuple.Create(
                userAchievement.UserID, userAchievement.Achievement.Title, userAchievement.Achievement.CompletedMessage
                ));
        }

        public void ExecuteBalancerAchievements()
        {
            
        }

        public void ExecuteInjuredAchievements()
        {
            
        }

        public void ExecuteStrategistAchievements()
        {
            
        }

        public void ExecuteVeteranAchievements()
        {
            
        }
    }
}