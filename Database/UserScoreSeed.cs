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
            Task.Run(() => Update(context)).Wait();
        }

        private static async Task Update(GameContext context)
        {
            WebPushClient _webPushClient = new WebPushClient();
            var todayStats = context.Stats.Where(stats => stats.Date.Date == CommonFunctions.UTCToEastern(NextGame.PREVIOUS_GAME).Date);
            var allLineups = context.UserLineups.Where(x => x.Date.Date == CommonFunctions.UTCToEastern(NextGame.PREVIOUS_GAME).Date && !x.IsCalculated)
                .ToList();

            if (allLineups.Count == 0)
                return;

            foreach (var lineup in allLineups)
            {
                lineup.FP =
                    Math.Round(todayStats
                    .Where(stats => stats.PlayerID == lineup.PgID
                        || stats.PlayerID == lineup.SgID
                        || stats.PlayerID == lineup.SfID
                        || stats.PlayerID == lineup.PfID
                        || stats.PlayerID == lineup.CID)
                    .Select(stats => stats.FP)
                    .Sum(), 1);
                lineup.IsCalculated = true;
            }

            var usersPlayed = context.UserLineups
                .Where(x => x.Date.Date == CommonFunctions.UTCToEastern(NextGame.PREVIOUS_GAME).Date)
                .Select(x => x.User)
                .Distinct();

            await context.Users.Except(usersPlayed).ForEachAsync(u => u.Streak = 0);

            foreach (var user in usersPlayed)
            {
                user.Streak++;
                var userScore = Math.Round(allLineups
                    .Where(x => x.Date.Date == CommonFunctions.UTCToEastern(NextGame.PREVIOUS_GAME).Date
                            && x.UserID.Equals(user.Id))
                    .Select(x => x.FP)
                    .FirstOrDefault(), 1);

                _usersPlayed.Push(new GameScorePushNotificationModel
                {
                    UserID = user.Id,
                    Score = userScore
                });

                var gs = new GameScoreNotification
                {
                    UserID = user.Id,
                    ReadStatus = false,
                    DateCreated = DateTime.UtcNow,
                    Score = userScore
                };
                await context.GameScoreNotifications.AddAsync(gs);
            }
            await context.SaveChangesAsync();
            await SendPushNotifications(context);
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
