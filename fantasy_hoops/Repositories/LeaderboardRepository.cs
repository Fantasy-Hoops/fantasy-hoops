﻿using fantasy_hoops.Database;
using fantasy_hoops.Dtos;
using fantasy_hoops.Helpers;
using fantasy_hoops.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using fantasy_hoops.Models.Enums;
using fantasy_hoops.Repositories.Interfaces;

namespace fantasy_hoops.Repositories
{
    public class LeaderboardRepository : ILeaderboardRepository
    {
        private const int NoData = -1;
        
        private readonly GameContext _context;

        public LeaderboardRepository()
        {
            _context = new GameContext();
        }

        public List<PlayerLeaderboardRecordDto> GetPlayerLeaderboard(int from, int limit, LeaderboardType type)
        {
            GameContext context = new GameContext();
            DateTime date = CommonFunctions.Instance.GetLeaderboardDate(type);
            if (type == LeaderboardType.DAILY)
            {
                date = context.Stats.Max(stats => stats.Date);
            }

            return context.Stats
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

        public List<UserLeaderboardRecordDto> GetUserLeaderboard(int from, int limit, LeaderboardType type, string date,
            int weekNumber, int year)
        {
            DateTime previousEct = CommonFunctions.Instance.UTCToEastern(RuntimeUtils.PREVIOUS_GAME);
            DateTime dateTime = date.Length == 0
                ? DateTime.UtcNow < RuntimeUtils.PREVIOUS_LAST_GAME
                    ? previousEct.AddDays(-1)
                    : previousEct
                : DateTime.ParseExact(date, "yyyyMMdd", CultureInfo.InvariantCulture);
            
            var userLineups = GetUserLineupsBaseQuery(type, dateTime, year, weekNumber);
            switch (type)
            {
                case LeaderboardType.DAILY:
                    return userLineups.Select(lineup => new UserLeaderboardRecordDto
                        {
                            UserId = lineup.UserID,
                            Username = lineup.User.UserName,
                            AvatarUrl = lineup.User.AvatarURL,
                            LongDate = lineup.Date.ToString("yyyy-MM-dd"),
                            ShortDate = lineup.Date.ToString("MMM. dd"),
                            Date = lineup.Date,
                            FP = lineup.FP,
                            Lineup = new List<Player> {lineup.Pg, lineup.Sg, lineup.Sf, lineup.Pf, lineup.C}
                                .Select(player => new LineupPlayerDto
                                {
                                    Player = player,
                                    TeamColor = player.Team.Color,
                                    FP = player.Stats.Where(stats => stats.Date.Equals(dateTime.Date))
                                        .Select(stats => stats.FP)
                                        .FirstOrDefault(),
                                    Price = player.Price
                                }).OrderBy(lineupPlayer =>
                                    CommonFunctions.Instance.LineupPositionsOrder.IndexOf(lineupPlayer.Player.Position,
                                        StringComparison.Ordinal))
                                .ToList()
                        })
                        .Skip(from)
                        .Take(limit)
                        .ToList();
                case LeaderboardType.WEEKLY:
                case LeaderboardType.MONTHLY:
                case LeaderboardType.FROM_DATE:
                    return userLineups.GroupBy(lineup => lineup.UserID)
                        .Select(lineup => new UserLeaderboardRecordDto
                        {
                            UserId = lineup.Max(userLineup => userLineup.UserID),
                            Username = lineup.Max(userLineup => userLineup.User.UserName),
                            AvatarUrl = lineup.Max(userLineup => userLineup.User.AvatarURL),
                            FP = Math.Round(lineup.Sum(userLineup => userLineup.FP), 1)
                        })
                        .OrderByDescending(lineup => lineup.FP)
                        .Skip(from)
                        .Take(limit)
                        .ToList();
                default:
                    return new List<UserLeaderboardRecordDto>();
            }
        }

        private List<UserLineup> GetUserLineupsBaseQuery(LeaderboardType type, DateTime dateTime, int year, int weekNumber)
        {
            int leaderboardYear = year == NoData
                ? CommonFunctions.Instance.UTCToEastern(DateTime.UtcNow).Year
                : year;
            int week = weekNumber == NoData
                ? CommonFunctions.Instance.GetIso8601WeekOfYear(
                    _context.UserLineups.Where(lineup => lineup.IsCalculated).Max(lineup => lineup.Date)
                )
                : weekNumber;
            DateTime firstDayOfWeek =
                CommonFunctions.Instance.FirstDateOfWeek(leaderboardYear, week, CultureInfo.CurrentCulture);
            DateTime lastDayOfWeek =
                CommonFunctions.Instance.LastDayOfWeek(leaderboardYear, week, CultureInfo.CurrentCulture);
            return _context.UserLineups
                .Include(lineup => lineup.User)
                .Include(lineup => lineup.Pg).ThenInclude(player => player.Team)
                .Include(lineup => lineup.Pg).ThenInclude(player => player.Stats)
                .Include(lineup => lineup.Sg).ThenInclude(player => player.Team)
                .Include(lineup => lineup.Sg).ThenInclude(player => player.Stats)
                .Include(lineup => lineup.Sf).ThenInclude(player => player.Team)
                .Include(lineup => lineup.Sf).ThenInclude(player => player.Stats)
                .Include(lineup => lineup.Pf).ThenInclude(player => player.Team)
                .Include(lineup => lineup.Pf).ThenInclude(player => player.Stats)
                .Include(lineup => lineup.C).ThenInclude(player => player.Team)
                .Include(lineup => lineup.C).ThenInclude(player => player.Stats)
                .Where(lineup => lineup.IsCalculated)
                .Where(lineup => type != LeaderboardType.DAILY || lineup.Date.Equals(dateTime.Date))
                .Where(lineup =>
                    type != LeaderboardType.WEEKLY || lineup.Date >= firstDayOfWeek && lineup.Date <= lastDayOfWeek)
                .Where(lineup =>
                    type != LeaderboardType.MONTHLY || lineup.Date >= CommonFunctions.Instance.GetLeaderboardDate(type))
                .Where(lineup => type != LeaderboardType.FROM_DATE || lineup.Date >= dateTime)
                .OrderByDescending(lineup => lineup.FP)
                .ToList();
        }

        // TODO update to use refactored user leaderboard query base
        public List<UserLeaderboardRecordDto> GetFriendsLeaderboard(string id, int from, int limit,
            LeaderboardType type,
            string date, int weekNumber, int year)
        {
            GameContext context = new GameContext();
            DateTime previousECT = CommonFunctions.Instance.UTCToEastern(RuntimeUtils.PREVIOUS_GAME);

            DateTime dateTime = date.Length == 0
                ? DateTime.UtcNow < RuntimeUtils.PREVIOUS_LAST_GAME
                    ? previousECT.AddDays(-1)
                    : previousECT
                : DateTime.ParseExact(date, "yyyyMMdd", CultureInfo.InvariantCulture);

            var loggedInUser = context.Users.Where(u => u.Id.Equals(id));
            User user = loggedInUser.FirstOrDefault();
            string loggedInUserId = user != null ? user.Id : "";

            var friendsOnly = context.FriendRequests
                .Include(u => u.Sender)
                .Where(f => f.ReceiverID.Equals(loggedInUserId) && f.Status == RequestStatus.ACCEPTED)
                .Select(u => u.Sender)
                .Union(context.FriendRequests
                    .Include(u => u.Receiver)
                    .Where(f => f.SenderID.Equals(loggedInUserId) && f.Status == RequestStatus.ACCEPTED)
                    .Select(u => u.Receiver))
                .Concat(loggedInUser);
            switch (type)
            {
                case LeaderboardType.DAILY:
                    var dailyStats = context.Stats.Where(stats => stats.Date.Equals(dateTime.Date));

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
                            Lineup = context.Players
                                .Include(player => player.Team)
                                .Where(player =>
                                    player.PlayerID == lineup.PgID
                                    || player.PlayerID == lineup.SgID
                                    || player.PlayerID == lineup.SfID
                                    || player.PlayerID == lineup.PfID
                                    || player.PlayerID == lineup.CID)
                                .Select(player => new LineupPlayerDto
                                {
                                    Player = player,
                                    TeamColor = player.Team.Color,
                                    FP = dailyStats.FirstOrDefault(stats => stats.PlayerID == player.PlayerID).FP,
                                    Price = player.Price
                                })
                                .OrderBy(p =>
                                    CommonFunctions.Instance.LineupPositionsOrder.IndexOf(p.Player.Position))
                                .ToList()
                        })
                        .Where(x => x.Lineup.Any())
                        .OrderByDescending(x => x.FP)
                        .Skip(from)
                        .Take(limit)
                        .ToList();
                case LeaderboardType.WEEKLY:
                    int week = weekNumber == NoData
                        ? CommonFunctions.Instance.GetIso8601WeekOfYear(
                            CommonFunctions.Instance.UTCToEastern(DateTime.UtcNow))
                        : weekNumber;
                    int leaderboardYear = year == NoData ? DateTime.Now.Year : year;
                    return friendsOnly
                        .SelectMany(user => user.UserLineups)
                        .Include(lineup => lineup.User)
                        .AsEnumerable()
                        .Where(lineup => lineup.IsCalculated
                                         && CommonFunctions.Instance.GetIso8601WeekOfYear(lineup.Date) == week
                                         && lineup.Date.Year == leaderboardYear)
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
                                .Where(lineup =>
                                    lineup.Date >= CommonFunctions.Instance.GetLeaderboardDate(type) &&
                                    lineup.IsCalculated)
                                .Select(lineup => lineup.FP).Sum(), 1),
                            GamesPlayed = user.UserLineups
                                .Count(lineup =>
                                    lineup.Date >= CommonFunctions.Instance.GetLeaderboardDate(type) &&
                                    lineup.IsCalculated)
                        })
                        .Where(user => user.GamesPlayed > 0)
                        .OrderByDescending(lineup => lineup.FP)
                        .Skip(from)
                        .Take(limit)
                        .ToList();
            }
        }

        public List<UserLeaderboardRecordDto> GetSeasonLineups(int from, int limit, int year)
        {
            GameContext context = new GameContext();
            DateTime seasonStart = DateTime.MinValue;
            DateTime seasonEnd = DateTime.MaxValue;
            if (year != NoData)
            {
                seasonStart = new DateTime(year, 10, 1);
                seasonEnd = new DateTime(year + 1, 7, 1);
            }

            return context.UserLineups
                .Where(lineup => lineup.Date >= seasonStart && lineup.Date <= seasonEnd)
                .OrderByDescending(lineup => lineup.FP)
                .Take(10)
                .Include(lineup => lineup.User)
                .Include(lineup => lineup.Pg).ThenInclude(player => player.Team)
                .Include(lineup => lineup.Pg).ThenInclude(player => player.Stats)
                .Include(lineup => lineup.Sg).ThenInclude(player => player.Team)
                .Include(lineup => lineup.Sg).ThenInclude(player => player.Stats)
                .Include(lineup => lineup.Sf).ThenInclude(player => player.Team)
                .Include(lineup => lineup.Sf).ThenInclude(player => player.Stats)
                .Include(lineup => lineup.Pf).ThenInclude(player => player.Team)
                .Include(lineup => lineup.Pf).ThenInclude(player => player.Stats)
                .Include(lineup => lineup.C).ThenInclude(player => player.Team)
                .Include(lineup => lineup.C).ThenInclude(player => player.Stats)
                .AsEnumerable()
                .Select(lineup => new UserLeaderboardRecordDto
                {
                    UserId = lineup.UserID,
                    Username = lineup.User.UserName,
                    AvatarUrl = lineup.User.AvatarURL,
                    LongDate = lineup.Date.ToString("yyyy-MM-dd"),
                    ShortDate = lineup.Date.ToString($"MMM. dd{(year == NoData ? ", yyy" : "")}"),
                    Date = lineup.Date,
                    FP = lineup.FP,
                    Lineup = new List<Player> {lineup.Pg, lineup.Sg, lineup.Sf, lineup.Pf, lineup.C}
                        .Select(player => new LineupPlayerDto
                        {
                            Player = player,
                            TeamColor = player.Team.Color,
                            FP = player.Stats.Where(stats => stats.Date.Equals(lineup.Date))
                                .Select(stats => stats.FP)
                                .FirstOrDefault(),
                            Price = player.Price
                        })
                        .OrderBy(lineupPlayer =>
                            CommonFunctions.Instance.LineupPositionsOrder.IndexOf(lineupPlayer.Player.Position,
                                StringComparison.Ordinal))
                        .ToList()
                }).Skip(from).Take(limit).ToList();
        }

        public List<PlayerLeaderboardRecordDto> GetSeasonPlayers(int from, int limit, int year)
        {
            DateTime seasonStart = DateTime.MinValue;
            DateTime seasonEnd = DateTime.MaxValue;
            if (year != NoData)
            {
                seasonStart = new DateTime(year, 10, 1);
                seasonEnd = new DateTime(year + 1, 7, 1);
            }

            return new GameContext().Stats
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
                    ShortDate = p.Date.ToString($"MMM. dd{(year == NoData ? ", yyy" : "")}")
                })
                .Skip(from)
                .Take(limit)
                .ToList();
        }

        public List<PlayerLeaderboardRecordDto> GetMostSelectedPlayers(int from, int limit)
        {
            var userLineups = new GameContext().UserLineups
                .Where(lineup => lineup.Date < RuntimeUtils.PREVIOUS_GAME);
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
                .Take(limit)
                .ToList();
        }
    }
}