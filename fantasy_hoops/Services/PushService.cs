using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using fantasy_hoops.Models.PushNotifications;
using fantasy_hoops.Models.ViewModels;
using fantasy_hoops.Repositories;
using fantasy_hoops.Repositories.Interfaces;
using fantasy_hoops.Services.Interfaces;
using Newtonsoft.Json;
using WebPush;
using PushSubscription = fantasy_hoops.Models.PushNotifications.PushSubscription;

namespace fantasy_hoops.Services
{
    public class PushService : IPushService
    {
        private readonly WebPushClient _client;
        private readonly VapidDetails _vapidDetails;
        private readonly IPushNotificationRepository _pushNotificationRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILineupRepository _lineupRepository;

        public PushService()
        {
            var vapidSubject = Startup.Configuration["Vapid:Subject"];
            var vapidPublicKey = Startup.Configuration["Vapid:PublicKey"];
            var vapidPrivateKey = Startup.Configuration["Vapid:PrivateKey"];
            CheckOrGenerateVapidDetails(vapidSubject, vapidPublicKey, vapidPrivateKey);
            _vapidDetails = new VapidDetails(vapidSubject, vapidPublicKey, vapidPrivateKey);
            _pushNotificationRepository = new PushNotificationRepository();
            _userRepository = new UserRepository();
            _lineupRepository = new LineupRepository();
        }

        public PushService(IPushNotificationRepository pushNotificationRepository, IUserRepository userRepository, ILineupRepository lineupRepository)
        {
            _pushNotificationRepository = pushNotificationRepository;
            _userRepository = userRepository;
            _lineupRepository = lineupRepository;
            _client = new WebPushClient();
            var vapidSubject = Startup.Configuration["Vapid:Subject"];
            var vapidPublicKey = Startup.Configuration["Vapid:PublicKey"];
            var vapidPrivateKey = Startup.Configuration["Vapid:PrivateKey"];
            CheckOrGenerateVapidDetails(vapidSubject, vapidPublicKey, vapidPrivateKey);
            _vapidDetails = new VapidDetails(vapidSubject, vapidPublicKey, vapidPrivateKey);
        }

        public PushService(IPushNotificationRepository repository, string vapidSubject, string vapidPublicKey, string vapidPrivateKey)
        {
            _pushNotificationRepository = repository;
            _client = new WebPushClient();
            CheckOrGenerateVapidDetails(vapidSubject, vapidPublicKey, vapidPrivateKey);
            _vapidDetails = new VapidDetails(vapidSubject, vapidPublicKey, vapidPrivateKey);
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
            if (_pushNotificationRepository.SubscriptionExists(subscription))
                return _pushNotificationRepository.GetByP256Dh(subscription.P256Dh);

            _pushNotificationRepository.AddSubscription(subscription);

            return subscription;
        }

        public async Task Unsubscribe(PushSubscription subscription)
        {
            if (!_pushNotificationRepository.SubscriptionExists(subscription)) return;

            _pushNotificationRepository.RemoveSubscription(subscription);
        }

        public async Task SendToAllUsers(PushNotificationViewModel notification)
        {
            foreach (var user in _userRepository.GetAllUsers())
                await Send(user.UserId, notification);
        }

        public async Task Send(string userId, PushNotificationViewModel notification)
        {
            if (!Boolean.Parse(Startup.Configuration["PushNotificationsEnabled"]))
            {
                return;
            }
            
            foreach (var subscription in GetUserSubscriptions(userId))
            {
                try
                {
                    _client.SendNotification(subscription.ToWebPushSubscription(), JsonConvert.SerializeObject(notification), _vapidDetails);
                }
                catch (WebPushException e)
                {
                    if (e.Message == "Subscription no longer valid")
                    {
                        _pushNotificationRepository.RemoveSubscription(subscription);
                    }
                }
            }
        }

        public async Task Send(string userId, string text)
        {
            await Send(userId, new PushNotificationViewModel(text));
        }

        private IEnumerable<PushSubscription> GetUserSubscriptions(string userId) =>
            _pushNotificationRepository.GetUserSubscriptions(userId);

        public async Task SendAdminNotification(PushNotificationViewModel notification)
        {
            string adminRoleID = _userRepository.GetAdminRoleId();
            foreach (var admin in _userRepository.GetAdmins(adminRoleID))
                await Send(admin.UserId, notification);
        }

        public void SendNudgeNotifications()
        {
            PushNotificationViewModel notification =
                    new PushNotificationViewModel("Fantasy Hoops Reminder",
                        "Game is starting in less than 5 hours! Don't forget to set up your lineup!")
                    {
                        Actions = new List<NotificationAction> { new NotificationAction("lineup", "🏆 Lineup") }
                    };
            var userNotSelected = _lineupRepository.UsersNotSelected();
            foreach (var user in userNotSelected)
                Send(user.Id, notification).Wait();
        }

        public void SendAchievementLevelUpNotification(Tuple<string, string, int> notificationData)
        {
            PushNotificationViewModel notification =
                new PushNotificationViewModel($"'{notificationData.Item2}' Achievement Level Up",
                    $"You have reached level {notificationData.Item3}!");

            Send(notificationData.Item1, notification).Wait();
        }

        public void SendAchievementUnlockedNotification(Tuple<string, string, string> notificationData)
        {
            PushNotificationViewModel notification =
                new PushNotificationViewModel($"Achievement '{notificationData.Item2}' Unlocked",
                    notificationData.Item3);

            Send(notificationData.Item1, notification).Wait();
        }
    }
}
