﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace fantasy_hoops.Models
{
    public class Notification
    {
        [Key]
        public int NotificationID { get; set; }
        public string ReceiverID { get; set; }
        public bool ReadStatus { get; set; }
        public DateTime DateCreated { get; set; }

        [ForeignKey("ReceiverID")]
        public virtual User Receiver { get; set; }
    }
}
