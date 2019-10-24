using System;
using System.Collections.Generic;
using fantasy_hoops.Models;
using fantasy_hoops.Models.ViewModels;

namespace fantasy_hoops.Repositories
{
    public interface IPushNotificationRepository
    {
        IEnumerable<PushSubscription> GetAllSubscriptions();
        IEnumerable<PushSubscription> GetUserSubscriptions(string userId);
        bool SubscriptionExists(PushSubscription subscription);
        PushSubscription GetByP256Dh(string p256dh);
        void AddSubscription(PushSubscription subscription);
        void RemoveSubscription(PushSubscription subscription);
    }
}
