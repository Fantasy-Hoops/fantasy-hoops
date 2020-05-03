using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using fantasy_hoops.Database;
using fantasy_hoops.Dtos;
using fantasy_hoops.Helpers;
using fantasy_hoops.Models;
using fantasy_hoops.Models.ViewModels;
using fantasy_hoops.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace fantasy_hoops.Repositories
{
    public class LineupRepository : ILineupRepository
    {
        private readonly GameContext _context;

        public LineupRepository(GameContext context = null)
        {
            _context = context ?? new GameContext();
        }

        public UserLineup GetLineup(string id)
        {
            return _context.UserLineups
                .FirstOrDefault(lineup =>
                    lineup.UserID.Equals(id) &&
                    lineup.Date.Equals(CommonFunctions.Instance.UTCToEastern(RuntimeUtils.NEXT_GAME).Date));
        }

        public void AddLineup(SubmitLineupViewModel model)
        {
            _context.UserLineups.Add(
                new UserLineup
                {
                    Date = CommonFunctions.Instance.UTCToEastern(RuntimeUtils.NEXT_GAME).Date,
                    UserID = model.UserID,
                    PgID = model.PgID,
                    SgID = model.SgID,
                    SfID = model.SfID,
                    PfID = model.PfID,
                    CID = model.CID
                });
            _context.SaveChanges();
        }

        public void UpdateLineup(SubmitLineupViewModel model)
        {
            var userLineup = _context.UserLineups
                .FirstOrDefault(lineup => lineup.UserID.Equals(model.UserID)
                                          && lineup.Date.Equals(CommonFunctions.Instance
                                              .UTCToEastern(RuntimeUtils.NEXT_GAME).Date));
            if (userLineup == null)
            {
                return;
            }

            userLineup.PgID = model.PgID;
            userLineup.SgID = model.SgID;
            userLineup.SfID = model.SfID;
            userLineup.PfID = model.PfID;
            userLineup.CID = model.CID;
            userLineup.FP = 0.0;
            userLineup.IsCalculated = false;
            _context.SaveChanges();
        }

        public int GetLineupPrice(SubmitLineupViewModel model)
        {
            return _context.Players.Where(player => player.PlayerID == model.PgID
                                                    || player.PlayerID == model.SgID
                                                    || player.PlayerID == model.SfID
                                                    || player.PlayerID == model.PfID
                                                    || player.PlayerID == model.CID)
                .Select(p => p.Price)
                .Sum();
        }

        public bool ArePricesCorrect(SubmitLineupViewModel model)
        {
            return !(IsPlayerPriceIncorrect(model.PgID, model.PgPrice)
                     || IsPlayerPriceIncorrect(model.SgID, model.SgPrice)
                     || IsPlayerPriceIncorrect(model.SfID, model.SfPrice)
                     || IsPlayerPriceIncorrect(model.PfID, model.PfPrice)
                     || IsPlayerPriceIncorrect(model.CID, model.CPrice));
        }

        public bool AreNotPlayingPlayers(SubmitLineupViewModel model)
        {
            return _context.Players
                .Where(player => player.PlayerID == model.PgID
                                 || player.PlayerID == model.SgID
                                 || player.PlayerID == model.SfID
                                 || player.PlayerID == model.PfID
                                 || player.PlayerID == model.CID)
                .Any(player => !player.IsPlaying);
        }

        public bool IsUpdating(String userID)
        {
            return _context.UserLineups
                .Any(x => x.UserID.Equals(userID)
                          && x.Date.Equals(CommonFunctions.Instance.UTCToEastern(RuntimeUtils.NEXT_GAME).Date));
        }

        private bool IsPlayerPriceIncorrect(int playerID, int price)
        {
            return _context.Players.Where(pg => pg.PlayerID == playerID).Select(p => p.Price).FirstOrDefault() != price;
        }

        public List<string> GetUserSelectedIds()
        {
            return _context.UserLineups
                .Where(lineup =>
                    lineup.Date.Date.Equals(CommonFunctions.Instance.UTCToEastern(RuntimeUtils.NEXT_GAME).Date))
                .Select(lineup => lineup.UserID)
                .ToList();
        }

        public List<User> UsersNotSelected()
        {
            List<string> usersSelectedIDs = GetUserSelectedIds();
            return _context.Users
                .Where(user => user.Streak > 0 && !usersSelectedIDs.Any(userID => userID.Equals(user.Id)))
                .ToList();
        }

        public UserLeaderboardRecordDto GetUserCurrentLineup(string userId)
        {
            return _context.UserLineups
                .Where(lineup =>
                    lineup.UserID.Equals(userId) && (lineup.Date.Date ==
                                                     CommonFunctions.Instance.UTCToEastern(RuntimeUtils.NEXT_GAME).Date
                                                     || lineup.Date.Date == CommonFunctions.Instance
                                                         .UTCToEastern(RuntimeUtils.PREVIOUS_GAME).Date)
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
                        .Select(player => new LineupPlayerDto
                        {
                            Player = player,
                            TeamColor = player.Team.Color,
                            FP = _context.Stats.Where(stats => stats.Date.Date == lineup.Date.Date
                                                               && stats.PlayerID == player.PlayerID)
                                .Select(stats => stats.FP).FirstOrDefault(),
                            Price = player.Price
                        })
                        .OrderBy(p => CommonFunctions.Instance.LineupPositionsOrder.IndexOf(p.Player.Position))
                        .ToList(),
                    IsLive = lineup.Date.Equals(CommonFunctions.Instance.UTCToEastern(RuntimeUtils.PREVIOUS_GAME).Date)
                             && !lineup.IsCalculated
                })
                .FirstOrDefault();
        }

        public List<UserLeaderboardRecordDto> GetRecentLineups(string userId, int start, int count)
        {
            return _context.UserLineups
                .Where(lineup => lineup.IsCalculated && lineup.UserID.Equals(userId))
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
                        .Select(player => new LineupPlayerDto
                        {
                            Player = player,
                            TeamColor = player.Team.Color,
                            FP = _context.Stats.Where(stats => stats.Date.Date == lineup.Date.Date
                                                               && stats.PlayerID == player.PlayerID)
                                .Select(stats => stats.FP).FirstOrDefault(),
                            Price = player.Price
                        })
                        .OrderBy(p => CommonFunctions.Instance.LineupPositionsOrder.IndexOf(p.Player.Position))
                        .ToList()
                })
                .ToList();
        }

        public List<BestLineupDto> GetBestLineups(string date, int from, int limit)
        {
            DateTime dateTime;
            if (date != null && date.Length == 8)
            {
                dateTime = DateTime.ParseExact(date, "yyyyMMdd", CultureInfo.InvariantCulture);
            }
            else
            {
                dateTime = _context.BestLineups.Max(lineup => lineup.Date);
            }

            return _context.BestLineups
                .Where(lineup => lineup.Date.Equals(dateTime))
                .Include(lineup => lineup.Lineup)
                .ThenInclude(lineup => lineup.Player)
                .ThenInclude(player => player.Team)
                .Select(lineup => new BestLineupDto
                {
                    Date = lineup.Date,
                    Lineup = lineup.Lineup
                        .Select(l => new LineupPlayerDto
                        {
                            Player = l.Player,
                            TeamColor = l.Player.Team.Color,
                            FP = l.FP,
                            Price = l.Price
                        })
                        .OrderBy(p => CommonFunctions.Instance.LineupPositionsOrder.IndexOf(p.Player.Position))
                        .ToList(),
                    FP = lineup.TotalFP,
                    Price = lineup.LineupPrice
                })
                .OrderByDescending(lineup => lineup.FP)
                .Skip(from)
                .Take(limit)
                .ToList();
        }
    }
}