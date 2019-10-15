using System;
using System.Collections.Generic;
using System.Linq;
using fantasy_hoops.Database;
using fantasy_hoops.Helpers;
using fantasy_hoops.Models;
using fantasy_hoops.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace fantasy_hoops.Repositories
{
    public class LineupRepository : ILineupRepository
    {

        private readonly GameContext _context;

        public LineupRepository(GameContext context)
        {
            _context = context;
        }

        public object GetLineup(string id)
        {
            return _context.UserLineups
            .Where(lineup => lineup.UserID.Equals(id) && lineup.Date.Equals(CommonFunctions.UTCToEastern(NextGame.NEXT_GAME).Date))
            .Select(lineup => new
            {
                lineup = _context.Players
                    .Where(player =>
                        player.PlayerID == lineup.PgID
                        || player.PlayerID == lineup.SgID
                        || player.PlayerID == lineup.SfID
                        || player.PlayerID == lineup.PfID
                        || player.PlayerID == lineup.CID)
                    .Select(player => new
                    {
                        id = player.NbaID,
                        fullName = player.FullName,
                        firstName = player.FirstName,
                        lastName = player.LastName,
                        abbrName = player.AbbrName,
                        position = player.Position,
                        price = player.Price,
                        fppg = player.FPPG,
                        playerId = player.PlayerID,
                        teamColor = player.Team.Color
                    })
            })
            .FirstOrDefault();
        }

        public void AddLineup(SubmitLineupViewModel model)
        {
            _context.UserLineups.Add(
                    new UserLineup
                    {
                        Date = CommonFunctions.UTCToEastern(NextGame.NEXT_GAME).Date,
                        UserID = model.UserID,
                        PgID = model.PgID,
                        SgID = model.SgID,
                        SfID = model.SfID,
                        PfID = model.PfID,
                        CID = model.CID
                    });
        }

        public void UpdateLineup(SubmitLineupViewModel model)
        {
            var userLineup = _context.UserLineups
                    .Where(lineup => lineup.UserID.Equals(model.UserID)
                                    && lineup.Date.Equals(CommonFunctions.UTCToEastern(NextGame.NEXT_GAME).Date)).FirstOrDefault();
            userLineup.PgID = model.PgID;
            userLineup.SgID = model.SgID;
            userLineup.SfID = model.SfID;
            userLineup.PfID = model.PfID;
            userLineup.CID = model.CID;
            userLineup.FP = 0.0;
            userLineup.IsCalculated = false;
        }

        public int GetLineupPrice(SubmitLineupViewModel model)
        {
            return _context.Players.Where(pg => pg.PlayerID == model.PgID).Select(p => p.Price)
                    .Union(_context.Players.Where(sg => sg.PlayerID == model.SgID).Select(p => p.Price))
                    .Union(_context.Players.Where(sf => sf.PlayerID == model.SfID).Select(p => p.Price))
                    .Union(_context.Players.Where(pf => pf.PlayerID == model.PfID).Select(p => p.Price))
                    .Union(_context.Players.Where(c => c.PlayerID == model.CID).Select(p => p.Price))
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
                    .Where(x => x.UserID.Equals(userID)
                        && x.Date.Equals(CommonFunctions.UTCToEastern(NextGame.NEXT_GAME).Date))
                    .Any();
        }

        private bool IsPlayerPriceIncorrect(int playerID, int price)
        {
            return _context.Players.Where(pg => pg.PlayerID == playerID).Select(p => p.Price).FirstOrDefault() != price;
        }

        public IEnumerable<string> GetUserSelectedIds()
        {
            return _context.UserLineups
                .Where(lineup => lineup.Date.Date.Equals(CommonFunctions.UTCToEastern(NextGame.NEXT_GAME).Date))
                .Select(lineup => lineup.UserID);
        }

        public IEnumerable<User> UsersNotSelected(IEnumerable<string> usersSelectedIDs)
        {
            return _context.Users
                .Where(user => user.Streak > 0 && !usersSelectedIDs.Any(userID => userID.Equals(user.Id)))
                .ToList();
        }
    }
}
