using System.ComponentModel.DataAnnotations.Schema;

namespace fantasy_hoops.Models.Notifications
{
    public class RequestNotification : Notification
    {
        public Type RequestType { get; set; }
        public string SenderID { get; set; }
        public string RequestMessage { get; set; }

        [ForeignKey("FriendID")]
        public virtual User Sender { get; set; }
        public string TournamentId { get; set; }
        
        public enum Type { FRIEND, TOURNAMENT }
    }
}
