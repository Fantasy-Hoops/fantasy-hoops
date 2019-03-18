using System.Net;
using System.Linq;
using System;
using Newtonsoft.Json.Linq;
using fantasy_hoops.Models;
using fantasy_hoops.Helpers;
using FluentScheduler;
using fantasy_hoops.Services;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace fantasy_hoops.Database
{
	public class StatsSeed
	{

		private static ScoreService _scoreService;

		public static void Initialize(GameContext context)
		{
			if (JobManager.RunningSchedules.Any(s => !s.Name.Equals("statsSeed")))
			{
				JobManager.AddJob(() => Initialize(context),
				s => s.WithName("statsSeed")
				.ToRunOnceIn(30)
				.Seconds());
				return;
			}

			_scoreService = new ScoreService();
			Task.Run(() => Calculate(context)).Wait();
		}

		private static JObject GetBoxscore(string url)
		{
			HttpWebResponse webResponse = CommonFunctions.GetResponse(url);
			string apiResponse = CommonFunctions.ResponseToString(webResponse);
			JObject json = JObject.Parse(apiResponse);
			return json;
		}

		private static bool IsFinished(GameContext context, JArray games)
		{
			if (games.Any(g => (int)g["statusNum"] != 3 && (bool)g["isGameActivated"]))
			{
				JobManager.AddJob(() => Initialize(context),
				s => s.WithName("statsSeed")
				.ToRunOnceIn(10)
				.Minutes());
				return false;
			}
			return true;
		}

		private static async Task Calculate(GameContext context)
		{
			string gameDate = CommonFunctions.UTCToEastern(NextGame.PREVIOUS_GAME).ToString("yyyyMMdd");
			JArray games = CommonFunctions.GetGames(gameDate);

			if (!IsFinished(context, games))
				return;

			foreach (JObject game in games)
			{
				string bsUrl = "http://data.nba.net/10s/prod/v1/" + gameDate + "/" + game["gameId"] + "_boxscore.json";
				JObject boxscore = GetBoxscore(bsUrl);
				if (boxscore["stats"] == null)
					continue;
				int hTeam = (int)boxscore["basicGameData"]["hTeam"]["teamId"];
				int vTeam = (int)boxscore["basicGameData"]["vTeam"]["teamId"];
				DateTime date = CommonFunctions.UTCToEastern(DateTime.Parse((string)boxscore["basicGameData"]["startTimeUTC"]));
				var players = boxscore["stats"]["activePlayers"];
				foreach (var player in (JArray)players)
				{
					int oppId;
					string score = "";
					if (!await context.Players.AnyAsync(x => x.NbaID.Equals((int)player["personId"])))
						continue;
					if ((int)player["teamId"] == hTeam)
					{
						oppId = vTeam;
						score = (int)boxscore["basicGameData"]["vTeam"]["score"] + "-" + (int)boxscore["basicGameData"]["hTeam"]["score"] + ";vs";
					}
					else
					{
						oppId = hTeam;
						score = (int)boxscore["basicGameData"]["hTeam"]["score"] + "-" + (int)boxscore["basicGameData"]["vTeam"]["score"] + ";@";
					}
					if ((string)player["min"] == null || ((string)player["min"]).Length == 0)
						continue;
					await AddToDatabaseAsync(context, player, date, oppId, score);
				}
			}
			await context.SaveChangesAsync();

			JobManager.AddJob(() => UserScoreSeed.Initialize(context),
					s => s.WithName("userScore")
					.ToRunNow());
		}

		private static async Task AddToDatabaseAsync(GameContext context, JToken player, DateTime date, int oppId, string score)
		{
			Stats statsObj = new Stats
			{
				Date = date,
				OppID = oppId,
				Score = score,
				MIN = (string)player["min"],
				FGM = (int)player["fgm"],
				FGA = (int)player["fga"],
				FGP = (double)player["fgp"],
				TPM = (int)player["tpm"],
				TPA = (int)player["tpa"],
				TPP = (double)player["tpp"],
				FTM = (int)player["ftm"],
				FTA = (int)player["fta"],
				FTP = (double)player["ftp"],
				DREB = (int)player["defReb"],
				OREB = (int)player["offReb"],
				TREB = (int)player["totReb"],
				AST = (int)player["assists"],
				BLK = (int)player["blocks"],
				STL = (int)player["steals"],
				FLS = (int)player["pFouls"],
				TOV = (int)player["turnovers"],
				PTS = (int)player["points"]
			};

			statsObj.Player = context.Players.Where(x => x.NbaID == (int)player["personId"]).FirstOrDefault();
			var sth = statsObj.Player.PlayerID;
			bool shouldAdd = !await context.Stats.AnyAsync(x => x.Date.Equals(date) && x.PlayerID == statsObj.Player.PlayerID);

			if (shouldAdd)
			{
				statsObj.GS = _scoreService.GetGameScore(statsObj.PTS, statsObj.FGM, statsObj.DREB, statsObj.OREB,
						statsObj.STL, statsObj.AST, statsObj.BLK, statsObj.FGA, statsObj.FTA - statsObj.FTM,
						statsObj.FLS, statsObj.TOV);

				statsObj.FP = _scoreService.GetFantasyPoints(statsObj.PTS, statsObj.DREB, statsObj.OREB,
						statsObj.AST, statsObj.STL, statsObj.BLK, statsObj.TOV);

				statsObj.Price = _scoreService.GetPrice(statsObj.Player);
				await context.Stats.AddAsync(statsObj);
			}
		}
	}
}