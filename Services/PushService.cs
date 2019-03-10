using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using fantasy_hoops.Database;
using fantasy_hoops.Helpers;
using fantasy_hoops.Models;
using fantasy_hoops.Models.ViewModels;
using FluentScheduler;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using WebPush;
using PushSubscription = fantasy_hoops.Models.PushSubscription;

namespace fantasy_hoops.Services
{
    public class PushService : IPushService
    {
        private static readonly Lazy<PushService> INSTANCE = new Lazy<PushService>();

        private readonly WebPushClient _client;
        private readonly GameContext _context;
        public static VapidDetails _vapidDetails;

        public PushService()
        {
            _context = new GameContext();
            _client = new WebPushClient();
            _context = new GameContext();
            _client = new WebPushClient();
            var vapidSubject = Environment.GetEnvironmentVariable("VapidSubject");
            var vapidPublicKey = Environment.GetEnvironmentVariable("VapidPublicKey");
            var vapidPrivateKey = Environment.GetEnvironmentVariable("VapidPrivateKey");
            CheckOrGenerateVapidDetails(vapidSubject, vapidPublicKey, vapidPrivateKey);
            _vapidDetails = new VapidDetails(vapidSubject, vapidPublicKey, vapidPrivateKey);
        }

        public PushService(string vapidSubject, string vapidPublicKey, string vapidPrivateKey)
        {
            _context = new GameContext();
            _client = new WebPushClient();
            CheckOrGenerateVapidDetails(vapidSubject, vapidPublicKey, vapidPrivateKey);
            _vapidDetails = new VapidDetails(vapidSubject, vapidPublicKey, vapidPrivateKey);
        }

        public PushService(IConfiguration configuration)
        {
            _context = new GameContext();
            _client = new WebPushClient();
            var vapidSubject = Environment.GetEnvironmentVariable("VapidSubject");
            var vapidPublicKey = Environment.GetEnvironmentVariable("VapidPublicKey");
            var vapidPrivateKey = Environment.GetEnvironmentVariable("VapidPrivateKey");
            CheckOrGenerateVapidDetails(vapidSubject, vapidPublicKey, vapidPrivateKey);
            _vapidDetails = new VapidDetails(vapidSubject, vapidPublicKey, vapidPrivateKey);
        }

        public static Lazy<PushService> Instance
        {
            get { return INSTANCE; }
        }

        public void CheckOrGenerateVapidDetails(string vapidSubject, string vapidPublicKey, string vapidPrivateKey)
        {
            if (string.IsNullOrEmpty(vapidSubject) ||
                string.IsNullOrEmpty(vapidPublicKey) ||
                string.IsNullOrEmpty(vapidPrivateKey))
            {
                var vapidKeys = VapidHelper.GenerateVapidKeys();

                // Prints 2 URL Safe Base64 Encoded Strings
                Debug.WriteLine($"Public {vapidKeys.PublicKey}");
                Debug.WriteLine($"Private {vapidKeys.PrivateKey}");

                throw new Exception(
                    "You must set the Vapid:Subject, Vapid:PublicKey and Vapid:PrivateKey application settings or pass them to the service in the constructor. You can use the ones just printed to the debug console.");
            }
        }

        public string GetVapidPublicKey() => _vapidDetails.PublicKey;

        public async Task<PushSubscription> Subscribe(PushSubscription subscription)
        {
            if (await _context.PushSubscriptions.AnyAsync(s => s.P256Dh == subscription.P256Dh))
                return await _context.PushSubscriptions.FindAsync(subscription.P256Dh);

            await _context.PushSubscriptions.AddAsync(subscription);
            await _context.SaveChangesAsync();

            return subscription;
        }

        public async Task Unsubscribe(PushSubscription subscription)
        {
            if (!await _context.PushSubscriptions.AnyAsync(s => s.P256Dh == subscription.P256Dh)) return;

            _context.PushSubscriptions.Remove(subscription);
            await _context.SaveChangesAsync();
        }

        public async Task Send(string userId, PushNotificationViewModel notification)
        {
            foreach (var subscription in await GetUserSubscriptions(userId))
            {
                try
                {
                    _client.SendNotification(subscription.ToWebPushSubscription(), JsonConvert.SerializeObject(notification), _vapidDetails);
                }
                catch (WebPushException e)
                {
                    if (e.Message == "Subscription no longer valid")
                    {
                        _context.PushSubscriptions.Remove(subscription);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        // Track exception with eg. AppInsights
                    }
                }
            }
        }

        public async Task Send(string userId, string text)
        {
            await Send(userId, new PushNotificationViewModel(text));
        }

        private async Task<List<PushSubscription>> GetUserSubscriptions(string userId) =>
            await _context.PushSubscriptions.Where(s => s.UserID.Equals(userId)).ToListAsync();

        public async Task SendAdminNotification(PushNotificationViewModel notification)
        {
            string adminRoleID = _context.Roles.Where(role => role.Name.Equals("Admin")).FirstOrDefault().Id;
            foreach (var admin in await _context.UserRoles.Where(userRole => userRole.RoleId.Equals(adminRoleID)).ToListAsync())
                await Send(admin.UserId, notification);
        }

        public async Task SendNudgeNotifications()
        {
            if (JobManager.RunningSchedules.Any(s => !s.Name.Equals("nudgeNotifications")))
            {
                JobManager.AddJob(() => SendNudgeNotifications().Wait(),
                s => s.WithName("nudgeNotifications")
                .ToRunOnceIn(30)
                .Seconds());
                return;
            }

            PushNotificationViewModel notification =
                    new PushNotificationViewModel("Fantasy Hoops Reminder",
                        string.Format("Game is starting in less than 2 hours! Don't forget to set up your lineup!"));
            notification.Actions = new List<NotificationAction> { new NotificationAction("lineup", "🏆 Lineup") };
            var usersSelectedIDs = _context.Lineups
                .Where(lineup => lineup.Date.Equals(CommonFunctions.UTCToEastern(NextGame.NEXT_GAME)))
                .GroupBy(u => u.UserID)
                .Select(result => result.FirstOrDefault().UserID);
            foreach (var user in await _context.Users
                .Where(user => user.Streak > 0 && !usersSelectedIDs.Any(userID => userID.Equals(user.Id)))
                .ToListAsync())
                await Send(user.Id, notification);
        }
    }
}
