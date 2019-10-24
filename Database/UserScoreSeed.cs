using fantasy_hoops.Helpers;
using fantasy_hoops.Models;
using fantasy_hoops.Models.ViewModels;
using fantasy_hoops.Services;
using FluentScheduler;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebPush;

namespace fantasy_hoops.Database
{
    public class UserScoreSeed : IJob
    {
        private readonly GameContext _context;
        private static readonly Stack<GameScorePushNotificationModel> _usersPlayed = new Stack<GameScorePushNotificationModel>();

        public UserScoreSeed(GameContext context)
        {
            _context = context;
        }

        private async Task SendPushNotifications()
        {
            while (_usersPlayed.Count > 0)
            {
                var user = _usersPlayed.Pop();
                PushNotificationViewModel notification =
                        new PushNotificationViewModel("Fantasy Hoops Game Score",
                                string.Format("Game has finished! Your lineup scored {0} FP", user.Score))
                        {
                            Actions = new List<NotificationAction> { new NotificationAction("leaderboard", "🏆 Leaderboard") }
                        };
                await PushService.Instance.Value.Send(user.UserID, notification);
            }
        }

        public void Execute()
        {
            WebPushClient _webPushClient = new WebPushClient();

            var todayStats = _context.Stats
                    .Where(stats => stats.Date.Equals(CommonFunctions.UTCToEastern(NextGame.PREVIOUS_GAME).Date))
                    .Select(stats => new { stats.PlayerID, stats.FP });

            var allLineups = _context.UserLineups
                    .Where(lineup => lineup.Date.Equals(CommonFunctions.UTCToEastern(NextGame.PREVIOUS_GAME).Date) && !lineup.IsCalculated)
                    .Include(lineup => lineup.User)
                    .ToList();

            if (allLineups.Count == 0)
                return;

            _context.Users
                    .Except(allLineups.Select(lineup => lineup.User))
                    .ToList()
                    .ForEach(user => user.Streak = 0);

            foreach (var lineup in allLineups)
            {
                var selectedPlayers = new List<int> { lineup.PgID, lineup.SgID, lineup.SfID, lineup.PfID, lineup.CID };
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
            Task.Run(() => SendPushNotifications());
        }
    }
}
