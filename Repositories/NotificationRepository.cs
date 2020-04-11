using fantasy_hoops.Database;
using fantasy_hoops.Models;
using fantasy_hoops.Models.Notifications;
using fantasy_hoops.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using fantasy_hoops.Dtos;
using fantasy_hoops.Models.Tournaments;
using fantasy_hoops.Repositories.Interfaces;
using fantasy_hoops.Services;
using fantasy_hoops.Services.Interfaces;

namespace fantasy_hoops.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly GameContext _context;
        private readonly IPushService _pushService;

        public NotificationRepository()
        {
            _context = new GameContext();
            _pushService = new PushService();
        }

        public List<NotificationDto> GetAllNotifications()
        {
            return _context.Notifications.OfType<GameScoreNotification>()
                .Select(notification => new NotificationDto
                {
                    NotificationID = notification.NotificationID,
                    UserID = notification.ReceiverID,
                    DateCreated = notification.DateCreated,
                    ReadStatus = notification.ReadStatus,
                    Score = notification.Score
                })
                .AsEnumerable()
                .Union(_context.Notifications
                    .OfType<InjuryNotification>()
                    .Include(notification => notification.Player)
                    .ThenInclude(player => player.Team)
                    .Select(notification => new NotificationDto
                    {
                        NotificationID = notification.NotificationID,
                        UserID = notification.ReceiverID,
                        DateCreated = notification.DateCreated,
                        ReadStatus = notification.ReadStatus,
                        PlayerID = notification.PlayerID,
                        NbaId = notification.Player.NbaID,
                        AbbrName = notification.Player.AbbrName,
                        Position = notification.Player.Position,
                        TeamColor = notification.Player.Team.Color,
                        InjuryStatus = notification.InjuryStatus,
                        InjuryDescription = notification.InjuryDescription
                    }))
                .AsEnumerable()
                .Union(_context.Notifications
                    .OfType<RequestNotification>()
                    .Include(notification => notification.Sender)
                    .Select(notification => new NotificationDto
                    {
                        NotificationID = notification.NotificationID,
                        UserID = notification.ReceiverID,
                        DateCreated = notification.DateCreated,
                        ReadStatus = notification.ReadStatus,
                        FriendID = notification.SenderID,
                        RequestMessage = notification.RequestMessage,
                        FriendUsername = notification.Sender.UserName,
                        FriendAvatarUrl = notification.Sender.AvatarURL
                    }))
                .ToList();
        }

        public List<NotificationDto> GetNotifications(string userID, int start, int count)
        {
            if (count == 0)
                count = _context.Notifications.Count(y => y.ReceiverID.Equals(userID));
            return _context.Notifications.OfType<GameScoreNotification>()
                .Select(notification => new NotificationDto
                {
                    NotificationID = notification.NotificationID,
                    UserID = notification.ReceiverID,
                    DateCreated = notification.DateCreated,
                    ReadStatus = notification.ReadStatus,
                    Score = notification.Score
                })
                .AsEnumerable()
                .Union(_context.Notifications
                    .OfType<InjuryNotification>()
                    .Include(notification => notification.Player)
                    .ThenInclude(player => player.Team)
                    .Select(notification => new NotificationDto
                    {
                        NotificationID = notification.NotificationID,
                        UserID = notification.ReceiverID,
                        DateCreated = notification.DateCreated,
                        ReadStatus = notification.ReadStatus,
                        PlayerID = notification.PlayerID,
                        NbaId = notification.Player.NbaID,
                        AbbrName = notification.Player.AbbrName,
                        Position = notification.Player.Position,
                        TeamColor = notification.Player.Team.Color,
                        InjuryStatus = notification.InjuryStatus,
                        InjuryDescription = notification.InjuryDescription
                    }))
                .AsEnumerable()
                .Union(_context.Notifications
                    .OfType<RequestNotification>()
                    .Include(notification => notification.Sender)
                    .Select(notification => new NotificationDto
                    {
                        NotificationID = notification.NotificationID,
                        UserID = notification.ReceiverID,
                        DateCreated = notification.DateCreated,
                        ReadStatus = notification.ReadStatus,
                        FriendID = notification.SenderID,
                        RequestMessage = notification.RequestMessage,
                        FriendUsername = notification.Sender.UserName,
                        FriendAvatarUrl = notification.Sender.AvatarURL
                    }))
                .Where(y => y.UserID.Equals(userID))
                .OrderByDescending(y => y.DateCreated)
                .Skip(start)
                .Take(count)
                .ToList();
        }

        public void AddTournamentRequestNotification(Tournament tournament, string receiverId, string senderId)
        {
            string senderUsername = _context.Users.Find(senderId).UserName;
            string message = $"User {senderUsername} invited you to join the tournament: {tournament.Name}";
            if (AddRequestNotification(RequestNotification.Type.TOURNAMENT, receiverId, senderId, message, tournament.Id))
            {
                PushNotificationViewModel pushNotification =
                    new PushNotificationViewModel("Fantasy Hoops Tournament Invitation", message);
                _pushService.Send(receiverId, pushNotification);
            }
        }

        public bool AddRequestNotification(RequestNotification.Type requestType, string receiverId, string senderId,
            string message, string tournamentId)
        {
            var notification = new RequestNotification
            {
                RequestType = requestType,
                ReceiverID = receiverId,
                SenderID = senderId,
                ReadStatus = false,
                DateCreated = DateTime.UtcNow,
                RequestMessage = message,
                TournamentId = tournamentId
            };

            _context.RequestNotifications.Add(notification);
            return _context.SaveChanges() > 0;
        }

        public void RemoveFriendRequestNotification(string userID, string friendID)
        {
            var notifications = _context.RequestNotifications
                .Where(x => x.ReceiverID.Equals(userID) && x.SenderID.Equals(friendID)).ToList();

            if (notifications != null)
                _context.RequestNotifications.RemoveRange(notifications);

            _context.SaveChanges();
        }

        public void ReadNotification(NotificationViewModel model)
        {
            Notification notification = _context.Notifications
                .FirstOrDefault(x => x.NotificationID == model.NotificationID && x.ReceiverID.Equals(model.UserID));

            if (notification == null)
            {
                return;
            }

            notification.ReadStatus = true;
            _context.SaveChanges();
        }

        public void ReadAllNotifications(string userID)
        {
            _context.Notifications
                .Where(x => x.ReceiverID.Equals(userID) && x.ReadStatus == false)
                .ToList()
                .ForEach(n => n.ReadStatus = true);

            _context.SaveChanges();
        }
    }
}