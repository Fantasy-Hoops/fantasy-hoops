using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using fantasy_hoops.Database;
using fantasy_hoops.Dtos;
using fantasy_hoops.Helpers;
using fantasy_hoops.Models;
using fantasy_hoops.Models.Enums;
using fantasy_hoops.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace fantasy_hoops.Repositories
{
    public class UserRepository : IUserRepository
    {
        private const string DEFAULT_TEAM_ABV = "SEA";
        private const string DEFAULT_COLOR = "#2C3E50";

        private readonly DateTime _date = CommonFunctions.Instance.GetLeaderboardDate(LeaderboardType.WEEKLY);
        private readonly GameContext _context;

        public UserRepository(GameContext context = null)
        {
            _context = context ?? new GameContext();
        }

        public object GetProfile(string id, int start, int count)
        {
            int position = GetPosition(id);
            return new GameContext().Users
                .Where(user => user.Id.Equals(id))
                .Include(user => user.FavoriteTeam)
                .Include(user => user.UserLineups)
                .Select(user => new
                {
                    user.Id,
                    user.UserName,
                    user.Email,
                    user.Description,
                    user.FavoriteTeamId,
                    Team = new
                    {
                        Name = user.FavoriteTeam.City + " " + user.FavoriteTeam.Name,
                        user.FavoriteTeam.Color
                    },
                    user.Streak,
                    Position = position,
                    weeklyScore = Math.Round(user.UserLineups.Where(lineup => lineup.Date >= _date)
                        .Select(lineup => lineup.FP)
                        .Sum(), 1),
                    userRecord = Math.Round(user.UserLineups.Select(lineup => lineup.FP).Max(), 1),
                    user.AvatarURL
                })
                .First();
        }

        public User GetUserById(string id)
        {
            return new GameContext().Users.Find(id);
        }

        public User GetUserByName(string username)
        {
            return new GameContext().Users
                .FirstOrDefault(x => x.UserName.ToLower().Equals(username.ToLower()));
        }

        public User GetUserByEmail(string email)
        {
            return new GameContext().Users
                .FirstOrDefault(user => user.Email.ToLower().Equals(email.ToLower()));
        }

        public IQueryable<object> Roles(string id)
        {
            GameContext context = new GameContext();
            return context.UserRoles
                .Where(userRole => userRole.UserId.Equals(id))
                .Join(context.Roles,
                    userRole => userRole.RoleId,
                    role => role.Id,
                    (userRole, role) => role.NormalizedName);
        }

        public bool IsAdmin(string userId)
        {
            GameContext context = new GameContext();
            string adminRoleId = context.Roles
                .Where(role => role.NormalizedName.Equals("Admin"))
                .Select(role => role.Id)
                .FirstOrDefault();

            return context.UserRoles
                .Where(userRole => userRole.UserId.Equals(userId))
                .Any(role => role.RoleId.Equals(adminRoleId));
        }

        public IQueryable<Object> GetFriends(string id)
        {
            GameContext context = new GameContext();
            var friends = context.FriendRequests
                .Where(x => x.ReceiverID.Equals(id) && x.Status.Equals(RequestStatus.ACCEPTED))
                .Select(x => new
                {
                    id = x.SenderID,
                    x.Sender.UserName,
                    Color = context.Teams
                        .Where(t => t.TeamID == x.Sender.FavoriteTeamId)
                        .Select(t => t.Color)
                        .FirstOrDefault(),
                    x.Sender.AvatarURL
                })
                .Union(context.FriendRequests
                    .Where(x => x.SenderID.Equals(id) && x.Status.Equals(RequestStatus.ACCEPTED))
                    .Select(x => new
                    {
                        id = x.ReceiverID,
                        x.Receiver.UserName,
                        Color = context.Teams
                            .Where(t => t.TeamID == x.Receiver.FavoriteTeamId)
                            .Select(t => t.Color)
                            .FirstOrDefault(),
                        x.Receiver.AvatarURL
                    }));

            return friends;
        }

        public IQueryable<Object> GetTeam(string id)
        {
            GameContext context = new GameContext();
            return context.Users
                .Where(usr => usr.Id.Equals(id))
                .Select(x => new
                {
                    team = context.Teams.FirstOrDefault(team => team.TeamID == x.FavoriteTeamId)
                });
        }

        public IQueryable<Object> GetUserPool()
        {
            GameContext context = new GameContext();
            return context.Users
                .Include(user => user.FavoriteTeam)
                .Select(user => new
                {
                    user.UserName,
                    user.Id,
                    Color = user.FavoriteTeam.Abbreviation.Equals(DEFAULT_TEAM_ABV)
                        ? DEFAULT_COLOR
                        : user.FavoriteTeam.Color,
                    user.AvatarURL,
                    roles = context.UserRoles
                        .Where(userRole => userRole.UserId.Equals(user.Id))
                        .Select(userRole => context.Roles.FirstOrDefault(role => role.Id.Equals(userRole.RoleId)).Name)
                        .ToList()
                })
                .OrderBy(user => user.UserName);
        }

        public List<UserDto> GetAllUsers()
        {
            return new GameContext().Users
                .Select(user => new UserDto
                {
                    UserId = user.Id,
                    Username = user.UserName,
                    AvatarUrl = user.AvatarURL
                }).ToList();
        }


        public bool UserExists(string username)
        {
            return new GameContext().Users
                .Any(x => x.UserName.ToLower().Equals(username.ToLower()));
        }

        public bool EmailExists(string email)
        {
            return new GameContext().Users
                .Any(x => x.Email.ToLower().Equals(email.ToLower()));
        }

        public bool IsDuplicateUserName(string id, string username)
        {
            return new GameContext().Users.Any(x => x.UserName.ToLower().Equals(username.ToLower()) && !x.Id.Equals(id));
        }

        public bool IsDuplicateEmail(string id, string email)
        {
            return new GameContext().Users.Any(x => x.Email.ToLower().Equals(email.ToLower()) && !x.Id.Equals(id));
        }

        public string GetAdminRoleId()
        {
            return new GameContext().Roles.FirstOrDefault(role => role.Name.Equals("Admin"))?.Id;
        }

        public List<IdentityUserRole<string>> GetAdmins(string adminRoleId)
        {
            return new GameContext().UserRoles.Where(userRole => userRole.RoleId.Equals(adminRoleId)).ToList();
        }

        public void DeleteUserResources(User userToDelete)
        {
            GameContext context = new GameContext();
            var friendRequests = context.FriendRequests
                .Where(request =>
                    request.ReceiverID.Equals(userToDelete.Id) || request.SenderID.Equals(userToDelete.Id))
                .ToList();
            context.FriendRequests.RemoveRange(friendRequests);
            var notifications = context.Notifications
                .Where(notification => notification.ReceiverID.Equals(userToDelete.Id))
                .ToList();
            context.Notifications.RemoveRange(notifications);
            var frNotifications = context.RequestNotifications
                .Where(notification => notification.SenderID.Equals(userToDelete.Id))
                .ToList();
            context.RequestNotifications.RemoveRange(frNotifications);
            var lineups = context.UserLineups
                .Where(lineup => lineup.UserID.Equals(userToDelete.Id))
                .ToList();
            context.UserLineups.RemoveRange(lineups);

            context.SaveChanges();
        }

        private int GetPosition(string id)
        {
            return new GameContext().UserLineups
                .Where(lineup => lineup.Date >= _date)
                .AsEnumerable()
                .GroupBy(lineup => lineup.UserID)
                .Select(lineup => new
                {
                    UserID = lineup.Max(l => l.UserID),
                    FP = lineup.Sum(l => l.FP)
                })
                .OrderByDescending(lineup => lineup.FP)
                .Select(lineup => lineup.UserID)
                .ToList()
                .IndexOf(id) + 1;
        }
    }
}