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
    public class PlayerSeed
    {
        public static DateTime PLAYER_POOL_DATE = DateTime.UtcNow;
        public static int PRICE_FLOOR = 10;

        private static ScoreService _scoreService;

        public static void Initialize(GameContext context, bool updatePrice)
        {
            if (JobManager.RunningSchedules.Any(s => !s.Name.Equals("playerSeed")))
            {
                JobManager.AddJob(() => Initialize(context, updatePrice),
                s => s.WithName("playerSeed")
                .ToRunOnceIn(30)
                .Seconds());
                return;
            }

            _scoreService = new ScoreService();
            if (bool.Parse(Environment.GetEnvironmentVariable("IS_PRODUCTION")))
                Task.Run(() => Calculate(context, updatePrice)).Wait();
            NextGame.NEXT_GAME_CLIENT = NextGame.NEXT_GAME;
            PLAYER_POOL_DATE = NextGame.NEXT_GAME;
        }

        private static JObject GetPlayer(int id)
        {
            string url = "http://data.nba.net/v2015/json/mobile_teams/nba/" + CommonFunctions.GetSeasonYear() + "/players/playercard_" + id + "_02.json";
            HttpWebResponse webResponse = CommonFunctions.GetResponse(url);
            if (webResponse == null)
                return null;
            string apiResponse = CommonFunctions.ResponseToString(webResponse);
            JObject json = JObject.Parse(apiResponse);
            return json;
        }

        private static async Task Calculate(GameContext context, bool updatePrice)
        {
            await context.Players.ForEachAsync(p => p.IsPlaying = false);
            string date = GetDate();
            JArray games = CommonFunctions.GetGames(date);
            foreach (var game in games)
            {
                SetNextOpponent(context, game);
                var hTeamPlayers = context.Players.Where(p => p.Team.NbaID == (int)game["hTeam"]["teamId"]).ToList();
                var vTeamPlayers = context.Players.Where(p => p.Team.NbaID == (int)game["vTeam"]["teamId"]).ToList();

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
                    if (updatePrice)
                        player.Price = gamesPlayed <= 0 ? PRICE_FLOOR : Price(player);
                    player.IsPlaying = IsPlaying(player);
                }
            }
            await context.SaveChangesAsync();
        }

        private static bool IsPlaying(Player player)
        {
            // Don't show players out for more that 5 days
            if ((player.StatusDate.HasValue
                    && (player.StatusDate.Value.AddDays(5) < NextGame.NEXT_GAME)
                    && (player.Status.ToLower().Contains("out")
                     || player.Status.ToLower().Contains("injured")))
                    || player.IsInGLeague
                    || player.Position.Equals("NA"))
                return false;
            return true;
        }

        private static string GetDate()
        {
            return CommonFunctions.UTCToEastern(NextGame.NEXT_GAME).ToString("yyyyMMdd");
        }

        private static double FPPG(Player p)
        {
            return Math.Round((1 * p.PTS) + (1.2 * p.REB) + (1.5 * p.AST) + (3 * p.STL) + (3 * p.BLK) - (1 * p.TOV), 2);
        }

        public static int Price(Player p)
        {
            int price = _scoreService.GetPrice(p);
            if (price < PRICE_FLOOR)
                return PRICE_FLOOR;
            return price;
        }

        private static void SetNextOpponent(GameContext context, JToken game)
        {
            Team hTeam = context.Teams.Where(team => team.NbaID == (int)game["hTeam"]["teamId"]).FirstOrDefault();
            Team vTeam = context.Teams.Where(team => team.NbaID == (int)game["vTeam"]["teamId"]).FirstOrDefault();

            hTeam.NextOpponentID = vTeam.TeamID;
            hTeam.NextOppFormatted = string.Format("vs {0}", game["vTeam"]["triCode"]);

            vTeam.NextOpponentID = hTeam.TeamID;
            vTeam.NextOppFormatted = string.Format("@ {0}", game["hTeam"]["triCode"]);
        }
    }
}