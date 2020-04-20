using Newtonsoft.Json;

namespace fantasy_hoops.Models.PushNotifications
{
    public class NotificationAction
    {
        public NotificationAction(string action, string title)
        {
            Action = action;
            Title = title;
        }

        [JsonProperty("action")]
        public string Action { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }
    }
}