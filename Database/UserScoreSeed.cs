using fantasy_hoops.Helpers;
using fantasy_hoops.Models;
using fantasy_hoops.Models.ViewModels;
using fantasy_hoops.Services;
using FluentScheduler;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebPush;

namespace fantasy_hoops.Database
{
    public class UserScoreSeed
    {
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

            await usersPlayed
                .ForEachAsync(async user =>
                {
                    user.Streak++;

                    var userScore = Math.Round(allPlayers
                        .Where(x => x.Date == CommonFunctions.UTCToEastern(NextGame.PREVIOUS_GAME)
                                && x.UserID.Equals(user.Id))
                        .Select(x => x.FP).Sum(), 1);

                    var gs = new GameScoreNotification
                    {
                        UserID = user.Id,
                        ReadStatus = false,
                        DateCreated = DateTime.UtcNow,
                        Score = userScore
                    };

                    await context.GameScoreNotifications.AddAsync(gs);
                    PushNotificationViewModel notification =
                    new PushNotificationViewModel("FantasyHoops Game Score",
                        string.Format("Game has finished! Your lineup scored {0} FP", gs.Score));

                    foreach (var subscription in await context.PushSubscriptions.Where(sub => sub.UserID.Equals(user.Id)).ToListAsync())
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
                                await context.SaveChangesAsync();
                            }
                            else
                            {
                                // Track exception with eg. AppInsights
                            }
                        }
                    }
                });
            await context.SaveChangesAsync();
        }
    }
}
