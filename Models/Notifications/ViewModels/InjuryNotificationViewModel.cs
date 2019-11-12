using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fantasy_hoops.Models.Notifications.ViewModels
{
    public class InjuryNotificationViewModel : Notification
    {
        public int PlayerID { get; set; }
        public string InjuryDescription { get; set; }
        public string InjuryStatus { get; set; }
        public string AbbrName { get; set; }
        public int NbaID { get; set; }
        public string Position { get; set; }
        public string TeamColor { get; set; }
    }
}
