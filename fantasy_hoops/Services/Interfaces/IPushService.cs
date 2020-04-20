using System;
using System.Threading.Tasks;
using fantasy_hoops.Models;
using fantasy_hoops.Models.PushNotifications;
using fantasy_hoops.Models.ViewModels;

namespace fantasy_hoops.Services.Interfaces
{
    public interface IPushService
    {
        void CheckOrGenerateVapidDetails(string subject, string vapidPublicKey, string vapidPrivateKey);
        string GetVapidPublicKey();
        Task<PushSubscription> Subscribe(PushSubscription subscription);
        Task Unsubscribe(PushSubscription subscription);
        Task SendToAllUsers(PushNotificationViewModel notification);
        /// <param name="text">text of the notification</param>
        Task Send(string userId, string text);
        Task Send(string userId, PushNotificationViewModel notification);
        Task SendAdminNotification(PushNotificationViewModel notification);
        void SendNudgeNotifications();
        void SendAchievementLevelUpNotification(Tuple<string, string, int> notificationData);
        void SendAchievementUnlockedNotification(Tuple<string, string, string> notificationData);
    }
}
