using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using fantasy_hoops.Enums;

namespace fantasy_hoops.Models
{
    public class FriendRequest
    {
        [Key]
        public int ID { get; set; }
        public DateTime Date { get; set; }
        public String SenderID { get; set; }
        public String ReceiverID { get; set; }
        public RequestStatus Status { get; set; }

        [ForeignKey("SenderID")]
        public virtual User Sender { get; set; }
        [ForeignKey("ReceiverID")]
        public virtual User Receiver { get; set; }
    }
}
