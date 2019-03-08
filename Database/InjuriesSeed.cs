﻿using System.Net;
using System.Linq;
using System.Threading.Tasks;
using System;
using Newtonsoft.Json.Linq;
using fantasy_hoops.Models;
using Microsoft.EntityFrameworkCore;
using fantasy_hoops.Helpers;
using fantasy_hoops.Models.Notifications;
using FluentScheduler;
using System.Threading;
using Newtonsoft.Json;
using fantasy_hoops.Services;
using WebPush;
using fantasy_hoops.Models.ViewModels;
using System.Collections.Generic;

namespace fantasy_hoops.Database
{
    public class InjuriesSeed
    {
        const int DAYS_TO_SAVE = 2;
        static DateTime dayFrom = DateTime.UtcNow.AddDays(-DAYS_TO_SAVE);
        private static List<InjuryPushNotificationViewModel> lineupsAffected = new List<InjuryPushNotificationViewModel>();

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

            Task.Run(() => Extract(context)).Wait();
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
                try
                {
                    if (dayFrom.CompareTo(DateTime.Parse(injury["CreatedDate"].ToString()).AddHours(5)) > 0)
                        break;
                    await AddToDatabaseAsync(context, injury);
                }
                catch (Exception)
                {
                    continue;
                }
            }
            await context.SaveChangesAsync();
            SendPushNotifications(context);
        }

        private static async Task AddToDatabaseAsync(GameContext context, JToken injury)
        {
            var injuryObj = new Injuries
            {
                Title = (string)injury["Title"],
                Status = (string)injury["PlayerStatus"],
                Injury = (string)injury["Injury"],
                Description = (string)injury["News"],
                Date = DateTime.Parse(injury["CreatedDate"].ToString()).AddHours(5),
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
                .StatusDate = DateTime.Parse(injury["CreatedDate"].ToString()).AddHours(5);

            if (!statusBefore.Equals(statusAfter))
                await UpdateNotifications(context, injuryObj, statusBefore, statusAfter);
        }

        private static async Task UpdateNotifications(GameContext context, Injuries injury, string statusBefore, string statusAfter)
        {
            await context.Lineups
                .Where(x => x.Date.Equals(CommonFunctions.UTCToEastern(NextGame.NEXT_GAME))
                            && x.PlayerID == injury.PlayerID)
                .ForEachAsync(async s =>
                {
                    lineupsAffected.Add(new InjuryPushNotificationViewModel
                    {
                        UserID = s.UserID,
                        StatusBefore = statusBefore,
                        StatusAfter = statusAfter,
                        AbbrName = s.Player.AbbrName,
                        PlayerNbaID = s.Player.NbaID
                    });
                    var inj = new InjuryNotification
                    {
                        UserID = s.UserID,
                        ReadStatus = false,
                        DateCreated = DateTime.UtcNow,
                        PlayerID = s.PlayerID,
                        InjuryStatus = injury.Status,
                        InjuryDescription = injury.Injury
                    };

                    if (!await context.InjuryNotifications
                    .AnyAsync(x => x.InjuryStatus.Equals(inj.InjuryStatus)
                                && x.PlayerID == inj.PlayerID))
                        await context.InjuryNotifications.AddAsync(inj);
                });
        }

        private static void SendPushNotifications(GameContext context)
        {
            WebPushClient _webPushClient = new WebPushClient();
            lineupsAffected.ForEach(lineup =>
            {
                PushNotificationViewModel notification =
                    new PushNotificationViewModel("FantasyHoops Injury",
                        string.Format("{0} status changed from {1} to {2}!", lineup.AbbrName, lineup.StatusBefore, lineup.StatusAfter));
                notification.Image = Environment.GetEnvironmentVariable("REACT_APP_IMAGES_SERVER_NAME") + "/content/images/players/" + lineup.PlayerNbaID + ".png";
                notification.Actions = new List<NotificationAction> { new NotificationAction("lineup", "🤾🏾‍♂️ Lineup") };
                foreach (var subscription in context.PushSubscriptions.Where(sub => sub.UserID.Equals(lineup.UserID)))
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
        }
    }
}