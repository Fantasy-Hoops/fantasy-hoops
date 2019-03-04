using System;
using System.Linq;
using fantasy_hoops.Database;
using fantasy_hoops.Helpers;
using fantasy_hoops.Models;

namespace fantasy_hoops.Repositories
{
    public class UserRepository : IUserRepository
    {

        private readonly GameContext _context;
        private readonly TeamRepository _teamRepository;
        DateTime date = CommonFunctions.GetDate("weekly");

        public UserRepository(GameContext context)
        {
            _context = context;
            _teamRepository = new TeamRepository(_context);
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
            int streak = GetStreak(id);
            decimal totalScore = GetWeeklyScore(id);
            int position = GetWeeklyRanking(id);

            var profile = _context.Users.Where(x => x.Id.Equals(id)).Select(x => new
            {
                x.Id,
                x.UserName,
                x.Email,
                x.Description,
                x.FavoriteTeamId,
                date = NextGame.NEXT_GAME,
                Team = new
                {
                    Name = team.City + " " + team.Name,
                    team.Color
                },
                recentActivity = activity,
                Streak = streak,
                Position = position,
                TotalScore = totalScore
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
                       .FirstOrDefault()
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
                     .FirstOrDefault()
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
                        .FirstOrDefault()
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

        private IQueryable<Object> GetRecentActivity(string id, int start, int count)
        {
            var recentActivity = _context.Lineups
                .Where(lineup => lineup.Calculated && lineup.UserID.Equals(id))
                .GroupBy(lineup => new { lineup.UserID, lineup.Date })
                .Select(result => new
                {
                    result.First().Date,
                    longDate = result.First().Date.ToString("yyyy-MM-dd"),
                    shortDate = result.First().Date.ToString("MMM. dd"),
                    score = Math.Round(result.Sum(c => c.FP), 1),
                    lineup = result.Select(lineup => new
                    {
                        lineup.Player.NbaID,
                        lineup.Player.Position,
                        teamColor = lineup.Player.Team.Color,
                        lineup.Player.FullName,
                        lineup.Player.FirstName,
                        lineup.Player.LastName,
                        lineup.Player.AbbrName,
                        lineup.FP
                    }).OrderBy(player => Array.IndexOf(CommonFunctions.PlayersOrder, player.Position))
                })
                .OrderByDescending(lineup => lineup.Date)
                .Skip(start)
                .Take(count);
            return recentActivity;
        }

        private int GetStreak(string id)
        {
            return _context.Users.Where(u => u.Id.Equals(id)).FirstOrDefault().Streak;
        }

        private decimal GetWeeklyScore(string id)
        {
            decimal weekly = Convert.ToDecimal(_context.Lineups
                    .Where(x => x.UserID.Equals(id) && x.Date >= date)
                    .Select(x => x.FP).Sum());
            if ((weekly % 1) == 0)
            {
                return 0.0m + weekly;
            }
            return Convert.ToDecimal(weekly);
        }

        private int GetWeeklyRanking(string id)
        {

            var ranking = _context.Users.Select(x => new
            {
                x.Id,
                Score = _context.Lineups
                    .Where(y => y.UserID.Equals(x.Id) && y.Date >= date)
                    .Select(y => y.FP).Sum(),
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
    }
}
