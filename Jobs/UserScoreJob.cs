using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fantasy_hoops.Database;
using fantasy_hoops.Helpers;
using fantasy_hoops.Models;
using fantasy_hoops.Models.ViewModels;
using fantasy_hoops.Services.Interfaces;
using FluentScheduler;
using Microsoft.EntityFrameworkCore;

namespace fantasy_hoops.Jobs
{
    public class UserScoreJob : IJob
    {
        private static readonly Stack<GameScorePushNotificationModel> _usersPlayed =
            new Stack<GameScorePushNotificationModel>();

        private readonly GameContext _context;
        private readonly IPushService _pushService;

        public UserScoreJob(IPushService pushService)
        {
            _context = new GameContext();
            _pushService = pushService;
        }

        private async Task SendPushNotifications()
        {
            while (_usersPlayed.Count > 0)
            {
                var user = _usersPlayed.Pop();
                PushNotificationViewModel notification =
                    new PushNotificationViewModel("Fantasy Hoops Game Score",
                        $"Game has finished! Your lineup scored {user.Score} FP")
                    {
                        Actions = new List<NotificationAction> {new NotificationAction("leaderboard", "🏆 Leaderboard")}
                    };
                await _pushService.Send(user.UserID, notification);
            }
        }

        public void Execute()
        {
            var todayStats = _context.Stats
                .Where(stats => stats.Date.Equals(CommonFunctions.UTCToEastern(NextGameJob.PREVIOUS_GAME).Date))
                .Select(stats => new {stats.PlayerID, stats.FP});

            var allLineups = _context.UserLineups
                .Include(lineup => lineup.User)
                .Where(lineup => lineup.Date.Equals(CommonFunctions.UTCToEastern(NextGameJob.PREVIOUS_GAME).Date) &&
                                 !lineup.IsCalculated)
                .Include(lineup => lineup.User)
                .ToList();

            if (allLineups.Count == 0)
                return;

            _context.Users
                .AsEnumerable()
                .Except(allLineups.Select(lineup => lineup.User).AsEnumerable())
                .ToList()
                .ForEach(user => user.Streak = 0);

            foreach (var lineup in allLineups)
            {
                var selectedPlayers = new List<int> {lineup.PgID, lineup.SgID, lineup.SfID, lineup.PfID, lineup.CID};
                lineup.FP =
                    Math.Round(todayStats
                        .Where(stats => selectedPlayers.Contains(stats.PlayerID))
                        .Select(stats => stats.FP)
                        .Sum(), 1);
                lineup.IsCalculated = true;

                lineup.User.Streak++;

                _usersPlayed.Push(new GameScorePushNotificationModel
                {
                    UserID = lineup.User.Id,
                    Score = lineup.FP
                });

                var gs = new GameScoreNotification
                {
                    UserID = lineup.User.Id,
                    ReadStatus = false,
                    DateCreated = DateTime.UtcNow,
                    Score = lineup.FP
                };
                _context.GameScoreNotifications.Add(gs);
            }

            _context.SaveChanges();
            SendPushNotifications().Wait();

            Task.Run(async () => await new BestLineupsJob(_pushService).Execute());
        }
    }
}