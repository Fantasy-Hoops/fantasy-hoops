using System.Net;
using System.Linq;
using System.Threading.Tasks;
using System;
using Newtonsoft.Json.Linq;
using fantasy_hoops.Models;
using fantasy_hoops.Helpers;
using fantasy_hoops.Services;
using FluentScheduler;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace fantasy_hoops.Database
{
	public class PlayerSeed : IJob
    {
        private readonly GameContext _context;
        private readonly IScoreService _scoreService;
        private readonly bool _updatePrice;

        public static DateTime PLAYER_POOL_DATE = DateTime.UtcNow;
		public static int PRICE_FLOOR = 10;

        public PlayerSeed(IScoreService scoreService, bool updatePrice)
        {
            _context = new GameContext();
            _scoreService = scoreService;
            _updatePrice = updatePrice;
        }

		private JObject GetPlayer(int id)
		{
			string url = "http://data.nba.net/v2015/json/mobile_teams/nba/" + CommonFunctions.SEASON_YEAR + "/players/playercard_" + id + "_02.json";
			HttpWebResponse webResponse = CommonFunctions.GetResponse(url);
			if (webResponse == null)
				return null;
			string apiResponse = CommonFunctions.ResponseToString(webResponse);
			JObject json = JObject.Parse(apiResponse);
			return json;
		}

		private bool IsPlaying(Player player)
		{
            if(player.IsInGLeague || player.Position.Equals("NA"))
            {
                return false;
            }
            
            if(player.InjuryID != 0)
            {
                if(player.Injury == null)
                {
                    player.Injury = _context.Injuries.Where(inj => inj.InjuryID == player.InjuryID).FirstOrDefault();
                }

                if ((player.Injury.Date.HasValue && player.Injury.Date.Value.AddDays(5) < NextGame.NEXT_GAME)
                    && (player.Injury.Status.ToLower().Contains("out")
                    || player.Injury.Status.ToLower().Contains("injured")))
                {
                    return false;
                }
            }

            return true;
		}

		private string GetDate()
		{
			return CommonFunctions.UTCToEastern(NextGame.NEXT_GAME).ToString("yyyyMMdd");
		}

		private double FPPG(Player p)
		{
			return Math.Round((1 * p.PTS) + (1.2 * p.REB) + (1.5 * p.AST) + (3 * p.STL) + (3 * p.BLK) - (1 * p.TOV), 2);
		}

        private int Price(Player p)
		{
			int price = _scoreService.GetPrice(p);
			if (price < PRICE_FLOOR)
				return PRICE_FLOOR;
			return price;
		}

		private void SetNextOpponent(JToken game)
		{
			Team hTeam = _context.Teams.Where(team => team.NbaID == (int)game["hTeam"]["teamId"]).FirstOrDefault();
			Team vTeam = _context.Teams.Where(team => team.NbaID == (int)game["vTeam"]["teamId"]).FirstOrDefault();

			if (vTeam != null && hTeam != null)
			{
				hTeam.NextOpponentID = vTeam.TeamID;
				vTeam.NextOpponentID = hTeam.TeamID;
				hTeam.NextOppFormatted = string.Format("vs {0}", game["vTeam"]["triCode"]);
				vTeam.NextOppFormatted = string.Format("@ {0}", game["hTeam"]["triCode"]);
			}
		}

        public void Execute()
        {
            _context.Players.ForEachAsync(p => p.IsPlaying = false).Wait();
            string date = GetDate();
            JArray games = CommonFunctions.GetGames(date);
            foreach (var game in games)
            {
                SetNextOpponent(game);
                var hTeamPlayers = _context.Players.Where(p => p.Team.NbaID == (int)game["hTeam"]["teamId"]).ToList();
                var vTeamPlayers = _context.Players.Where(p => p.Team.NbaID == (int)game["vTeam"]["teamId"]).ToList();

                foreach (var player in hTeamPlayers.Union(vTeamPlayers))
                {
                    JObject p = GetPlayer(player.NbaID);
                    if (p == null)
                    {
                        player.Price = PRICE_FLOOR;
                        continue;
                    }
                    int gamesPlayed = 0;
                    JToken stats = null;
                    if (!(p["pl"]["ca"] == null || p["pl"]["ca"]["sa"] == null))
                    {
                        stats = p["pl"]["ca"]["sa"].Last;
                        gamesPlayed = (int)stats["gp"];
                    }
                    player.PTS = gamesPlayed <= 0 ? 0 : (double)stats["pts"];
                    player.REB = gamesPlayed <= 0 ? 0 : (double)stats["reb"];
                    player.AST = gamesPlayed <= 0 ? 0 : (double)stats["ast"];
                    player.STL = gamesPlayed <= 0 ? 0 : (double)stats["stl"];
                    player.BLK = gamesPlayed <= 0 ? 0 : (double)stats["blk"];
                    player.TOV = gamesPlayed <= 0 ? 0 : (double)stats["tov"];
                    player.GP = gamesPlayed;
                    player.FPPG = gamesPlayed <= 0 ? 0 : FPPG(player);
                    if (_updatePrice)
                        player.Price = gamesPlayed <= 0 ? PRICE_FLOOR : Price(player);
                    player.IsPlaying = IsPlaying(player);
                }
            }
            _context.SaveChanges();

            NextGame.NEXT_GAME_CLIENT = NextGame.NEXT_GAME;
            PLAYER_POOL_DATE = NextGame.NEXT_GAME;
        }
    }
}