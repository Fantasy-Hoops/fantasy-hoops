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
        private static List<GameScorePushNotificationModel> _usersPlayed = new List<GameScorePushNotificationModel>();

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
            var allPlayers = context.Lineups.Where(x => x.Date == CommonFunctions.UTCToEastern(NextGame.PREVIOUS_GAME) && !x.Calculated)
                .Include(x => x.Player).ThenInclude(x => x.Stats)
                .ToList();

            if (allPlayers.Count == 0)
                return;

            foreach (var player in allPlayers)
            {
                player.FP = player.Player.Stats
                    .Where(s => s.Date >= CommonFunctions.UTCToEastern(NextGame.PREVIOUS_GAME) && s.Date <= CommonFunctions.UTCToEastern(NextGame.PREVIOUS_LAST_GAME))
                    .Select(x => x.FP).FirstOrDefault();
                player.Calculated = true;
            }

            var usersPlayed = context.Lineups
                .Where(x => x.Date == CommonFunctions.UTCToEastern(NextGame.PREVIOUS_GAME))
                .Select(x => x.User)
                .Distinct();

            await context.Users.Except(usersPlayed).ForEachAsync(u => u.Streak = 0);

            foreach (var user in usersPlayed)
            {
                user.Streak++;
                var userScore = Math.Round(allPlayers
                    .Where(x => x.Date == CommonFunctions.UTCToEastern(NextGame.PREVIOUS_GAME)
                            && x.UserID.Equals(user.Id))
                    .Select(x => x.FP).Sum(), 1);

                _usersPlayed.Add(new GameScorePushNotificationModel
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
            SendPushNotifications(context);
        }

        private static void SendPushNotifications(GameContext context)
        {
            WebPushClient _webPushClient = new WebPushClient();
            _usersPlayed.ForEach(user =>
            {
                PushNotificationViewModel notification =
                    new PushNotificationViewModel("FantasyHoops Game Score",
                        string.Format("Game has finished! Your lineup scored {0} FP", user.Score));
                notification.Actions = new List<NotificationAction> { new NotificationAction("leaderboard", "🏆 Leaderboard") };
                foreach (var subscription in context.PushSubscriptions.Where(sub => sub.UserID.Equals(user.UserID)))
                {
                    try
                    {
                        _webPushClient.SendNotification(subscription.ToWebPushSubscription(), JsonConvert.SerializeObject(notification), PushService._vapidDetails);
                    }
                    catch (WebPushException e)
                    {
                        if (e.Message == "Subscription no longer valid")
                        {
                            context.PushSubscriptions.Remove(subscription);
                            context.SaveChanges();
                        }
                        else
                        {
                            // Track exception with eg. AppInsights
                        }
                    }
                }
            });
            _usersPlayed.Clear();
        }
    }
}
