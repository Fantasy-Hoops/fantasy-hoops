using fantasy_hoops.Database;
using fantasy_hoops.Models;
using fantasy_hoops.Models.Notifications;
using fantasy_hoops.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using fantasy_hoops.Dtos;

namespace fantasy_hoops.Repositories
{
    public class NotificationRepository : INotificationRepository
    {

        private readonly GameContext _context;
        public NotificationRepository()
        {
            _context = new GameContext();
        }

        public List<NotificationDto> GetAllNotifications()
        {
            return _context.Notifications.OfType<GameScoreNotification>()
                .Select(notification => new NotificationDto
                {
                    NotificationID = notification.NotificationID,
                    UserID = notification.UserID,
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
                        UserID = notification.UserID,
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
                    .OfType<FriendRequestNotification>()
                    .Include(notification => notification.Friend)
                    .Select(notification => new NotificationDto
                    {
                        NotificationID = notification.NotificationID,
                        UserID = notification.UserID,
                        DateCreated = notification.DateCreated,
                        ReadStatus = notification.ReadStatus,
                        FriendID = notification.FriendID,
                        RequestMessage = notification.RequestMessage,
                        FriendUsername = notification.Friend.UserName,
                        FriendAvatarUrl = notification.Friend.AvatarURL
                    }))
                .ToList();
        }

        public List<NotificationDto> GetNotifications(string userID, int start, int count)
        {
            if (count == 0)
                count = _context.Notifications.Count(y => y.UserID.Equals(userID));
            return _context.Notifications.OfType<GameScoreNotification>()
                .Select(notification => new NotificationDto
                {
                    NotificationID = notification.NotificationID,
                    UserID = notification.UserID,
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
                        UserID = notification.UserID,
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
                    .OfType<FriendRequestNotification>()
                    .Include(notification => notification.Friend)
                    .Select(notification => new NotificationDto
                    {
                        NotificationID = notification.NotificationID,
                        UserID = notification.UserID,
                        DateCreated = notification.DateCreated,
                        ReadStatus = notification.ReadStatus,
                        FriendID = notification.FriendID,
                        RequestMessage = notification.RequestMessage,
                        FriendUsername = notification.Friend.UserName,
                        FriendAvatarUrl = notification.Friend.AvatarURL
                    }))
                    .Where(y => y.UserID.Equals(userID))
                    .OrderByDescending(y => y.DateCreated)
                    .Skip(start)
                    .Take(count)
                    .ToList();
        }

        public void AddFriendRequestNotification(string userID, string friendID, string message)
        {
            var notification = new FriendRequestNotification
            {
                UserID = userID,
                FriendID = friendID,
                ReadStatus = false,
                DateCreated = DateTime.UtcNow,
                RequestMessage = message
            };

            _context.FriendRequestNotifications.Add(notification);
            _context.SaveChanges();
        }

        public void RemoveFriendRequestNotification(string userID, string friendID)
        {
            var notifications = _context.FriendRequestNotifications
                .Where(x => x.UserID.Equals(userID) && x.FriendID.Equals(friendID)).ToList();

            if (notifications != null)
                _context.FriendRequestNotifications.RemoveRange(notifications);

            _context.SaveChanges();
        }

        public void ReadNotification(NotificationViewModel model)
        {
            Notification notification = _context.Notifications
                .FirstOrDefault(x => x.NotificationID == model.NotificationID && x.UserID.Equals(model.UserID));
            
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
                .Where(x => x.UserID.Equals(userID) && x.ReadStatus == false)
                .ToList()
                .ForEach(n => n.ReadStatus = true);

            _context.SaveChanges();
        }

    }
}
