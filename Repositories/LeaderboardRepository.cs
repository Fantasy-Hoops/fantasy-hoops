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
                return _context.Users
                .Select(x => new
                {
                    x.Id,
                    x.UserName,
                    Score = Math.Round(x.Lineups
                        .Where(y => y.Date >= date && y.Calculated)
                        .Select(y => y.FP).Sum(), 2),
                    pg = x.Lineups
                        .Where(y => y.Date >= date && y.Position.Equals("PG") && y.Calculated).Select(l => new
                        {
                            l.Player.NbaID,
                            teamColor = l.Player.Team.Color,
                            l.Player.FullName,
                            l.Player.FirstName,
                            l.Player.LastName,
                            l.Player.AbbrName,
                            l.FP
                        }).FirstOrDefault(),
                    sg = x.Lineups
                        .Where(y => y.Date >= date && y.Position.Equals("SG") && y.Calculated).Select(l => new
                        {
                            l.Player.NbaID,
                            teamColor = l.Player.Team.Color,
                            l.Player.FullName,
                            l.Player.FirstName,
                            l.Player.LastName,
                            l.Player.AbbrName,
                            l.FP
                        }).FirstOrDefault(),
                    sf = x.Lineups
                        .Where(y => y.Date >= date && y.Position.Equals("SF") && y.Calculated).Select(l => new
                        {
                            l.Player.NbaID,
                            teamColor = l.Player.Team.Color,
                            l.Player.FullName,
                            l.Player.FirstName,
                            l.Player.LastName,
                            l.Player.AbbrName,
                            l.FP
                        }).FirstOrDefault(),
                    pf = x.Lineups
                        .Where(y => y.Date >= date && y.Position.Equals("PF") && y.Calculated).Select(l => new
                        {
                            l.Player.NbaID,
                            teamColor = l.Player.Team.Color,
                            l.Player.FullName,
                            l.Player.FirstName,
                            l.Player.LastName,
                            l.Player.AbbrName,
                            l.FP
                        }).FirstOrDefault(),
                    c = x.Lineups
                        .Where(y => y.Date >= date && y.Position.Equals("C") && y.Calculated).Select(l => new
                        {
                            l.Player.NbaID,
                            teamColor = l.Player.Team.Color,
                            l.Player.FullName,
                            l.Player.FirstName,
                            l.Player.LastName,
                            l.Player.AbbrName,
                            l.FP
                        }).FirstOrDefault()

                })
                .Where(y => y.Score > 0)
                .OrderByDescending(x => x.Score)
                .Skip(from)
                .Take(limit);
            }
            return _context.Users
                .Select(x => new
                {
                    x.Id,
                    x.UserName,
                    Score = Math.Round(x.Lineups
                        .Where(y => y.Date >= date && y.Calculated)
                        .Select(y => y.FP).Sum(), 2),
                    pg = x.Lineups
                        .Where(y => y.Date >= date && y.Position.Equals("PG") && y.Calculated).Select(l => new
                        {
                            l.Player.NbaID,
                            teamColor = l.Player.Team.Color,
                            l.Player.FullName,
                            l.Player.FirstName,
                            l.Player.LastName,
                            l.Player.AbbrName,
                            l.FP
                        }).FirstOrDefault(),
                    sg = x.Lineups
                        .Where(y => y.Date >= date && y.Position.Equals("SG") && y.Calculated).Select(l => new
                        {
                            l.Player.NbaID,
                            teamColor = l.Player.Team.Color,
                            l.Player.FullName,
                            l.Player.FirstName,
                            l.Player.LastName,
                            l.Player.AbbrName,
                            l.FP
                        }).FirstOrDefault(),
                    sf = x.Lineups
                        .Where(y => y.Date >= date && y.Position.Equals("SF") && y.Calculated).Select(l => new
                        {
                            l.Player.NbaID,
                            teamColor = l.Player.Team.Color,
                            l.Player.FullName,
                            l.Player.FirstName,
                            l.Player.LastName,
                            l.Player.AbbrName,
                            l.FP
                        }).FirstOrDefault(),
                    pf = x.Lineups
                        .Where(y => y.Date >= date && y.Position.Equals("PF") && y.Calculated).Select(l => new
                        {
                            l.Player.NbaID,
                            teamColor = l.Player.Team.Color,
                            l.Player.FullName,
                            l.Player.FirstName,
                            l.Player.LastName,
                            l.Player.AbbrName,
                            l.FP
                        }).FirstOrDefault(),
                    c = x.Lineups
                        .Where(y => y.Date >= date && y.Position.Equals("C") && y.Calculated).Select(l => new
                        {
                            l.Player.NbaID,
                            teamColor = l.Player.Team.Color,
                            l.Player.FullName,
                            l.Player.FirstName,
                            l.Player.LastName,
                            l.Player.AbbrName,
                            l.FP
                        }).FirstOrDefault()

                })
                .Where(y => y.Score > 0)
                .OrderByDescending(x => x.Score)
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

            return friendsOnly
                .Select(x => new
                {
                    x.Id,
                    x.UserName,
                    Score = Math.Round(x.Lineups
                        .Where(y => y.Date >= date && y.Calculated)
                        .Select(y => y.FP).Sum(), 2),
                    pg = x.Lineups
                        .Where(y => y.Date >= date && y.Position.Equals("PG") && y.Calculated).Select(l => new
                        {
                            l.Player.NbaID,
                            teamColor = l.Player.Team.Color,
                            l.Player.FullName,
                            l.Player.FirstName,
                            l.Player.LastName,
                            l.Player.AbbrName,
                            l.FP
                        }).FirstOrDefault(),
                    sg = x.Lineups
                        .Where(y => y.Date >= date && y.Position.Equals("SG") && y.Calculated).Select(l => new
                        {
                            l.Player.NbaID,
                            teamColor = l.Player.Team.Color,
                            l.Player.FullName,
                            l.Player.FirstName,
                            l.Player.LastName,
                            l.Player.AbbrName,
                            l.FP
                        }).FirstOrDefault(),
                    sf = x.Lineups
                        .Where(y => y.Date >= date && y.Position.Equals("SF") && y.Calculated).Select(l => new
                        {
                            l.Player.NbaID,
                            teamColor = l.Player.Team.Color,
                            l.Player.FullName,
                            l.Player.FirstName,
                            l.Player.LastName,
                            l.Player.AbbrName,
                            l.FP
                        }).FirstOrDefault(),
                    pf = x.Lineups
                        .Where(y => y.Date >= date && y.Position.Equals("PF") && y.Calculated).Select(l => new
                        {
                            l.Player.NbaID,
                            teamColor = l.Player.Team.Color,
                            l.Player.FullName,
                            l.Player.FirstName,
                            l.Player.LastName,
                            l.Player.AbbrName,
                            l.FP
                        }).FirstOrDefault(),
                    c = x.Lineups
                        .Where(y => y.Date >= date && y.Position.Equals("C") && y.Calculated).Select(l => new
                        {
                            l.Player.NbaID,
                            teamColor = l.Player.Team.Color,
                            l.Player.FullName,
                            l.Player.FirstName,
                            l.Player.LastName,
                            l.Player.AbbrName,
                            l.FP
                        }).FirstOrDefault()
                })
                .Where(y => y.Score > 0)
                .OrderByDescending(x => x.Score)
                .Skip(from)
                .Take(limit);
        }

    }
}
