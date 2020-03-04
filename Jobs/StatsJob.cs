using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using fantasy_hoops.Database;
using fantasy_hoops.Helpers;
using fantasy_hoops.Models;
using fantasy_hoops.Services.Interfaces;
using FluentScheduler;
using Newtonsoft.Json.Linq;

namespace fantasy_hoops.Jobs
{
    public class StatsJob : IJob
    {
        private readonly IScoreService _scoreService;
        private readonly IPushService _pushService;

        public StatsJob(IScoreService scoreService, IPushService pushService)
        {
            _scoreService = scoreService;
            _pushService = pushService;
        }

        public void Execute()
        {
            Task.Run(() => CalculateStats());
        }

        private JObject GetBoxscore(string url)
        {
            HttpWebResponse webResponse = CommonFunctions.GetResponse(url);
            string apiResponse = CommonFunctions.ResponseToString(webResponse);
            JObject json = JObject.Parse(apiResponse);
            return json;
        }

        private bool IsFinished(JArray games)
        {
            if (games.Any(g => (int) g["statusNum"] != 3 || (bool) g["isGameActivated"]))
            {
                JobManager.AddJob(new StatsJob(_scoreService, _pushService),
                    s => s.WithName("statsSeed")
                        .ToRunOnceIn(5)
                        .Minutes());
                return false;
            }

            return true;
        }

        private Player FindPlayer(JToken player)
        {
            return new GameContext().Players
                .FirstOrDefault(x => x.NbaID == (int) player["personId"]);
        }

        private Stats FindStats(Player player, DateTime date)
        {
            return new GameContext().Stats.FirstOrDefault(stats =>
                stats.Date.Equals(date) && stats.PlayerID == player.PlayerID);
        }

        private void AddToDatabase(GameContext context, JToken player, DateTime date, int oppId, string score)
        {
            Player statsPlayer = FindPlayer(player);
            if (statsPlayer == null)
            {
                return;
            }

            Stats dbStats = FindStats(statsPlayer, date);
            if (dbStats == null)
            {
                Stats statsObj = new Stats
                {
                    PlayerID = statsPlayer.PlayerID,
                    Date = date,
                    OppID = oppId,
                    Score = score,
                    MIN = (string) player["min"],
                    FGM = (int) player["fgm"],
                    FGA = (int) player["fga"],
                    FGP = (double) player["fgp"],
                    TPM = (int) player["tpm"],
                    TPA = (int) player["tpa"],
                    TPP = (double) player["tpp"],
                    FTM = (int) player["ftm"],
                    FTA = (int) player["fta"],
                    FTP = (double) player["ftp"],
                    DREB = (int) player["defReb"],
                    OREB = (int) player["offReb"],
                    TREB = (int) player["totReb"],
                    AST = (int) player["assists"],
                    BLK = (int) player["blocks"],
                    STL = (int) player["steals"],
                    FLS = (int) player["pFouls"],
                    TOV = (int) player["turnovers"],
                    PTS = (int) player["points"],
                    Price = statsPlayer.Price
                };

                statsObj.GS = _scoreService.GetGameScore(statsObj.PTS, statsObj.FGM, statsObj.DREB, statsObj.OREB,
                    statsObj.STL, statsObj.AST, statsObj.BLK, statsObj.FGA, statsObj.FTA - statsObj.FTM,
                    statsObj.FLS, statsObj.TOV);

                statsObj.FP = _scoreService.GetFantasyPoints(statsObj.PTS, statsObj.DREB, statsObj.OREB,
                    statsObj.AST, statsObj.STL, statsObj.BLK, statsObj.TOV);

                context.Stats.Add(statsObj);
            }
            else
            {
                dbStats.Score = score;
                dbStats.MIN = (string) player["min"];
                dbStats.FGM = (int) player["fgm"];
                dbStats.FGA = (int) player["fga"];
                dbStats.FGP = (double) player["fgp"];
                dbStats.TPM = (int) player["tpm"];
                dbStats.TPA = (int) player["tpa"];
                dbStats.TPP = (double) player["tpp"];
                dbStats.FTM = (int) player["ftm"];
                dbStats.FTA = (int) player["fta"];
                dbStats.FTP = (double) player["ftp"];
                dbStats.DREB = (int) player["defReb"];
                dbStats.OREB = (int) player["offReb"];
                dbStats.TREB = (int) player["totReb"];
                dbStats.AST = (int) player["assists"];
                dbStats.BLK = (int) player["blocks"];
                dbStats.STL = (int) player["steals"];
                dbStats.FLS = (int) player["pFouls"];
                dbStats.TOV = (int) player["turnovers"];
                dbStats.PTS = (int) player["points"];

                dbStats.GS = _scoreService.GetGameScore(dbStats.PTS, dbStats.FGM, dbStats.DREB, dbStats.OREB,
                    dbStats.STL, dbStats.AST, dbStats.BLK, dbStats.FGA, dbStats.FTA - dbStats.FTM,
                    dbStats.FLS, dbStats.TOV);

                dbStats.FP = _scoreService.GetFantasyPoints(dbStats.PTS, dbStats.DREB, dbStats.OREB,
                    dbStats.AST, dbStats.STL, dbStats.BLK, dbStats.TOV);
                
                context.Stats.Update(dbStats);
            }
        }

        private Game GetGame(DateTime date, int homeTeamId, int awayTeamId, int homeScore, int awayScore)
        {
            GameContext _context = new GameContext();
            Team homeTeam = _context.Teams.FirstOrDefault(team => team.NbaID == homeTeamId);
            if (homeTeam == null)
                homeTeam = CommonFunctions.GetUnknownTeam(_context);
            Team awayTeam = _context.Teams.FirstOrDefault(team => team.NbaID == awayTeamId);
            if (awayTeam == null)
                awayTeam = CommonFunctions.GetUnknownTeam(_context);

            Game gameObj = _context.Games.FirstOrDefault(game => game.Date.Equals(date)
                                                                 && game.HomeTeam.Equals(homeTeam)
                                                                 && game.AwayTeam.Equals(awayTeam));

            if (gameObj != null)
            {
                gameObj.HomeScore = homeScore;
                gameObj.AwayScore = awayScore;
            }
            else
            {
                gameObj = new Game
                {
                    Date = date,
                    HomeTeam = homeTeam,
                    HomeScore = homeScore,
                    AwayTeam = awayTeam,
                    AwayScore = awayScore
                };
                _context.Games.Add(gameObj);
            }

            return gameObj;
        }

        private void CalculateStats()
        {
            string gameDate = CommonFunctions.UTCToEastern(NextGameJob.PREVIOUS_GAME).ToString("yyyyMMdd");
            JArray games = CommonFunctions.GetGames(gameDate);

            int countOfActivatedGames = 0;
            bool isAnyGameStarted = false;
            foreach (var game in games)
            {
                if ((int) game["statusNum"] != 1)
                {
                    isAnyGameStarted = true;
                }

                if (!(bool) game["isGameActivated"] && (int) game["statusNum"] != 3)
                {
                    continue;
                }

                countOfActivatedGames++;

                string bsUrl = "http://data.nba.net/10s/prod/v1/" + gameDate + "/" + game["gameId"] + "_boxscore.json";
                JObject boxscore = GetBoxscore(bsUrl);
                if (boxscore["stats"] == null)
                {
                    continue;
                }

                int hTeam = (int) boxscore["basicGameData"]["hTeam"]["teamId"];
                int vTeam = (int) boxscore["basicGameData"]["vTeam"]["teamId"];

                DateTime date = DateTime.ParseExact((string) boxscore["basicGameData"]["startDateEastern"], "yyyyMMdd",
                    CultureInfo.InvariantCulture);
                var players = boxscore["stats"]["activePlayers"];

// TODO will be implemented later
                // int homeScore = (int) boxscore["basicGameData"]["hTeam"]["score"];
                // int awayScore = (int) boxscore["basicGameData"]["vTeam"]["score"];
                // Game gameObj = GetGame(date, hTeam, vTeam, homeScore, awayScore);

                GameContext context = new GameContext();
                foreach (var player in (JArray) players)
                {
                    if (player["min"] == null || ((string) player["min"]).Length == 0)
                    {
                        continue;
                    }

                    int oppId;
                    string score;
                    string liveToken = (int) game["statusNum"] != 3 ? ";LIVE" : "";
                    if ((int) player["teamId"] == hTeam)
                    {
                        oppId = vTeam;
                        score = (int) boxscore["basicGameData"]["vTeam"]["score"] + "-" +
                                (int) boxscore["basicGameData"]["hTeam"]["score"] + ";vs" + liveToken;
                    }
                    else
                    {
                        oppId = hTeam;
                        score = (int) boxscore["basicGameData"]["vTeam"]["score"] + "-" +
                                (int) boxscore["basicGameData"]["hTeam"]["score"] + ";@" + liveToken;
                    }

                    AddToDatabase(context, player, date, oppId, score);
                }
                context.SaveChanges();
            }

            if (!isAnyGameStarted)
            {
                JobManager.AddJob(new StatsJob(_scoreService, _pushService),
                    s => s.WithName("statsSeed")
                    .ToRunOnceIn(5).Minutes());
                return;
            }

            if (!IsFinished(games))
            {
                int minutesDelay = countOfActivatedGames == 0 ? 5 : 1;
                JobManager.AddJob(new StatsJob(_scoreService, _pushService),
                        s => s.WithName("statsSeed")
                            .ToRunOnceIn(minutesDelay).Minutes());
            }
            else
            {
                GameContext context = new GameContext();
                context.Stats
                    .RemoveRange(context.Stats.Where(stat => stat.Score.Contains("LIVE")));
                context.SaveChanges();

                Task.Run(() => new UserScoreJob(_pushService).Execute());

                JobManager.AddJob(new StatsJob(_scoreService, _pushService),
                        s => s.WithName("statsSeed")
                    .ToRunOnceAt(NextGameJob.NEXT_GAME.AddMinutes(5)));
            }
        }
    }
}