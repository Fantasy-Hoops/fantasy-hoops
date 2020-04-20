using fantasy_hoops.Controllers;
using fantasy_hoops.Models.PushNotifications;

namespace fantasy_hoops.Models.ViewModels
{
    public class PushSubscriptionViewModel
    {
        public Subscription Subscription { get; set; }
        public string DeviceId { get; set; }
    }
}