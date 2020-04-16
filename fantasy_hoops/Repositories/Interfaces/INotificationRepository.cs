using System.Collections.Generic;
using fantasy_hoops.Dtos;
using fantasy_hoops.Models.Notifications;
using fantasy_hoops.Models.Tournaments;
using fantasy_hoops.Models.ViewModels;

namespace fantasy_hoops.Repositories.Interfaces
{
    public interface INotificationRepository
    {

        List<NotificationDto> GetAllNotifications();
        List<NotificationDto> GetNotifications(string userID, int start, int count);
        void AddTournamentRequestNotification(Tournament tournament, string receiverId, string senderId);
        bool AddRequestNotification(RequestNotification.Type requestType, string receiverId, string senderId, string message, string tournamentId = null);
        void RemoveFriendRequestNotification(string userId, string friendId);
        void ReadNotification(NotificationViewModel model);
        void ReadAllNotifications(string userID);

    }
}
