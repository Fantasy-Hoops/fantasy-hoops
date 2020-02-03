using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using fantasy_hoops.Models.ViewModels;
using fantasy_hoops.Repositories;
using fantasy_hoops.Repositories.Interfaces;
using fantasy_hoops.Services.Interfaces;
using FluentScheduler;
using Newtonsoft.Json;
using WebPush;
using PushSubscription = fantasy_hoops.Models.PushSubscription;

namespace fantasy_hoops.Services
{
    public class PushService : IPushService
    {
        private readonly WebPushClient _client;
        public static VapidDetails _vapidDetails;
        private readonly IPushNotificationRepository _pushNotificationRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILineupRepository _lineupRepository;

        public PushService(IPushNotificationRepository pushNotificationRepository, IUserRepository userRepository, ILineupRepository lineupRepository)
        {
            _pushNotificationRepository = pushNotificationRepository;
            _userRepository = userRepository;
            _lineupRepository = lineupRepository;
            _client = new WebPushClient();
            var vapidSubject = Environment.GetEnvironmentVariable("VapidSubject");
            var vapidPublicKey = Environment.GetEnvironmentVariable("VapidPublicKey");
            var vapidPrivateKey = Environment.GetEnvironmentVariable("VapidPrivateKey");
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

        public async Task Send(string userId, PushNotificationViewModel notification)
        {
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
                        string.Format("Game is starting in less than 5 hours! Don't forget to set up your lineup!"))
                    {
                        Actions = new List<NotificationAction> { new NotificationAction("lineup", "🏆 Lineup") }
                    };
            var userNotSelected = _lineupRepository.UsersNotSelected();
            foreach (var user in userNotSelected)
                Send(user.Id, notification).Wait();
        }
    }
}
