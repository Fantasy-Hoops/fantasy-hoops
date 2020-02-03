using System;
using System.Collections.Generic;
using System.Linq;
using fantasy_hoops.Database;
using fantasy_hoops.Dtos;
using fantasy_hoops.Helpers;
using fantasy_hoops.Jobs;
using fantasy_hoops.Models;
using fantasy_hoops.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace fantasy_hoops.Repositories
{
    public class UserRepository : IUserRepository
    {
        readonly DateTime date = CommonFunctions.GetDate("weekly");

        private readonly GameContext _context;
        private readonly ITeamRepository _teamRepository;

        public UserRepository(ITeamRepository repository)
        {
            _context = new GameContext();
            _teamRepository = repository;
        }

        public IQueryable<Object> GetProfile(string id, int start, int count)
        {
            User user = GetUser(id);

            Team team = _teamRepository.GetTeamById(user.FavoriteTeamId);
            if (team == null)
            {
                team = new Team()
                {
                    City = "Seattle",
                    Name = "Supersonics",
                    Color = "#FFC200"
                };
            }

            var activity = GetRecentActivity(id, start, count).ToList();
            var currentLineup = GetCurrentLineup(id);
            int streak = GetStreak(id);
            decimal totalScore = GetWeeklyScore(id);
            int position = GetWeeklyRanking(id);
            decimal userRecord = GetUserRecord(id);

            var profile = _context.Users.Where(x => x.Id.Equals(id)).Select(x => new
            {
                x.Id,
                x.UserName,
                x.Email,
                x.Description,
                x.FavoriteTeamId,
                date = NextGameJob.NEXT_GAME,
                Team = new
                {
                    Name = team.City + " " + team.Name,
                    team.Color
                },
                recentActivity = activity,
                currentLineup,
                Streak = streak,
                Position = position,
                TotalScore = totalScore,
                userRecord,
                x.AvatarURL
            });
            return profile;
        }

        public User GetUser(string id)
        {
            return _context.Users
                .Where(x => x.Id.Equals(id))
                .FirstOrDefault();
        }

        public User GetUserByName(string username)
        {
            return _context.Users
                .Where(x => x.UserName.ToLower().Equals(username.ToLower()))
                .FirstOrDefault();
        }

        public IQueryable<object> Roles(string id)
        {
            return _context.UserRoles
                .Where(userRole => userRole.UserId.Equals(id))
                .Join(_context.Roles,
                    userRole => userRole.RoleId,
                    role => role.Id,
                    (userRole, role) => role.NormalizedName);
        }

        public bool IsAdmin(string userId)
        {
            string adminRoleId = _context.Roles
                .Where(role => role.NormalizedName.Equals("Admin"))
                .Select(role => role.Id)
                .FirstOrDefault();

            return _context.UserRoles
                .Where(userRole => userRole.UserId.Equals(userId))
                .Any(role => role.RoleId.Equals(adminRoleId));
        }

        public IQueryable<Object> GetFriends(string id)
        {
            var friends = _context.FriendRequests
               .Where(x => x.ReceiverID.Equals(id) && x.Status.Equals(RequestStatus.ACCEPTED))
               .Select(x => new
               {
                   id = x.SenderID,
                   x.Sender.UserName,
                   Color = _context.Teams
                       .Where(t => t.TeamID == x.Sender.FavoriteTeamId)
                       .Select(t => t.Color)
                       .FirstOrDefault(),
                   x.Sender.AvatarURL
               })
               .Union(_context.FriendRequests
               .Where(x => x.SenderID.Equals(id) && x.Status.Equals(RequestStatus.ACCEPTED))
               .Select(x => new
               {
                   id = x.ReceiverID,
                   x.Receiver.UserName,
                   Color = _context.Teams
                     .Where(t => t.TeamID == x.Receiver.FavoriteTeamId)
                     .Select(t => t.Color)
                     .FirstOrDefault(),
                   x.Receiver.AvatarURL
               }));

            return friends;
        }

        public IQueryable<Object> GetTeam(string id)
        {
            return _context.Users
                .Where(usr => usr.Id.Equals(id))
                .Select(x => new
                {
                    team = _context.Teams.Where(team => team.TeamID == x.FavoriteTeamId).FirstOrDefault()
                });
        }

        public IQueryable<Object> GetUserPool()
        {
            return _context.Users
                .Select(x => new
                {
                    x.UserName,
                    x.Id,
                    color = _context.Teams
                        .Where(y => y.TeamID == x.FavoriteTeamId)
                        .Select(y => y.Color)
                        .FirstOrDefault(),
                    x.AvatarURL
                })
                .OrderBy(x => x.UserName);
        }

        public bool UserExists(string username)
        {
            return _context.Users
                .Where(x => x.UserName.ToLower().Equals(username.ToLower()))
                .Any();
        }

        public bool EmailExists(string email)
        {
            return _context.Users
                .Where(x => x.Email.ToLower().Equals(email.ToLower()))
                .Any();
        }

        private UserLeaderboardRecordDto GetCurrentLineup(string id)
        {
            return _context.UserLineups
                    .Where(lineup => lineup.UserID.Equals(id)
                    && ((lineup.Date.Date == CommonFunctions.UTCToEastern(NextGameJob.NEXT_GAME).Date)
                        || lineup.Date.Date == CommonFunctions.UTCToEastern(NextGameJob.PREVIOUS_GAME).Date)
                    && !lineup.IsCalculated)
                    .Select(lineup => new UserLeaderboardRecordDto
                    {
                        UserId = lineup.UserID,
                        Username = lineup.User.UserName,
                        LongDate = lineup.Date.ToString("yyyy-MM-dd"),
                        ShortDate = lineup.Date.ToString("MMM. dd"),
                        Date = lineup.Date,
                        FP = lineup.FP,
                        Lineup = _context.Players
                            .Where(player =>
                                player.PlayerID == lineup.PgID
                                || player.PlayerID == lineup.SgID
                                || player.PlayerID == lineup.SfID
                                || player.PlayerID == lineup.PfID
                                || player.PlayerID == lineup.CID)
                            .Select(player => new PlayerDto
                            {
                                NbaId = player.NbaID,
                                Position = player.Position,
                                TeamColor = player.Team.Color,
                                FullName = player.FullName,
                                FirstName = player.FirstName,
                                LastName = player.LastName,
                                AbbrName = player.AbbrName,
                                FP = _context.Stats.Where(stats => stats.Date.Date == lineup.Date.Date
                                    && stats.PlayerID == player.PlayerID)
                                .Select(stats => stats.FP).FirstOrDefault()
                            })
                            .OrderBy(p => CommonFunctions.LineupPositionsOrder.IndexOf(p.Position))
                            .ToList(),
                        IsLive = lineup.Date.Equals(CommonFunctions.UTCToEastern(NextGameJob.PREVIOUS_GAME).Date) && !lineup.IsCalculated
                    })
                    .FirstOrDefault();
        }

        private List<UserLeaderboardRecordDto> GetRecentActivity(string id, int start, int count)
        {
            return _context.UserLineups
                .Where(lineup => lineup.IsCalculated && lineup.UserID.Equals(id))
                .OrderByDescending(lineup => lineup.Date)
                .Skip(start)
                .Take(count)
                .Select(lineup => new UserLeaderboardRecordDto
                {
                    UserId = lineup.UserID,
                    Username = lineup.User.UserName,
                    LongDate = lineup.Date.ToString("yyyy-MM-dd"),
                    ShortDate = lineup.Date.ToString("MMM. dd"),
                    Date = lineup.Date,
                    FP = lineup.FP,
                    Lineup = _context.Players
                        .Where(player =>
                            player.PlayerID == lineup.PgID
                            || player.PlayerID == lineup.SgID
                            || player.PlayerID == lineup.SfID
                            || player.PlayerID == lineup.PfID
                            || player.PlayerID == lineup.CID)
                        .Select(player => new PlayerDto
                        {
                            NbaId = player.NbaID,
                            Position = player.Position,
                            TeamColor = player.Team.Color,
                            FullName = player.FullName,
                            FirstName = player.FirstName,
                            LastName = player.LastName,
                            AbbrName = player.AbbrName,
                            FP = _context.Stats.Where(stats => stats.Date.Date == lineup.Date.Date
                                && stats.PlayerID == player.PlayerID)
                            .Select(stats => stats.FP).FirstOrDefault()
                        })
                        .OrderBy(p => CommonFunctions.LineupPositionsOrder.IndexOf(p.Position))
                        .ToList()
                })
                .ToList();
        }

        private int GetStreak(string id)
        {
            return _context.Users.Where(u => u.Id.Equals(id)).FirstOrDefault().Streak;
        }

        private decimal GetUserRecord(string id)
        {
            var userLineup = _context.Users
                .Where(user => user.Id.Equals(id))
                .SelectMany(user => user.UserLineups)
                .OrderByDescending(lineup => lineup.FP)
                .FirstOrDefault();

            if (userLineup == null)
                return 0.0m;

            decimal record = Convert.ToDecimal(userLineup.FP);

            if ((record % 1) == 0)
                return 0.0m + record;

            return record;
        }

        private decimal GetWeeklyScore(string id)
        {
            decimal weekly = Convert.ToDecimal(_context.UserLineups
                    .Where(lineup => lineup.UserID.Equals(id) && lineup.Date >= date)
                    .Select(lineup => lineup.FP).Sum());
            if ((weekly % 1) == 0)
                return 0.0m + weekly;
            return Convert.ToDecimal(weekly);
        }

        private int GetWeeklyRanking(string id)
        {

            var ranking = _context.Users.Select(x => new
            {
                x.Id,
                Score = _context.UserLineups
                    .Where(lineup => lineup.UserID.Equals(x.Id) && lineup.Date >= date)
                    .Select(lineup => lineup.FP).Sum(),
                Ranking = 0
            })
            .Where(x => x.Score > 0)
            .OrderByDescending(x => x.Score)
            .ToList();

            int position = 0;
            int rank = 1;
            ranking.ForEach(x =>
            {
                if (x.Id.Equals(id))
                {
                    position = rank;
                }
                rank++;
            });
            return position;
        }

        public bool IsDuplicateUserName(string id, string username)
        {
            return _context.Users.Where(x => x.UserName.ToLower().Equals(username.ToLower()) && !x.Id.Equals(id)).Any();
        }

        public bool IsDuplicateEmail(string id, string email)
        {
            return _context.Users.Where(x => x.Email.ToLower().Equals(email.ToLower()) && !x.Id.Equals(id)).Any();
        }

        public string GetAdminRoleId()
        {
            return _context.Roles.Where(role => role.Name.Equals("Admin")).FirstOrDefault().Id;
        }

        public List<IdentityUserRole<string>> GetAdmins(string adminRoleId)
        {
            return _context.UserRoles.Where(userRole => userRole.RoleId.Equals(adminRoleId)).ToList();
        }

        public void DeleteUserResources(User userToDelete)
        {
            var friendRequests = _context.FriendRequests
                .Where(request => request.ReceiverID.Equals(userToDelete.Id) || request.SenderID.Equals(userToDelete.Id))
                .ToList();
            _context.FriendRequests.RemoveRange(friendRequests);
            var notifications = _context.Notifications
                .Where(notification => notification.UserID.Equals(userToDelete.Id))
                .ToList();
            _context.Notifications.RemoveRange(notifications);
            var frNotifications = _context.FriendRequestNotifications
                .Where(notification => notification.FriendID.Equals(userToDelete.Id))
                .ToList();
            _context.FriendRequestNotifications.RemoveRange(frNotifications);
            var lineups = _context.UserLineups
                .Where(lineup => lineup.UserID.Equals(userToDelete.Id))
                .ToList();
            _context.UserLineups.RemoveRange(lineups);

            _context.SaveChanges();
        }
    }
}
