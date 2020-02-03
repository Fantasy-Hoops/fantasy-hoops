using System.Collections.Generic;
using fantasy_hoops.Dtos;
using fantasy_hoops.Models.ViewModels;

namespace fantasy_hoops.Repositories.Interfaces
{
    public interface INotificationRepository
    {

        List<NotificationDto> GetAllNotifications();
        List<NotificationDto> GetNotifications(string userID, int start, int count);
        void AddFriendRequestNotification(string userID, string friendID, string message);
        void RemoveFriendRequestNotification(string userID, string friendID);
        void ReadNotification(NotificationViewModel model);
        void ReadAllNotifications(string userID);

    }
}
