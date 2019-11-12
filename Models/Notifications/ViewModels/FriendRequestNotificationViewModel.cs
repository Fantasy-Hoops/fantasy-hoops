using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fantasy_hoops.Models.Notifications.ViewModels
{
    public class FriendRequestNotificationViewModel : Notification
    {
        public string FriendID { get; set; }
        public string RequestMessage { get; set; }
        public string FriendUserName { get; set; }
        public string FriendAvatarURL { get; set; }
    }
}
