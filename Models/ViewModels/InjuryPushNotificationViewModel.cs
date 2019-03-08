using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fantasy_hoops.Models.ViewModels
{
    public class InjuryPushNotificationViewModel
    {
        public string UserID { get; set; }
        public string StatusBefore { get; set; }
        public string StatusAfter { get; set; }
        public string AbbrName { get; set; }
        public int PlayerNbaID { get; set; }
    }
}
