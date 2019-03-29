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
                date = NextGame.NEXT_GAME,
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
                userRecord
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

        private object GetCurrentLineup(string id)
        {
            return _context.UserLineups
                    .Where(lineup => lineup.UserID.Equals(id)
                    && ((lineup.Date.Date == CommonFunctions.UTCToEastern(NextGame.NEXT_GAME).Date)
                        || lineup.Date.Date == CommonFunctions.UTCToEastern(NextGame.PREVIOUS_GAME).Date)
                    && !lineup.IsCalculated)
                    .Select(lineup => new
                    {
                        lineup.UserID,
                        lineup.User.UserName,
                        longDate = lineup.Date.ToString("yyyy-MM-dd"),
                        shortDate = lineup.Date.ToString("MMM. dd"),
                        lineup.Date,
                        lineup.FP,
                        lineup = _context.Players
                        .Where(player =>
                            player.PlayerID == lineup.PgID
                            || player.PlayerID == lineup.SgID
                            || player.PlayerID == lineup.SfID
                            || player.PlayerID == lineup.PfID
                            || player.PlayerID == lineup.CID)
                        .Select(player => new
                        {
                            player.NbaID,
                            player.Position,
                            teamColor = player.Team.Color,
                            player.FullName,
                            player.FirstName,
                            player.LastName,
                            player.AbbrName,
                            FP = _context.Stats.Where(stats => stats.Date.Date == lineup.Date.Date
                                && stats.PlayerID == player.PlayerID)
                            .Select(stats => stats.FP).FirstOrDefault()
                        }).OrderBy(p => Array.IndexOf(CommonFunctions.PlayersOrder, p.Position)),
                        isLive = lineup.Date.Equals(CommonFunctions.UTCToEastern(NextGame.PREVIOUS_GAME).Date) && !lineup.IsCalculated
                    })
                    .FirstOrDefault();
        }

        private IQueryable<Object> GetRecentActivity(string id, int start, int count)
        {
            return _context.UserLineups
                .Where(lineup => lineup.IsCalculated && lineup.UserID.Equals(id))
                .OrderByDescending(lineup => lineup.Date)
                .Skip(start)
                .Take(count)
                .Select(lineup => new
                {
                    lineup.UserID,
                    lineup.User.UserName,
                    longDate = lineup.Date.ToString("yyyy-MM-dd"),
                    shortDate = lineup.Date.ToString("MMM. dd"),
                    lineup.Date,
                    lineup.FP,
                    lineup = _context.Players
                        .Where(player =>
                            player.PlayerID == lineup.PgID
                            || player.PlayerID == lineup.SgID
                            || player.PlayerID == lineup.SfID
                            || player.PlayerID == lineup.PfID
                            || player.PlayerID == lineup.CID)
                        .Select(player => new
                        {
                            player.NbaID,
                            player.Position,
                            teamColor = player.Team.Color,
                            player.FullName,
                            player.FirstName,
                            player.LastName,
                            player.AbbrName,
                            FP = _context.Stats.Where(stats => stats.Date.Date == lineup.Date.Date
                                && stats.PlayerID == player.PlayerID)
                            .Select(stats => stats.FP).FirstOrDefault()
                        }).OrderBy(p => Array.IndexOf(CommonFunctions.PlayersOrder, p.Position))
                });
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
    }
}
