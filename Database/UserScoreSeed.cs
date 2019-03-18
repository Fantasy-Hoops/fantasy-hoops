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

        //private static async Task Migrate(GameContext context)
        //{
        //    var oldLineups = context.Lineups
        //        .GroupBy(l => new { l.UserID, l.Date })
        //        .Select(res => new
        //        {
        //            res.First().UserID,
        //            res.First().Date,
        //            score = Math.Round(res.Sum(c => c.FP), 1),
        //            calculated = res.First().Calculated,
        //            lineup = res.Select(l => new
        //            {
        //                l.PlayerID,
        //                l.Player.NbaID,
        //                l.Player.Position,
        //                teamColor = l.Player.Team.Color,
        //                l.Player.FullName,
        //                l.Player.FirstName,
        //                l.Player.LastName,
        //                l.Player.AbbrName,
        //                l.FP
        //            }).OrderBy(p => Array.IndexOf(CommonFunctions.PlayersOrder, p.Position)).ToList()
        //        });

        //    foreach (var lineup in oldLineups)
        //    {
        //        var userLineup = new UserLineup
        //        {
        //            Date = lineup.Date,
        //            UserID = lineup.UserID,
        //            FP = lineup.score,
        //            IsCalculated = lineup.calculated,
        //            PgID = lineup.lineup[0].PlayerID,
        //            SgID = lineup.lineup[1].PlayerID,
        //            SfID = lineup.lineup[2].PlayerID,
        //            PfID = lineup.lineup[3].PlayerID,
        //            CID = lineup.lineup[4].PlayerID
        //        };

        //        await context.UserLineups.AddAsync(userLineup);
        //    }
        //    await context.SaveChangesAsync();
        //}

        private static async Task Update(GameContext context)
        {
            WebPushClient _webPushClient = new WebPushClient();
            var todayStats = context.Stats.Where(stats => stats.Date >= CommonFunctions.UTCToEastern(NextGame.PREVIOUS_GAME));
            var allLineups = context.UserLineups.Where(x => x.Date == CommonFunctions.UTCToEastern(NextGame.PREVIOUS_GAME) && !x.IsCalculated)
                .ToList();

            if (allLineups.Count == 0)
                return;

            foreach (var lineup in allLineups)
            {
                lineup.FP =
                    todayStats
                    .Where(stats => stats.PlayerID == lineup.PgID
                        || stats.PlayerID == lineup.SgID
                        || stats.PlayerID == lineup.SfID
                        || stats.PlayerID == lineup.PfID
                        || stats.PlayerID == lineup.CID)
                    .Select(stats => stats.FP)
                    .Sum();
                lineup.IsCalculated = true;
            }

            var usersPlayed = context.UserLineups
                .Where(x => x.Date == CommonFunctions.UTCToEastern(NextGame.PREVIOUS_GAME))
                .Select(x => x.User)
                .Distinct();

            await context.Users.Except(usersPlayed).ForEachAsync(u => u.Streak = 0);

            foreach (var user in usersPlayed)
            {
                user.Streak++;
                var userScore = Math.Round(allLineups
                    .Where(x => x.Date == CommonFunctions.UTCToEastern(NextGame.PREVIOUS_GAME)
                            && x.UserID.Equals(user.Id))
                    .Select(x => x.FP)
                    .FirstOrDefault());

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
