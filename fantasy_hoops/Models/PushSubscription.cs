using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace fantasy_hoops.Models
{
    public class PushSubscription
    {
        public PushSubscription() { }

        public PushSubscription(string userId, WebPush.PushSubscription subscription)
        {
            UserID = userId;
            Endpoint = subscription.Endpoint;
            ExpirationTime = null;
            P256Dh = subscription.P256DH;
            Auth = subscription.Auth;
        }
        public string UserID { get; set; }

        [ForeignKey("UserID")]
        public virtual User User { get; set; }

        [Required]
        public string Endpoint { get; set; }

        public double? ExpirationTime { get; set; }

        [Required]
        [Key]
        public string P256Dh { get; set; }

        [Required]
        public string Auth { get; set; }

        public WebPush.PushSubscription ToWebPushSubscription()
        {
            return new WebPush.PushSubscription(Endpoint, P256Dh, Auth);
        }
    }
}
