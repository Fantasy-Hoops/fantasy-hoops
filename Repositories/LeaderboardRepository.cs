using fantasy_hoops.Database;
using fantasy_hoops.Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace fantasy_hoops.Repositories
{
    public class LeaderboardRepository : ILeaderboardRepository
    {

        private readonly GameContext _context;

        public LeaderboardRepository(GameContext context)
        {
            _context = context;
        }

        public IEnumerable<object> GetPlayerLeaderboard(int from, int limit, string type)
        {
            DateTime date = CommonFunctions.GetDate(type);

            return _context.Players
                .Select(x => new
                {
                    x.PlayerID,
                    x.NbaID,
                    x.FullName,
                    x.FirstName,
                    x.LastName,
                    x.AbbrName,
                    x.Position,
                    teamColor = x.Team.Color,
                    FP = x.Stats
                        .Where(y => y.Date >= date)
                        .Select(y => y.FP).Sum()
                })
                .Where(x => x.FP > 0)
                .OrderByDescending(x => x.FP)
                .Skip(from)
                .Take(limit);
        }

        public IEnumerable<object> GetUserLeaderboard(int from, int limit, string type)
        {
            DateTime date = CommonFunctions.GetDate(type);
            if (type.Equals("daily"))
            {
                return _context.Lineups
                .Where(x => x.Calculated && x.Date >= date)
                .GroupBy(l => new { l.UserID, l.Date })
                .Select(res => new
                {
                    res.First().UserID,
                    res.First().User.UserName,
                    res.First().Date,
                    score = Math.Round(res.Sum(c => c.FP), 1),
                    lineup = res.Select(l => new
                    {
                        l.Player.NbaID,
                        l.Player.Position,
                        teamColor = l.Player.Team.Color,
                        l.Player.FullName,
                        l.Player.FirstName,
                        l.Player.LastName,
                        l.Player.AbbrName,
                        l.FP
                    }).OrderBy(p => Array.IndexOf(CommonFunctions.PlayersOrder, p.Position))
                })
                .OrderByDescending(x => x.score)
                .Skip(from)
                .Take(limit);
            }
            return _context.Lineups
                .Where(x => x.Calculated && x.Date >= date)
                .GroupBy(l => l.UserID)
                .Select(res => new
                {
                    res.First().UserID,
                    res.First().User.UserName,
                    res.First().Date,
                    score = Math.Round(res.Sum(c => c.FP), 2)
                })
                .OrderByDescending(x => x.score)
                .Skip(from)
                .Take(limit);
        }

        public IEnumerable<object> GetFriendsLeaderboard(string id, int from, int limit, string type)
        {
            var loggedInUser = _context.Users.Where(u => u.Id.Equals(id));
            var friendsOnly = _context.FriendRequests
                .Include(u => u.Sender)
                .Where(f => f.ReceiverID.Equals(loggedInUser.FirstOrDefault().Id) && f.Status == Models.RequestStatus.ACCEPTED)
                .Select(u => u.Sender)
                .Union(_context.FriendRequests
                .Include(u => u.Receiver)
                .Where(f => f.SenderID.Equals(loggedInUser.FirstOrDefault().Id) && f.Status == Models.RequestStatus.ACCEPTED)
                .Select(u => u.Receiver)).Concat(loggedInUser);

            DateTime date = CommonFunctions.GetDate(type);

            if (type.Equals("daily"))
            {
                return friendsOnly
                .Select(x => new
                {
                    userID = x.Id,
                    x.UserName,
                    Score = Math.Round(x.Lineups
                        .Where(y => y.Date >= date && y.Calculated)
                        .Select(y => y.FP).Sum(), 2),
                    lineup = x.Lineups.Where(y => y.Date >= date && y.Calculated)
                    .Select(l => new
                    {
                        l.Player.NbaID,
                        l.Player.Position,
                        teamColor = l.Player.Team.Color,
                        l.Player.FullName,
                        l.Player.FirstName,
                        l.Player.LastName,
                        l.Player.AbbrName,
                        l.FP
                    }).OrderBy(p => Array.IndexOf(CommonFunctions.PlayersOrder, p.Position))
                })
                .Where(y => y.Score > 0)
                .OrderByDescending(x => x.Score)
                .Skip(from)
                .Take(limit);
            }
            return friendsOnly
                .Select(x => new
                {
                    userID = x.Id,
                    x.UserName,
                    Score = Math.Round(x.Lineups
                        .Where(y => y.Date >= date && y.Calculated)
                        .Select(y => y.FP).Sum(), 2)
                })
                .Where(y => y.Score > 0)
                .OrderByDescending(x => x.Score)
                .Skip(from)
                .Take(limit);
        }

        public IQueryable<object> GetSeasonLineups()
        {
            return _context.Lineups
                .GroupBy(l => new { l.UserID, l.Date })
                .Select(res => new
                {
                    res.First().UserID,
                    res.First().User.UserName,
                    res.First().Date,
                    score = Math.Round(res.Sum(c => c.FP), 1),
                    lineup = res.Select(l => new
                    {
                        l.Player.NbaID,
                        l.Player.Position,
                        teamColor = l.Player.Team.Color,
                        l.Player.FullName,
                        l.Player.FirstName,
                        l.Player.LastName,
                        l.Player.AbbrName,
                        l.FP
                    }).OrderBy(p => Array.IndexOf(CommonFunctions.PlayersOrder, p.Position))
                })
                .OrderByDescending(t => t.score)
                .Take(10);
        }

        public IQueryable<object> GetSeasonPlayers()
        {
            return _context.Stats
                .OrderByDescending(s => s.FP)
                .Take(10)
                .Select(p => new
                {
                    p.Player.NbaID,
                    teamColor = p.Player.Team.Color,
                    p.Player.FullName,
                    p.Player.AbbrName,
                    p.FP

                });
        }
    }
}
