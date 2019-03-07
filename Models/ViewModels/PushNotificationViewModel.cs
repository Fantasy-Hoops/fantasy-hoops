using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace fantasy_hoops.Models.ViewModels
{
    /// <summary>
    ///     <see href="https://notifications.spec.whatwg.org/#dictdef-notificationoptions">Notification API Standard</see>
    /// </summary>
    public class PushNotificationViewModel
    {
        public PushNotificationViewModel() { }

        public PushNotificationViewModel(string text)
        {
            Body = text;
        }

        public PushNotificationViewModel(string title, string body)
        {
            Title = title;
            Body = body;
        }

        [JsonProperty("title")]
        public string Title { get; set; } = "Push Demo";

        [JsonProperty("lang")]
        public string Lang { get; set; } = "en";

        [JsonProperty("body")]
        public string Body { get; set; }

        [JsonProperty("tag")]
        public string Tag { get; set; }

        [JsonProperty("image")]
        public string Image { get; set; }

        [JsonProperty("icon")]
        public string Icon { get; set; }

        [JsonProperty("badge")]
        public string Badge { get; set; }

        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; set; } = DateTime.Now;

        [JsonProperty("requireInteraction")]
        public bool RequireInteraction { get; set; } = false;

        [JsonProperty("actions")]
        public List<NotificationAction> Actions { get; set; } = new List<NotificationAction>();

        [JsonProperty("data")]
        public object Data { get; set; }
    }

    /// <summary>
    ///     <see href="https://notifications.spec.whatwg.org/#dictdef-notificationaction">Notification API Standard</see>
    /// </summary>
    public class NotificationAction
    {

        [JsonProperty("action")]
        public string Action { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }
    }

    public class NotificationTag
    {
        public const string Notify = "demo_testmessage";
        public const string Trivia = "demo_trivia";
    }
}
