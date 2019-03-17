using System;
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
					.Where(lineup => lineup.UserID.Equals(id)
									&& lineup.Date.Equals(CommonFunctions.UTCToEastern(NextGame.NEXT_GAME)))
					.Select(lineup => new
					{
						pg = new { player = lineup.Pg, teamColor = lineup.Pg.Team.Color },
						sg = new { player = lineup.Sg, teamColor = lineup.Sg.Team.Color },
						sf = new { player = lineup.Sf, teamColor = lineup.Sf.Team.Color },
						pf = new { player = lineup.Pf, teamColor = lineup.Pf.Team.Color },
						c = new { player = lineup.C, teamColor = lineup.C.Team.Color }
					})
					.FirstOrDefault();
		}

		// REMOVE
		public void AddPlayer(String userID, String position, int playerID)
		{
			var player = new Lineup
			{
				UserID = userID,
				PlayerID = playerID,
				Position = position,
				Date = CommonFunctions.UTCToEastern(NextGame.NEXT_GAME),
				Calculated = false
			};
			_context.Lineups.Add(player);
		}

		// REMOVE
		public void UpdatePlayer(String userID, String position, int playerID)
		{
			var player = _context.Lineups
							.Where(x => x.UserID.Equals(userID)
											&& x.Position.Equals(position)
											&& x.Date == CommonFunctions.UTCToEastern(NextGame.NEXT_GAME))
							.FirstOrDefault();

			player.PlayerID = playerID;
			player.FP = 0.0;
			player.Calculated = false;
		}

		public void AddLineup(SubmitLineupViewModel model)
		{
			_context.UserLineups.Add(
					new UserLineup
					{
						Date = CommonFunctions.UTCToEastern(NextGame.NEXT_GAME),
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
									&& lineup.Date.Equals(CommonFunctions.UTCToEastern(NextGame.NEXT_GAME))).FirstOrDefault();
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

		public bool IsUpdating(String userID)
		{
			// REMOVE AFTER FULL LINEUPS CHANGE
			return (_context.UserLineups
					.Where(x => x.UserID.Equals(userID)
									&& x.Date == CommonFunctions.UTCToEastern(NextGame.NEXT_GAME))
					.Any()
					||
					_context.Lineups
					.Where(x => x.UserID.Equals(userID)
									&& x.Date == CommonFunctions.UTCToEastern(NextGame.NEXT_GAME))
					.Any());
		}

		private bool IsPlayerPriceIncorrect(int playerID, int price)
		{
			return _context.Players.Where(pg => pg.PlayerID == playerID).Select(p => p.Price).FirstOrDefault() != price;
		}
	}
}
