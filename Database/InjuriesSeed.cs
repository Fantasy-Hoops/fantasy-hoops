using System.Net;
using System.Linq;
using System.Threading.Tasks;
using System;
using Newtonsoft.Json.Linq;
using fantasy_hoops.Models;
using Microsoft.EntityFrameworkCore;
using fantasy_hoops.Helpers;
using fantasy_hoops.Models.Notifications;
using FluentScheduler;
using fantasy_hoops.Services;
using WebPush;
using fantasy_hoops.Models.ViewModels;
using System.Collections.Generic;
using System.Diagnostics;

namespace fantasy_hoops.Database
{
    public class InjuriesSeed
    {
        const int DAYS_TO_SAVE = 5;
        static DateTime dayFrom = DateTime.UtcNow.AddDays(-DAYS_TO_SAVE);
        private static Stack<InjuryPushNotificationViewModel> lineupsAffected = new Stack<InjuryPushNotificationViewModel>();

        public static void Initialize(GameContext context)
        {
            if (JobManager.RunningSchedules.Any(s => !s.Name.Equals("injuries")))
            {
                JobManager.AddJob(() => Initialize(context),
                s => s.WithName("injuries")
                .ToRunOnceIn(30)
                .Seconds());
                return;
            }

            try
            {
                Task.Run(() => Extract(context)).Wait();
            }
            catch (AggregateException e)
            {
                Debug.WriteLine("Injuries seed exception: " + e.Message);
            }
        }

        private static JArray GetInjuries()
        {
            HttpWebResponse webResponse = CommonFunctions.GetResponse("https://www.fantasylabs.com/api/players/news/2/");
            string myResponse = CommonFunctions.ResponseToString(webResponse);
            JArray injuries = JArray.Parse(myResponse);
            return injuries;
        }

        private static async Task Extract(GameContext context)
        {
            await context.Injuries.ForEachAsync(inj => context.Injuries.Remove(inj));
            JArray injuries = GetInjuries();
            foreach (JObject injury in injuries)
            {
                DateTime dateUTC = CommonFunctions.EasternToUTC(DateTime.Parse(injury["ModifiedDate"].ToString()));
                try
                {
                    if (dayFrom.CompareTo(dateUTC) > 0)
                        break;
                    await AddToDatabaseAsync(context, injury, dateUTC);
                }
                catch (Exception)
                {
                    continue;
                }
            }
            await context.SaveChangesAsync();
            await SendPushNotifications(context);
        }

        private static async Task AddToDatabaseAsync(GameContext context, JToken injury, DateTime dateUTC)
        {
            var injuryObj = new Injuries
            {
                Title = (string)injury["Title"],
                Status = (string)injury["PlayerStatus"],
                Injury = (string)injury["Injury"],
                Description = (string)injury["News"],
                Date = dateUTC,
                Link = (string)injury["Link"]
            };
            injuryObj.Player = context.Players.Where(x => x.NbaID == (int)injury["PrimarySourceKey"]).FirstOrDefault();

            if (injuryObj.Player == null)
                return;
            await context.Injuries.AddAsync(injuryObj);
            string statusBefore = context.Players
                .Where(p => p.NbaID == injuryObj.Player.NbaID)
                .FirstOrDefault()
                .Status;
            string statusAfter = injuryObj.Status;
            context.Players
                .Where(p => p.NbaID == injuryObj.Player.NbaID)
                .FirstOrDefault()
                .Status = injuryObj.Status;
            context.Players
                .Where(p => p.NbaID == injuryObj.Player.NbaID)
                .FirstOrDefault()
                .StatusDate = dateUTC;

            if (!statusBefore.Equals(statusAfter))
                await UpdateNotifications(context, injuryObj, statusBefore, statusAfter);
        }

        private static async Task UpdateNotifications(GameContext context, Injuries injury, string statusBefore, string statusAfter)
        {
            foreach (var lineup in context.Lineups
                .Where(x => x.Date.Equals(CommonFunctions.UTCToEastern(NextGame.NEXT_GAME))
                            && x.PlayerID == injury.PlayerID))
            {
                lineupsAffected.Push(new InjuryPushNotificationViewModel
                {
                    UserID = lineup.UserID,
                    StatusBefore = statusBefore,
                    StatusAfter = statusAfter,
                    AbbrName = lineup.Player.AbbrName,
                    PlayerNbaID = lineup.Player.NbaID
                });
                var inj = new InjuryNotification
                {
                    UserID = lineup.UserID,
                    ReadStatus = false,
                    DateCreated = DateTime.UtcNow,
                    PlayerID = lineup.PlayerID,
                    InjuryStatus = injury.Status,
                    InjuryDescription = injury.Injury
                };

                if (!context.InjuryNotifications
                .Any(x => x.InjuryStatus.Equals(inj.InjuryStatus)
                                && x.PlayerID == inj.PlayerID))
                    await context.InjuryNotifications.AddAsync(inj);
            }
        }

        private static async Task SendPushNotifications(GameContext context)
        {
            WebPushClient _webPushClient = new WebPushClient();
            while (lineupsAffected.Count > 0)
            {
                var lineup = lineupsAffected.Pop();
                PushNotificationViewModel notification =
                    new PushNotificationViewModel("Fantasy Hoops Injury",
                        string.Format("{0} status changed from {1} to {2}!", lineup.AbbrName, lineup.StatusBefore, lineup.StatusAfter));
                notification.Image = Environment.GetEnvironmentVariable("IMAGES_SERVER_NAME") + "/content/images/players/" + lineup.PlayerNbaID + ".png";
                notification.Actions = new List<NotificationAction> { new NotificationAction("lineup", "🤾🏾‍♂️ Lineup") };
                await PushService.Instance.Value.Send(lineup.UserID, notification);
            }
        }
    }
}