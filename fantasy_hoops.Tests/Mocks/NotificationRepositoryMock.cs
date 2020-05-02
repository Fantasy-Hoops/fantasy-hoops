using System.Collections.Generic;
using fantasy_hoops.Dtos;
using fantasy_hoops.Models.Notifications;
using fantasy_hoops.Models.Tournaments;
using fantasy_hoops.Models.ViewModels;
using fantasy_hoops.Repositories.Interfaces;

namespace fantasy_hoops.Tests.Mocks
{
    public class NotificationRepositoryMock : INotificationRepository
    {
        public List<NotificationDto> GetAllNotifications()
        {
            throw new System.NotImplementedException();
        }

        public List<NotificationDto> GetNotifications(string userID, int start, int count)
        {
            throw new System.NotImplementedException();
        }

        public void AddTournamentRequestNotification(Tournament tournament, string receiverId, string senderId)
        {
            return;
        }

        public bool AddRequestNotification(RequestNotification.Type requestType, string receiverId, string senderId, string message,
            string tournamentId = null)
        {
            return true;
        }

        public void RemoveFriendRequestNotification(string userId, string friendId)
        {
            throw new System.NotImplementedException();
        }

        public void ReadNotification(NotificationViewModel model)
        {
            throw new System.NotImplementedException();
        }

        public void ReadAllNotifications(string userID)
        {
            throw new System.NotImplementedException();
        }
    }
}