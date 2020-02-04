using fantasy_hoops.Database;
using fantasy_hoops.Dtos;
using fantasy_hoops.Helpers;
using fantasy_hoops.Models;
using fantasy_hoops.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using fantasy_hoops.Jobs;
using fantasy_hoops.Repositories.Interfaces;

namespace fantasy_hoops.Repositories
{
    public class LeaderboardRepository : ILeaderboardRepository
    {
        private readonly GameContext _context;

        public LeaderboardRepository()
        {
            _context = new GameContext();
        }

        public List<PlayerLeaderboardRecordDto> GetPlayerLeaderboard(int from, int limit, string type)
        {
            DateTime date = CommonFunctions.GetDate(type);

            return _context.Stats
                .Where(stats => stats.Date >= date)
                .Select(stats => new
                {
                    stats.Player,
                    teamColor = stats.Player.Team.Color,
                    stats.FP
                })
                .GroupBy(stats => stats.Player.PlayerID)
                .Select(res => new PlayerLeaderboardRecordDto
                {
                    NbaId = res.Max(p => p.Player.NbaID),
                    FullName = res.Max(p => p.Player.FullName),
                    AbbrName = res.Max(p => p.Player.AbbrName),
                    TeamColor = res.Max(p => p.teamColor),
                    FP = res.Sum(stats => stats.FP)
                })
                .OrderByDescending(s => s.FP)
                .Skip(from)
                .Take(limit)
                .ToList();
        }

        public List<UserLeaderboardRecordDto> GetUserLeaderboard(int from, int limit, string type, string date, int weekNumber)
        {
            DateTime previousECT = CommonFunctions.UTCToEastern(NextGameJob.PREVIOUS_GAME);

            DateTime dateTime = date.Length == 0
                ? DateTime.UtcNow < NextGameJob.PREVIOUS_LAST_GAME
                    ? previousECT.AddDays(-1)
                    : previousECT
                : DateTime.ParseExact(date, "yyyyMMdd", CultureInfo.InvariantCulture);
            switch (type)
            {
                case "daily":
                    var dailyStats = _context.Stats.Where(stats => stats.Date.Equals(dateTime.Date));
                    return _context.UserLineups
                    .Where(lineup => lineup.IsCalculated && lineup.Date.Equals(dateTime.Date))
                    .OrderByDescending(lineup => lineup.FP)
                    .Include(lineup => lineup.User)
                    .Select(lineup => new UserLeaderboardRecordDto
                    {
                        UserId = lineup.UserID,
                        Username = lineup.User.UserName,
                        AvatarUrl = lineup.User.AvatarURL,
                        LongDate = lineup.Date.ToString("yyyy-MM-dd"),
                        ShortDate = lineup.Date.ToString("MMM. dd"),
                        Date = lineup.Date,
                        FP = lineup.FP,
                        Lineup = _context.Players
                            .Include(player => player.Team)
                            .Where(player =>
                                player.PlayerID == lineup.PgID
                                || player.PlayerID == lineup.SgID
                                || player.PlayerID == lineup.SfID
                                || player.PlayerID == lineup.PfID
                                || player.PlayerID == lineup.CID)
                            .Select(player => new PlayerDto
                            {
                                PlayerId = player.PlayerID,
                                NbaId = player.NbaID,
                                Position = player.Position,
                                TeamColor = player.Team.Color,
                                FullName = player.FullName,
                                FirstName = player.FirstName,
                                LastName = player.LastName,
                                AbbrName = player.AbbrName,
                                FP = dailyStats.FirstOrDefault(stats => stats.PlayerID == player.PlayerID).FP
                            })
                            .OrderBy(p => CommonFunctions.LineupPositionsOrder.IndexOf(p.Position))
                            .ToList()
                    })
                    .Skip(from)
                    .Take(limit)
                    .ToList();
                case "weekly":
                    int week = weekNumber != -1
                        ? weekNumber
                        : CommonFunctions.GetIso8601WeekOfYear(DateTime.UtcNow);
                    return _context.UserLineups
                        .Include(lineup => lineup.User)
                        .AsEnumerable()
                        .Where(lineup => lineup.IsCalculated
                                         && CommonFunctions.GetIso8601WeekOfYear(lineup.Date) == week
                                         && lineup.Date.Year == DateTime.Now.Year)
                        .GroupBy(lineup => lineup.UserID)
                        .Select(lineup => new UserLeaderboardRecordDto
                        {
                            UserId = lineup.Max(p => p.UserID),
                            Username = lineup.Max(p => p.User.UserName),
                            AvatarUrl = lineup.Max(p => p.User.AvatarURL),
                            FP = Math.Round(lineup.Sum(res => res.FP), 1)
                        })
                        .OrderByDescending(lineup => lineup.FP)
                        .Skip(from)
                        .Take(limit)
                        .ToList();
                default:
                    return _context.UserLineups
                        .Include(lineup => lineup.User)
                        .AsEnumerable()
                        .Where(lineup => lineup.IsCalculated && lineup.Date >= CommonFunctions.GetDate(type))
                        .GroupBy(lineup => lineup.UserID)
                        .Select(lineup => new UserLeaderboardRecordDto
                        {
                            UserId = lineup.Max(p => p.UserID),
                            Username = lineup.Max(p => p.User.UserName),
                            AvatarUrl = lineup.Max(p => p.User.AvatarURL),
                            FP = Math.Round(lineup.Sum(res => res.FP), 1)
                        })
                        .OrderByDescending(lineup => lineup.FP)
                        .Skip(from)
                        .Take(limit)
                        .ToList();
            }
        }

        public List<UserLeaderboardRecordDto> GetFriendsLeaderboard(string id, int from, int limit, string type, string date, int weekNumber)
        {
            DateTime previousECT = CommonFunctions.UTCToEastern(NextGameJob.PREVIOUS_GAME);

            DateTime dateTime = date.Length == 0
                ? DateTime.UtcNow < NextGameJob.PREVIOUS_LAST_GAME
                    ? previousECT.AddDays(-1)
                    : previousECT
                : DateTime.ParseExact(date, "yyyyMMdd", CultureInfo.InvariantCulture);

            var loggedInUser = _context.Users.Where(u => u.Id.Equals(id));
            User user = loggedInUser.FirstOrDefault();
            string loggedInUserId = user != null ? user.Id : "";

            var friendsOnly = _context.FriendRequests
                .Include(u => u.Sender)
                .Where(f => f.ReceiverID.Equals(loggedInUserId) && f.Status == RequestStatus.ACCEPTED)
                .Select(u => u.Sender)
                .Union(_context.FriendRequests
                        .Include(u => u.Receiver)
                        .Where(f => f.SenderID.Equals(loggedInUserId) && f.Status == RequestStatus.ACCEPTED)
                        .Select(u => u.Receiver))
                .Concat(loggedInUser);
            switch (type)
            {
                case "daily":
                    var dailyStats = _context.Stats.Where(stats => stats.Date.Equals(dateTime.Date));

                    return friendsOnly
                    .SelectMany(user => user.UserLineups)
                    .Include(lineup => lineup.User)
                    .AsEnumerable()
                    .Where(lineup => lineup.IsCalculated && lineup.Date.Equals(dateTime.Date))
                    .OrderByDescending(lineup => lineup.FP)
                    .Select(lineup => new UserLeaderboardRecordDto
                    {
                        UserId = lineup.UserID,
                        Username = lineup.User.UserName,
                        AvatarUrl = lineup.User.AvatarURL,
                        LongDate = lineup.Date.ToString("yyyy-MM-dd"),
                        ShortDate = lineup.Date.ToString("MMM. dd"),
                        Date = lineup.Date,
                        FP = lineup.FP,
                        Lineup = _context.Players
                            .Include(player => player.Team)
                            .Where(player =>
                                player.PlayerID == lineup.PgID
                                || player.PlayerID == lineup.SgID
                                || player.PlayerID == lineup.SfID
                                || player.PlayerID == lineup.PfID
                                || player.PlayerID == lineup.CID)
                            .Select(player => new PlayerDto
                            {
                                PlayerId = player.PlayerID,
                                NbaId = player.NbaID,
                                Position = player.Position,
                                TeamColor = player.Team.Color,
                                FullName = player.FullName,
                                FirstName = player.FirstName,
                                LastName = player.LastName,
                                AbbrName = player.AbbrName,
                                FP = dailyStats.FirstOrDefault(stats => stats.PlayerID == player.PlayerID).FP
                            })
                            .OrderBy(p => CommonFunctions.LineupPositionsOrder.IndexOf(p.Position))
                            .ToList()
                    })
                    .Where(x => x.Lineup.Any())
                    .OrderByDescending(x => x.FP)
                    .Skip(from)
                    .Take(limit)
                    .ToList();
                case "weekly":
                    int week = weekNumber != -1
                        ? weekNumber
                        : CommonFunctions.GetIso8601WeekOfYear(CommonFunctions.UTCToEastern(DateTime.UtcNow));
                    return friendsOnly
                        .SelectMany(user => user.UserLineups)
                        .Include(lineup => lineup.User)
                        .AsEnumerable()
                        .Where(lineup => lineup.IsCalculated
                                         && CommonFunctions.GetIso8601WeekOfYear(lineup.Date) == week
                                         && lineup.Date.Year == DateTime.Now.Year)
                        .GroupBy(lineup => lineup.UserID)
                        .Select(lineup => new UserLeaderboardRecordDto
                        {
                            UserId = lineup.Max(p => p.UserID),
                            Username = lineup.Max(p => p.User.UserName),
                            AvatarUrl = lineup.Max(p => p.User.AvatarURL),
                            FP = Math.Round(lineup.Sum(res => res.FP), 1)
                        })
                        .OrderByDescending(lineup => lineup.FP)
                        .Skip(from)
                        .Take(limit)
                        .ToList();
                default:
                    return friendsOnly
                    .Select(user => new UserLeaderboardRecordDto
                    {
                        UserId = user.Id,
                        Username = user.UserName,
                        AvatarUrl = user.AvatarURL,
                        FP = Math.Round(user.UserLineups
                            .Where(lineup => lineup.Date >= CommonFunctions.GetDate(type) && lineup.IsCalculated)
                            .Select(lineup => lineup.FP).Sum(), 1),
                        GamesPlayed = user.UserLineups
                            .Count(lineup => lineup.Date >= CommonFunctions.GetDate(type) && lineup.IsCalculated)
                    })
                    .Where(user => user.GamesPlayed > 0)
                    .OrderByDescending(lineup => lineup.FP)
                    .Skip(from)
                    .Take(limit)
                    .ToList();
            }
        }

        public List<UserLeaderboardRecordDto> GetSeasonLineups(int year)
        {
            DateTime seasonStart = DateTime.MinValue;
            DateTime seasonEnd = DateTime.MaxValue;
            if (year != -1)
            {
                seasonStart = new DateTime(year, 10, 1);
                seasonEnd = new DateTime(year + 1, 7, 1);
            }
            
            return _context.UserLineups
                .Where(lineup => lineup.Date >= seasonStart && lineup.Date <= seasonEnd)
                .OrderByDescending(lineup => lineup.FP)
                .Take(10)
                .Include(lineup => lineup.User)
                .Select(lineup => new UserLeaderboardRecordDto
                {
                    UserId = lineup.UserID,
                    Username = lineup.User.UserName,
                    AvatarUrl = lineup.User.AvatarURL,
                    LongDate = lineup.Date.ToString("yyyy-MM-dd"),
                    ShortDate = lineup.Date.ToString("MMM. dd"),
                    Date = lineup.Date,
                    FP = lineup.FP,
                    Lineup = _context.Players
                        .Include(player => player.Team)
                        .Include(player => player.Stats)
                        .Where(player =>
                            player.PlayerID == lineup.PgID
                            || player.PlayerID == lineup.SgID
                            || player.PlayerID == lineup.SfID
                            || player.PlayerID == lineup.PfID
                            || player.PlayerID == lineup.CID)
                        .Select(player => new PlayerDto
                        {
                            NbaId = player.NbaID,
                            PlayerId = player.PlayerID,
                            Position = player.Position,
                            TeamColor = player.Team.Color,
                            FullName = player.FullName,
                            FirstName = player.FirstName,
                            LastName = player.LastName,
                            AbbrName = player.AbbrName,
                            FP = player.Stats.FirstOrDefault(stats => stats.Date.Equals(lineup.Date)).FP
                        })
                        .OrderBy(p => CommonFunctions.LineupPositionsOrder.IndexOf(p.Position))
                        .ToList()
                })
                .ToList();
        }

        public List<PlayerLeaderboardRecordDto> GetSeasonPlayers(int year)
        {
            DateTime seasonStart = DateTime.MinValue;
            DateTime seasonEnd = DateTime.MaxValue;
            if (year != -1)
            {
                seasonStart = new DateTime(year, 10, 1);
                seasonEnd = new DateTime(year + 1, 7, 1);
            }

            return _context.Stats
                .Where(stats => stats.Date >= seasonStart && stats.Date <= seasonEnd)
                .OrderByDescending(s => s.FP)
                .Take(10)
                .Select(p => new PlayerLeaderboardRecordDto
                {
                    NbaId = p.Player.NbaID,
                    TeamColor = p.Player.Team.Color,
                    FullName = p.Player.FullName,
                    AbbrName = p.Player.AbbrName,
                    FP = p.FP,
                    LongDate = p.Date.ToString("yyyy-MM-dd"),
                    ShortDate = p.Date.ToString("MMM. dd")
                })
                .ToList();
        }

        public List<PlayerLeaderboardRecordDto> GetMostSelectedPlayers(int from, int count)
        {
            var userLineups = _context.UserLineups
                .Where(lineup => lineup.Date < NextGameJob.PREVIOUS_GAME);
            var pg = userLineups
                .Select(lineup => new PlayerDto
                {
                    NbaId = lineup.Pg.NbaID,
                    AbbrName = lineup.Pg.AbbrName,
                    TeamColor = lineup.Pg.Team.Color
                })
                .ToList();
            var sg = userLineups
                .Select(lineup => new PlayerDto
                {
                    NbaId = lineup.Sg.NbaID,
                    AbbrName = lineup.Sg.AbbrName,
                    TeamColor = lineup.Sg.Team.Color
                })
                .ToList();
            var sf = userLineups
                .Select(lineup => new PlayerDto
                {
                    NbaId = lineup.Sf.NbaID,
                    AbbrName = lineup.Sf.AbbrName,
                    TeamColor = lineup.Sf.Team.Color
                })
                .ToList();
            var pf = userLineups
                .Select(lineup => new PlayerDto
                {
                    NbaId = lineup.Pf.NbaID,
                    AbbrName = lineup.Pf.AbbrName,
                    TeamColor = lineup.Pf.Team.Color
                })
                .ToList();
            var c = userLineups
                .Select(lineup => new PlayerDto
                {
                    NbaId = lineup.C.NbaID,
                    AbbrName = lineup.C.AbbrName,
                    TeamColor = lineup.C.Team.Color
                })
                .ToList();

            return pg.Concat(sg).Concat(sf).Concat(pf).Concat(c)
                .Select(lineup => lineup)
                .GroupBy(lineup => lineup.NbaId)
                .Select(res => new PlayerLeaderboardRecordDto
                {
                    NbaId = res.Max(p => p.NbaId),
                    AbbrName = res.Max(p => p.AbbrName),
                    TeamColor = res.Max(p => p.TeamColor),
                    Count = res.Count()
                })
                .OrderByDescending(player => player.Count)
                .Skip(from)
                .Take(count)
                .ToList();
        }
    }
}
