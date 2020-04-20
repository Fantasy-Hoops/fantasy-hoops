namespace fantasy_hoops.Models.PushNotifications
{
    public class Subscription
    {
        public string Endpoint { get; set; }
        public double? ExpirationTime { get; set; }
        public Keys Keys { get; set; }

        public WebPush.PushSubscription ToWebPushSubscription() =>
            new WebPush.PushSubscription(Endpoint, Keys.P256Dh, Keys.Auth);
    }
}