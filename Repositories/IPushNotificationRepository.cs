using System;
using System.Collections.Generic;
using fantasy_hoops.Models;
using fantasy_hoops.Models.ViewModels;

namespace fantasy_hoops.Repositories
{
    interface IPushNotificationRepository
    {
        IEnumerable<PushSubscription> GetAllSubscribtions();

        IEnumerable<PushSubscription> GetUserSubscribtions(string userId);
    }
}
