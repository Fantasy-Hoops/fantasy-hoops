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
            GameContext context = new GameContext();
            return context.Notifications.OfType<GameScoreNotification>()
                .Select(notification => new NotificationDto
                {
                    NotificationID = notification.NotificationID,
                    ReceiverID = notification.ReceiverID,
                    DateCreated = notification.DateCreated,
                    ReadStatus = notification.ReadStatus,
                    Score = notification.Score
                }).ToList()
                .Union(context.Notifications
                    .OfType<InjuryNotification>()
                    .Include(notification => notification.Player)
                    .ThenInclude(player => player.Team)
                    .Select(notification => new NotificationDto
                    {
                        NotificationID = notification.NotificationID,
                        ReceiverID = notification.ReceiverID,
                        DateCreated = notification.DateCreated,
                        ReadStatus = notification.ReadStatus,
                        PlayerID = notification.PlayerID,
                        NbaId = notification.Player.NbaID,
                        AbbrName = notification.Player.AbbrName,
                        Position = notification.Player.Position,
                        TeamColor = notification.Player.Team.Color,
                        InjuryStatus = notification.InjuryStatus,
                        InjuryDescription = notification.InjuryDescription
                    })).ToList()
                .Union(context.Notifications
                    .OfType<RequestNotification>()
                    .Include(notification => notification.Sender)
                    .Select(notification => new NotificationDto
                    {
                        NotificationID = notification.NotificationID,
                        ReceiverID = notification.ReceiverID,
                        DateCreated = notification.DateCreated,
                        ReadStatus = notification.ReadStatus,
                        SenderID = notification.SenderID,
                        RequestMessage = notification.RequestMessage,
                        FriendUsername = notification.Sender.UserName,
                        FriendAvatarUrl = notification.Sender.AvatarURL,
                        TournamentId = notification.TournamentId
                    })).ToList();
        }

        public List<NotificationDto> GetNotifications(string userID, int start, int count)
        {
            GameContext context = new GameContext();
            if (count == 0)
                count = context.Notifications.Count(y => y.ReceiverID.Equals(userID));
            return context.Notifications.OfType<GameScoreNotification>()
                .Select(notification => new NotificationDto
                {
                    NotificationID = notification.NotificationID,
                    ReceiverID = notification.ReceiverID,
                    DateCreated = notification.DateCreated,
                    ReadStatus = notification.ReadStatus,
                    Score = notification.Score
                }).ToList()
                .Union(context.Notifications
                    .OfType<InjuryNotification>()
                    .Include(notification => notification.Player)
                    .ThenInclude(player => player.Team)
                    .Select(notification => new NotificationDto
                    {
                        NotificationID = notification.NotificationID,
                        ReceiverID = notification.ReceiverID,
                        DateCreated = notification.DateCreated,
                        ReadStatus = notification.ReadStatus,
                        PlayerID = notification.PlayerID,
                        NbaId = notification.Player.NbaID,
                        AbbrName = notification.Player.AbbrName,
                        Position = notification.Player.Position,
                        TeamColor = notification.Player.Team.Color,
                        InjuryStatus = notification.InjuryStatus,
                        InjuryDescription = notification.InjuryDescription
                    })).ToList()
                .Union(context.Notifications
                    .OfType<RequestNotification>()
                    .Include(notification => notification.Sender)
                    .Select(notification => new NotificationDto
                    {
                        NotificationID = notification.NotificationID,
                        ReceiverID = notification.ReceiverID,
                        DateCreated = notification.DateCreated,
                        ReadStatus = notification.ReadStatus,
                        SenderID = notification.SenderID,
                        RequestMessage = notification.RequestMessage,
                        FriendUsername = notification.Sender.UserName,
                        FriendAvatarUrl = notification.Sender.AvatarURL,
                        TournamentId = notification.TournamentId
                    })).ToList()
                .Where(y => y.ReceiverID.Equals(userID))
                .OrderByDescending(y => y.DateCreated)
                .Skip(start)
                .Take(count)
                .ToList();
        }

        public void AddTournamentRequestNotification(Tournament tournament, string receiverId, string senderId)
        {
            string senderUsername = new GameContext().Users.Find(senderId).UserName;
            string message = $"User {senderUsername} invited you to join the tournament: {tournament.Title}";
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

            GameContext context = new GameContext();
            context.RequestNotifications.Add(notification);
            return context.SaveChanges() > 0;
        }

        public void RemoveFriendRequestNotification(string userID, string friendID)
        {
            GameContext context = new GameContext();
            var notifications = context.RequestNotifications
                .Where(x => x.ReceiverID.Equals(userID) && x.SenderID.Equals(friendID)).ToList();

            if (notifications != null)
                context.RequestNotifications.RemoveRange(notifications);

            context.SaveChanges();
        }

        public void ReadNotification(NotificationViewModel model)
        {
            GameContext context = new GameContext();
            Notification notification = context.Notifications
                .FirstOrDefault(x => x.NotificationID == model.NotificationID && x.ReceiverID.Equals(model.ReceiverID));

            if (notification == null)
            {
                return;
            }

            notification.ReadStatus = true;
            context.SaveChanges();
        }

        public void ReadAllNotifications(string userID)
        {
            GameContext context = new GameContext();
            context.Notifications
                .Where(x => x.ReceiverID.Equals(userID) && x.ReadStatus == false)
                .ToList()
                .ForEach(n => n.ReadStatus = true);

            context.SaveChanges();
        }
    }
}