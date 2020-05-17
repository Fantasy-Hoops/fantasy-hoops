using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using fantasy_hoops.Database;
using fantasy_hoops.Helpers;
using fantasy_hoops.Models;
using fantasy_hoops.Models.Achievements;
using fantasy_hoops.Services.Interfaces;
using FluentScheduler;

namespace fantasy_hoops.Jobs
{
    public class AchievementsJob : IJob
    {
        private readonly IPushService _pushService;
        private readonly DateTime _ectPrevious;

        public AchievementsJob(IPushService pushService)
        {
            _pushService = pushService;
            _ectPrevious = CommonFunctions.Instance.UTCToEastern(RuntimeUtils.PREVIOUS_GAME);
        }

        public void Execute()
        {
            Task.Run(() => ExecuteStreakAchievements());
            Task.Run(() => ExecuteVeteranAchievements());
            Task.Run(() => ExecuteKobeAchievements());
            Task.Run(() => ExecuteBalancerAchievements());
            Task.Run(() => ExecuteInjuredAchievements());
            Task.Run(() => ExecuteStrategistAchievements());
            Task.Run(() => ExecuteAgentZeroAchievements());
            
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
            GameContext context = new GameContext();
            Achievement achievement = context.Achievements
                .FirstOrDefault(a => a.Title.Equals("Wildfire"));

            if (achievement == null)
            {
                return;
            }

            var userAchievements = context.UserAchievements
                .Include(userAchievement => userAchievement.Achievement)
                .Include(userAchievement => userAchievement.User)
                .Where(userAchievement => achievement.Id.Equals(userAchievement.AchievementID))
                .ToList();

            foreach (var userAchievement in userAchievements)
            {
                User user = context.Users.Find(userAchievement.UserID);
                if (user == null || userAchievement.Progress >= user.Streak)
                {
                    continue;
                }

                // Add Progress
                if (userAchievement.Progress.CompareTo(userAchievement.LevelUpGoal) == -1)
                {
                    userAchievement.Progress = user.Streak;
                }

                // Level Up
                if (userAchievement.Progress.CompareTo(userAchievement.LevelUpGoal) == 0)
                {
                    userAchievement.Progress = user.Streak;
                    userAchievement.Level++;
                    userAchievement.LevelUpGoal *= 2;

                    _pushService.SendAchievementLevelUpNotification(Tuple.Create(
                        userAchievement.UserID, userAchievement.Achievement.Title, userAchievement.Level
                    ));
                }
            }

            context.SaveChanges();
        }

        public void ExecuteWeeklyWinners()
        {
            if (_ectPrevious.Date.DayOfWeek != DayOfWeek.Sunday)
            {
                return;
            }

            GameContext context = new GameContext();
            Achievement achievement = context.Achievements
                .FirstOrDefault(a => a.Title.Equals("Winner"));
            if (achievement == null)
            {
                return;
            }

            var winnerTuple = context.UserLineups
                .Where(lineup => lineup.Date >= _ectPrevious.AddDays(-6).Date
                                 && lineup.Date <= CommonFunctions.Instance.UTCToEastern(RuntimeUtils.PREVIOUS_GAME).Date)
                .ToList()
                .GroupBy(lineup => lineup.UserID)
                .Select(lineup => (lineup.Max(x => x.UserID), lineup.Sum(x => x.FP)))
                .OrderByDescending(lineup => lineup.Item2)
                .FirstOrDefault();
            
            User winner = context.Users.FirstOrDefault(user => user.Id.Equals(winnerTuple.Item1));

            if (winner == null)
            {
                return;
            }

            UserAchievement userAchievement = context.UserAchievements
                .Include(ua => ua.Achievement)
                .FirstOrDefault(ua => ua.UserID.Equals(winner.Id) && ua.Achievement.Title.Equals("Winner"));

            if (userAchievement == null)
            {
                userAchievement = new UserAchievement
                {
                    UserID = winner.Id,
                    AchievementID = achievement.Id,
                    Progress = 1,
                    LevelUpGoal = 1,
                    IsAchieved = true
                };
                context.UserAchievements
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

            context.SaveChanges();

            _pushService.SendAchievementUnlockedNotification(Tuple.Create(
                userAchievement.UserID, userAchievement.Achievement.Title, userAchievement.Achievement.CompletedMessage
            ));
        }

        public void ExecuteKobeAchievements()
        {
            GameContext context = new GameContext();
            Achievement achievement = context.Achievements
                .FirstOrDefault(a => a.Title.Equals("Kobe"));
            if (achievement == null)
            {
                return;
            }
            
            List<String> kobeLineupUserIds = context.UserLineups
                .Where(lineup => lineup.Date.Equals(_ectPrevious.Date))
                .ToList()
                .Where(lineup => IsKobeNumber(lineup.PgID) || IsKobeNumber(lineup.SgID) || IsKobeNumber(lineup.SfID)
                                 || IsKobeNumber(lineup.PfID) || IsKobeNumber(lineup.CID))
                .Select(lineup => lineup.UserID)
                .ToList();

            if (kobeLineupUserIds.Count == 0)
            {
                return;
            }
            
            foreach (var userId in kobeLineupUserIds)
            {
                User kobeWinner = context.Users.FirstOrDefault(user => user.Id.Equals(userId));
                if (kobeWinner == null)
                {
                    continue;
                }

                UserAchievement userAchievement = context.UserAchievements
                    .Include(ua => ua.Achievement)
                    .FirstOrDefault(ua => ua.UserID.Equals(userId) && ua.Achievement.Title.Equals("Kobe"));

                if (userAchievement == null)
                {

                    userAchievement = new UserAchievement
                    {
                        UserID = userId,
                        AchievementID = achievement.Id,
                        Progress = 1,
                        LevelUpGoal = 1,
                        IsAchieved = true
                    };

                    context.UserAchievements
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
            }

            context.SaveChanges();

            foreach (var userId in kobeLineupUserIds)
            {
                _pushService.SendAchievementUnlockedNotification(Tuple.Create(
                    userId, achievement.Title, achievement.CompletedMessage
                )); 
            }
        }

        private bool IsKobeNumber(int playerId)
        {
            GameContext context = new GameContext();
            Player player = context.Players.Find(playerId);
            if (player == null)
            {
                return false;
            }

            int playerNumber = int.Parse(player.Number);
            return playerNumber == 8 || playerNumber == 24;
        }

        public void ExecuteMonthWinners()
        {
            if (DateTime.DaysInMonth(_ectPrevious.Year, _ectPrevious.Month) != _ectPrevious.Day)
            {
                return;
            }

            GameContext context = new GameContext();
            Achievement achievement = context.Achievements
                .FirstOrDefault(a => a.Title.Equals("Month King"));
            if (achievement == null)
            {
                return;
            }
            
            var firstDayOfMonth = new DateTime(_ectPrevious.Year, _ectPrevious.Month, 1);
            var winnerTuple = context.UserLineups
                .Where(lineup => lineup.Date >= firstDayOfMonth.Date
                                 && lineup.Date <= _ectPrevious.Date)
                .ToList()
                .GroupBy(lineup => lineup.UserID)
                .Select(lineup => (lineup.Max(x => x.UserID), lineup.Sum(x => x.FP)))
                .OrderByDescending(lineup => lineup.Item2)
                .FirstOrDefault();
            
            User winner = context.Users.FirstOrDefault(user => user.Id.Equals(winnerTuple.Item1));

            if (winner == null)
            {
                return;
            }

            UserAchievement userAchievement = context.UserAchievements
                .Include(ua => ua.Achievement)
                .FirstOrDefault(ua => ua.UserID.Equals(winner.Id) && ua.Achievement.Title.Equals("Month King"));

            if (userAchievement == null)
            {
                userAchievement = new UserAchievement
                {
                    UserID = winner.Id,
                    AchievementID = achievement.Id,
                    Progress = 1,
                    LevelUpGoal = 1,
                    IsAchieved = true
                };
                context.UserAchievements
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

            context.SaveChanges();

            _pushService.SendAchievementUnlockedNotification(Tuple.Create(
                userAchievement.UserID, achievement.Title, achievement.CompletedMessage
            ));
        }

        public void ExecuteBalancerAchievements()
        {
            GameContext context = new GameContext();
            Achievement achievement = context.Achievements
                .FirstOrDefault(a => a.Title.Equals("Balancer"));

            if (achievement == null)
            {
                return;
            }
            
            List<string> userIds = context.UserLineups
                .Where(lineup => _ectPrevious.Date.Equals(lineup.Date))
                .ToList()
                .Where(lineup => IsPlayerPriceSixty(lineup.PgID)
                                 && IsPlayerPriceSixty(lineup.SgID)
                                 && IsPlayerPriceSixty(lineup.SfID)
                                 && IsPlayerPriceSixty(lineup.PfID)
                                 && IsPlayerPriceSixty(lineup.CID))
                .Select(lineup => lineup.UserID)
                .ToList();

            if (userIds.Count == 0)
            {
                return;
            }
            
            foreach (var userId in userIds)
            {
                User user = context.Users.Find(userId);
                if (user == null)
                {
                    continue;
                }

                UserAchievement userAchievement = context.UserAchievements
                    .Include(ua => ua.Achievement)
                    .FirstOrDefault(ua => ua.Achievement.Title.Equals("Balancer")
                                          && ua.UserID.Equals(user.Id));

                if (userAchievement == null)
                {
                    continue;
                }

                userAchievement.IsAchieved = true;
                userAchievement.Progress = userAchievement.LevelUpGoal;
            }

            context.SaveChanges();

            foreach (var userId in userIds)
            {
                _pushService.SendAchievementUnlockedNotification(Tuple.Create(
                    userId, achievement.Title, achievement.CompletedMessage
                ));
            }
        }

        private bool IsPlayerPriceSixty(int playerId)
        {
            GameContext context = new GameContext();
            Player player = context.Players.Find(playerId);
            if (player == null)
            {
                return false;
            }

            return player.Price == 60;
        }

        public void ExecuteInjuredAchievements()
        {
            GameContext context = new GameContext();
            Achievement achievement = context.Achievements
                .FirstOrDefault(a => a.Title.Equals("Injured"));

            if (achievement == null)
            {
                return;
            }
            
            var winnerLineup = context.UserLineups
                .Where(lineup => lineup.Date.Equals(_ectPrevious.Date))
                .ToList()
                .OrderByDescending(lineup => lineup.FP)
                .FirstOrDefault();

            if (winnerLineup == null)
            {
                return;
            }

            if (!(IsPlayerInjured(winnerLineup.PgID)
                || IsPlayerInjured(winnerLineup.SgID)
                || IsPlayerInjured(winnerLineup.SfID)
                || IsPlayerInjured(winnerLineup.PfID)
                || IsPlayerInjured(winnerLineup.CID)))
            {
                return;
            }
            
            User winner = context.Users.FirstOrDefault(user => user.Id.Equals(winnerLineup.UserID));

            if (winner == null)
            {
                return;
            }
            
            UserAchievement userAchievement = context.UserAchievements
                .Include(ua => ua.Achievement)
                .FirstOrDefault(ua => ua.Achievement.Title.Equals("Injured")
                                      && ua.UserID.Equals(winner.Id));

            if (userAchievement == null)
            {
                return;
            }

            userAchievement.IsAchieved = true;
            userAchievement.Progress = userAchievement.LevelUpGoal;
            
            context.SaveChanges();
            _pushService.SendAchievementUnlockedNotification(Tuple.Create(
                userAchievement.UserID, userAchievement.Achievement.Title, userAchievement.Achievement.CompletedMessage
            ));
        }

        private bool IsPlayerInjured(int playerId)
        {
            GameContext context = new GameContext();
            Player player = context.Players.Find(playerId);
            if (player == null)
            {
                return false;
            }

            Stats stats = context.Stats
                .FirstOrDefault(s => s.Date.Equals(_ectPrevious.Date)
                                         && s.PlayerID == playerId);
            if (stats == null)
            {
                return true;
            }

            Injury injury = context.Injuries.Find(player.InjuryID);
            return stats.MIN.Equals("0:00") && injury != null && !injury.Status.Equals("Active");
        }

        public void ExecuteStrategistAchievements()
        {
            GameContext context = new GameContext();
            Achievement achievement = context.Achievements
                .FirstOrDefault(a => a.Title.Equals("Strategist"));

            if (achievement == null)
            {
                return;
            }
            
            List<string> userIds = context.UserLineups
                .Where(lineup => _ectPrevious.Date.Equals(lineup.Date))
                .ToList()
                .Select(lineup => (
                    lineup.UserID,
                    new List<bool>
                    {
                        IsPlayerPriceOverHundred(lineup.PgID), IsPlayerPriceOverHundred(lineup.SgID),
                        IsPlayerPriceOverHundred(lineup.SfID), IsPlayerPriceOverHundred(lineup.PfID),
                        IsPlayerPriceOverHundred(lineup.CID)
                    }
                ))
                .Where(x => x.Item2.Count(price => price) > 1)
                .Select(lineup => lineup.UserID)
                .ToList();

            if (userIds.Count == 0)
            {
                return;
            }
            
            foreach (var userId in userIds)
            {
                User user = context.Users.Find(userId);
                if (user == null)
                {
                    continue;
                }

                UserAchievement userAchievement = context.UserAchievements
                    .Include(ua => ua.Achievement)
                    .FirstOrDefault(ua => ua.Achievement.Title.Equals("Strategist")
                                          && ua.UserID.Equals(user.Id));

                if (userAchievement == null)
                {
                    continue;
                }
                
                userAchievement.IsAchieved = true;
                userAchievement.Progress = userAchievement.LevelUpGoal;
            }
            
            context.SaveChanges();

            foreach (var userId in userIds)
            {
                _pushService.SendAchievementUnlockedNotification(Tuple.Create(
                    userId, achievement.Title, achievement.CompletedMessage
                ));
            }
        }

        private bool IsPlayerPriceOverHundred(int playerId)
        {
            GameContext context = new GameContext();
            Player player = context.Players.Find(playerId);
            if (player == null)
            {
                return false;
            }

            return player.Price >= 100;
        }

        public void ExecuteVeteranAchievements()
        {
            GameContext context = new GameContext();
            Achievement achievement = context.Achievements
                .FirstOrDefault(a => a.Title.Equals("Veteran"));

            if (achievement == null)
            {
                return;
            }

            var userLineups = context.UserLineups
                .Where(lineup => lineup.Date.Equals(_ectPrevious.Date) && lineup.IsCalculated)
                .ToList()
                .Select(lineup => (lineup.UserID, lineup.FP));

            foreach (var userLineup in userLineups)
            {
                User user = context.Users.Find(userLineup.UserID);
                if (user == null)
                {
                    continue;
                }

                UserAchievement ua = context.UserAchievements
                    .FirstOrDefault(uu => uu.UserID.Equals(user.Id)
                                          && uu.AchievementID == achievement.Id);

                if (ua == null)
                {
                    continue;
                }

                ua.Progress = Math.Round(ua.Progress + userLineup.FP, 1);

                // Level Up
                if (ua.Progress.CompareTo(ua.LevelUpGoal) >= 0)
                {
                    ua.Level++;
                    ua.LevelUpGoal *= 2;

                    _pushService.SendAchievementLevelUpNotification(Tuple.Create(
                        user.Id, ua.Achievement.Title, ua.Level
                    ));
                }
            }

            context.SaveChanges();
        }

        public void ExecuteAgentZeroAchievements()
        {
            GameContext context = new GameContext();
            Achievement achievement = context.Achievements
                .FirstOrDefault(a => a.Title.Equals("Agent Zero"));

            if (achievement == null)
            {
                return;
            }
            
            List<string> userIds = context.UserLineups
                .Where(lineup => _ectPrevious.Date.Equals(lineup.Date))
                .ToList()
                .Where(lineup => IsPlayerZero(lineup.PgID)
                                 && IsPlayerZero(lineup.SgID)
                                 && IsPlayerZero(lineup.SfID)
                                 && IsPlayerZero(lineup.PfID)
                                 && IsPlayerZero(lineup.CID))
                .Select(lineup => lineup.UserID)
                .ToList();

            if (userIds.Count == 0)
            {
                return;
            }

            foreach (var userId in userIds)
            {
                User user = context.Users.Find(userId);
                if (user == null)
                {
                    continue;
                }

                UserAchievement userAchievement = context.UserAchievements
                    .Include(ua => ua.Achievement)
                    .FirstOrDefault(ua => ua.Achievement.Title.Equals("Agent Zero")
                                          && ua.UserID.Equals(user.Id));

                if (userAchievement == null)
                {
                    continue;
                }

                userAchievement.IsAchieved = true;
                userAchievement.Progress = userAchievement.LevelUpGoal;
            }

            context.SaveChanges();

            foreach (var userId in userIds)
            {
                _pushService.SendAchievementUnlockedNotification(Tuple.Create(
                    userId, achievement.Title, achievement.CompletedMessage
                ));
            }
        }

        private bool IsPlayerZero(int playerId)
        {
            GameContext context = new GameContext();
            Player player = context.Players.Find(playerId);
            if (player == null)
            {
                return false;
            }

            return int.Parse(player.Number) == 0;
        }

        public void ExecuteContestWinnerAchievement(User contestWinner)
        {
            GameContext context = new GameContext();
            Achievement achievement = context.Achievements
                .FirstOrDefault(a => a.Title.Equals("Contester"));

            if (achievement == null)
            {
                return;
            }
            
            UserAchievement userAchievement = context.UserAchievements
                .Include(ua => ua.Achievement)
                .FirstOrDefault(ua => ua.Achievement.Title.Equals("Contester")
                                      && ua.UserID.Equals(contestWinner.Id));
            
            if (userAchievement == null)
            {
                userAchievement = new UserAchievement
                {
                    UserID = contestWinner.Id,
                    AchievementID = achievement.Id,
                    Progress = 1,
                    LevelUpGoal = 5
                };
                context.UserAchievements.Add(userAchievement);
            }
            else
            {
                userAchievement.Progress += 1;

                // Level Up
                if (userAchievement.Progress.CompareTo(userAchievement.LevelUpGoal) >= 0)
                {
                    userAchievement.Level++;
                    userAchievement.LevelUpGoal *= 2;

                    _pushService.SendAchievementLevelUpNotification(Tuple.Create(
                        contestWinner.Id, userAchievement.Achievement.Title, userAchievement.Level
                    ));
                }
            }

            context.SaveChanges();
        }

        public void ExecuteTournamentWinnerAchievement(User tournamentWinner)
        {
            GameContext context = new GameContext();
            Achievement achievement = context.Achievements
                .FirstOrDefault(a => a.Title.Equals("Master"));

            if (achievement == null)
            {
                return;
            }
            
            UserAchievement userAchievement = context.UserAchievements
                .Include(ua => ua.Achievement)
                .FirstOrDefault(ua => ua.Achievement.Title.Equals("Master")
                                      && ua.UserID.Equals(tournamentWinner.Id));
            
            if (userAchievement == null)
            {
                userAchievement = new UserAchievement
                {
                    UserID = tournamentWinner.Id,
                    AchievementID = achievement.Id,
                    Progress = 1,
                    LevelUpGoal = 1,
                    IsAchieved = true
                };
                context.UserAchievements.Add(userAchievement);
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
            _pushService.SendAchievementUnlockedNotification(Tuple.Create(
                userAchievement.UserID, userAchievement.Achievement.Title, userAchievement.Achievement.CompletedMessage
            ));

            context.SaveChanges();
        }
    }
}