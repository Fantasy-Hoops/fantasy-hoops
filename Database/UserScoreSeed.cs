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
    public class UserScoreSeed
    {
        private static Stack<GameScorePushNotificationModel> _usersPlayed = new Stack<GameScorePushNotificationModel>();

        public static void Initialize(GameContext context)
        {
            if (JobManager.RunningSchedules.Any(s => !s.Name.Equals("userScore")))
            {
                JobManager.AddJob(() => Initialize(context),
                s => s.WithName("userScore")
                .ToRunOnceIn(30)
                .Seconds());
                return;
            }
            Update(context);
        }

        private static void Update(GameContext context)
        {
            WebPushClient _webPushClient = new WebPushClient();

            var todayStats = context.Stats
                    .Where(stats => stats.Date.Equals(CommonFunctions.UTCToEastern(NextGame.PREVIOUS_GAME).Date))
                    .Select(stats => new { stats.PlayerID, stats.FP });

            var allLineups = context.UserLineups
                    .Where(lineup => lineup.Date.Equals(CommonFunctions.UTCToEastern(NextGame.PREVIOUS_GAME).Date) && !lineup.IsCalculated)
                    .Include(lineup => lineup.User)
                    .ToList();

            if (allLineups.Count == 0)
                return;

            context.Users
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
                context.GameScoreNotifications.Add(gs);
            }
            context.SaveChanges();
            Task.Run(() => SendPushNotifications(context));
        }

        private static async Task SendPushNotifications(GameContext context)
        {
            WebPushClient _webPushClient = new WebPushClient();
            while (_usersPlayed.Count > 0)
            {
                var user = _usersPlayed.Pop();
                PushNotificationViewModel notification =
                        new PushNotificationViewModel("Fantasy Hoops Game Score",
                                string.Format("Game has finished! Your lineup scored {0} FP", user.Score));
                notification.Actions = new List<NotificationAction> { new NotificationAction("leaderboard", "🏆 Leaderboard") };
                await PushService.Instance.Value.Send(user.UserID, notification);
            }
        }
    }
}
