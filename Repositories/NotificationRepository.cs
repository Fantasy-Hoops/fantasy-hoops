using fantasy_hoops.Database;
using fantasy_hoops.Models;
using fantasy_hoops.Models.Notifications;
using fantasy_hoops.Models.Notifications.ViewModels;
using fantasy_hoops.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace fantasy_hoops.Repositories
{
    public class NotificationRepository : INotificationRepository
    {

        private readonly GameContext _context;
        public NotificationRepository()
        {
            _context = new GameContext();
        }

        public IEnumerable<Notification> GetAllNotifications()
        {
            return _context.Notifications.OfType<GameScoreNotification>()
                    .Select(n => new GameScoreNotification
                    {
                        NotificationID = n.NotificationID,
                        UserID = n.UserID,
                        DateCreated = n.DateCreated,
                        ReadStatus = n.ReadStatus,
                        Score = n.Score
                    })
                    .AsEnumerable()
                    .OfType<Notification>()
                    .Union(_context.Notifications
                        .OfType<FriendRequestNotification>()
                        .Select(n => new FriendRequestNotificationViewModel
                        {
                            NotificationID = n.NotificationID,
                            UserID = n.UserID,
                            DateCreated = n.DateCreated,
                            ReadStatus = n.ReadStatus,
                            FriendID = n.FriendID,
                            RequestMessage = n.RequestMessage,
                            FriendUserName = n.Friend.UserName,
                            FriendAvatarURL = n.Friend.AvatarURL
                        }))
                    .Union(_context.InjuryNotifications
                        .OfType<InjuryNotification>()
                        .Select(n => new InjuryNotificationViewModel
                        {
                            NotificationID = n.NotificationID,
                            UserID = n.UserID,
                            DateCreated = n.DateCreated,
                            ReadStatus = n.ReadStatus,
                            InjuryStatus = n.InjuryStatus,
                            InjuryDescription = n.InjuryDescription,
                            PlayerID = n.PlayerID,
                            AbbrName = n.Player.AbbrName,
                            NbaID = n.Player.NbaID,
                            Position = n.Player.Position,
                            TeamColor = n.Player.Team.Color
                        }))
                    .OrderByDescending(n => n.DateCreated)
                    .ToList();
        }

        public IEnumerable<Notification> GetNotifications(string userID, int start, int count)
        {
            if (count == 0)
                count = _context.Notifications.Where(y => y.UserID.Equals(userID)).Count();
            return _context.Notifications.OfType<GameScoreNotification>()
                    .Select(n => new GameScoreNotification
                    {
                        NotificationID = n.NotificationID,
                        UserID = n.UserID,
                        DateCreated = n.DateCreated,
                        ReadStatus = n.ReadStatus,
                        Score = n.Score
                    })
                    .AsEnumerable()
                    .OfType<Notification>()
                    .Union(_context.Notifications
                        .OfType<FriendRequestNotification>()
                        .Select(n => new FriendRequestNotificationViewModel
                        {
                            NotificationID = n.NotificationID,
                            UserID = n.UserID,
                            DateCreated = n.DateCreated,
                            ReadStatus = n.ReadStatus,
                            FriendID = n.FriendID,
                            RequestMessage = n.RequestMessage,
                            FriendUserName = n.Friend.UserName,
                            FriendAvatarURL = n.Friend.AvatarURL
                        }))
                    .Union(_context.InjuryNotifications
                        .OfType<InjuryNotification>()
                        .Select(n => new InjuryNotificationViewModel
                        {
                            NotificationID = n.NotificationID,
                            UserID = n.UserID,
                            DateCreated = n.DateCreated,
                            ReadStatus = n.ReadStatus,
                            InjuryStatus = n.InjuryStatus,
                            InjuryDescription = n.InjuryDescription,
                            PlayerID = n.PlayerID,
                            AbbrName = n.Player.AbbrName,
                            NbaID = n.Player.NbaID,
                            Position = n.Player.Position,
                            TeamColor = n.Player.Team.Color
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
            _context.Notifications
                .Where(x => x.NotificationID == model.NotificationID
                        && x.UserID.Equals(model.UserID))
                .FirstOrDefault()
                .ReadStatus = true;

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
