using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using fantasy_hoops.Database;
using fantasy_hoops.Helpers;
using fantasy_hoops.Models;
using fantasy_hoops.Repositories;
using fantasy_hoops.Repositories.Interfaces;
using fantasy_hoops.Services.Interfaces;
using FluentScheduler;
using Newtonsoft.Json.Linq;

namespace fantasy_hoops.Jobs
{
    public class StatsJob : IJob
    {
        private readonly GameContext _context;
        private readonly IScoreService _scoreService;
        private readonly IPushService _pushService;
        private readonly ITeamRepository _teamRepository;

        public StatsJob(IScoreService scoreService, IPushService pushService)
        {
            _context = new GameContext();
            _scoreService = scoreService;
            _pushService = pushService;
            _teamRepository = new TeamRepository();
        }

        private JObject GetBoxscore(string url)
        {
            HttpWebResponse webResponse = CommonFunctions.Instance.GetResponse(url);
            string apiResponse = CommonFunctions.Instance.ResponseToString(webResponse);
            JObject json = JObject.Parse(apiResponse);
            return json;
        }

        private bool IsFinished(JArray games)
        {
            if (games.Any(g => (int) g["statusNum"] != 3 || (bool) g["isGameActivated"]))
            {
                return false;
            }

            return true;
        }

        private void AddToDatabase(JToken player, Game game, DateTime date, int oppId, string score)
        {
            Stats statsObj = new Stats
            {
                Game = game,
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
                PTS = (int) player["points"]
            };

            var statsPlayer = _context.Players
                .FirstOrDefault(x => x.NbaID == (int) player["personId"]);

            if (statsPlayer == null)
                return;

            statsObj.Player = statsPlayer;
            int playerPrice = _scoreService.GetPrice(statsPlayer);

            var dbStats = _context.Stats.FirstOrDefault(stats =>
                stats.Date.Date.Equals(date.Date) && stats.PlayerID == statsPlayer.PlayerID);

            if (dbStats == null)
            {
                statsObj.GS = _scoreService.GetGameScore(statsObj.PTS, statsObj.FGM, statsObj.DREB, statsObj.OREB,
                    statsObj.STL, statsObj.AST, statsObj.BLK, statsObj.FGA, statsObj.FTA - statsObj.FTM,
                    statsObj.FLS, statsObj.TOV);

                statsObj.FP = _scoreService.GetFantasyPoints(statsObj.PTS, statsObj.DREB, statsObj.OREB,
                    statsObj.AST, statsObj.STL, statsObj.BLK, statsObj.TOV);

                statsObj.Price = playerPrice;

                _context.Stats.Add(statsObj);
            }
            else
            {
                dbStats.Game = game;
                dbStats.Score = score;
                dbStats.MIN = statsObj.MIN;
                dbStats.FGM = statsObj.FGM;
                dbStats.FGA = statsObj.FGA;
                dbStats.FGP = statsObj.FGP;
                dbStats.TPM = statsObj.TPM;
                dbStats.TPA = statsObj.TPA;
                dbStats.TPP = statsObj.TPP;
                dbStats.FTM = statsObj.FTM;
                dbStats.FTA = statsObj.FTA;
                dbStats.FTP = statsObj.FTP;
                dbStats.DREB = statsObj.DREB;
                dbStats.OREB = statsObj.OREB;
                dbStats.TREB = statsObj.TREB;
                dbStats.AST = statsObj.AST;
                dbStats.BLK = statsObj.BLK;
                dbStats.STL = statsObj.STL;
                dbStats.FLS = statsObj.FLS;
                dbStats.TOV = statsObj.TOV;
                dbStats.PTS = statsObj.PTS;
                dbStats.GS = _scoreService.GetGameScore(statsObj.PTS, statsObj.FGM, statsObj.DREB, statsObj.OREB,
                    statsObj.STL, statsObj.AST, statsObj.BLK, statsObj.FGA, statsObj.FTA - statsObj.FTM,
                    statsObj.FLS, statsObj.TOV);

                dbStats.FP = _scoreService.GetFantasyPoints(statsObj.PTS, statsObj.DREB, statsObj.OREB,
                    statsObj.AST, statsObj.STL, statsObj.BLK, statsObj.TOV);

                dbStats.Price = playerPrice;
            }
        }

        private Game GetGame(DateTime date, int homeTeamId, int awayTeamId, int homeScore, int awayScore)
        {
            Team homeTeam = _context.Teams.FirstOrDefault(team => team.NbaID == homeTeamId);
            if (homeTeam == null)
                homeTeam = _teamRepository.GetUnknownTeam();
            Team awayTeam = _context.Teams.FirstOrDefault(team => team.NbaID == awayTeamId);
            if (awayTeam == null)
                awayTeam = _teamRepository.GetUnknownTeam();

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

        public void Execute()
        {
            string gameDate = CommonFunctions.Instance.UTCToEastern(RuntimeUtils.PREVIOUS_GAME).ToString("yyyyMMdd");
            JArray games = CommonFunctions.Instance.GetGames(gameDate);
            int countOfActivatedGames = 0;
            bool isAnyGameStarted = false;
            foreach (var game in games)
            {
                if ((int) game["statusNum"] != 1)
                    isAnyGameStarted = true;

                if (!(bool) game["isGameActivated"] && (int) game["statusNum"] != 3)
                    continue;

                countOfActivatedGames++;
                string bsUrl = "http://data.nba.net/10s/prod/v1/" + gameDate + "/" + game["gameId"] + "_boxscore.json";
                JObject boxscore = GetBoxscore(bsUrl);
                if (boxscore["stats"] == null)
                    continue;
                int hTeam = (int) boxscore["basicGameData"]["hTeam"]["teamId"];
                int vTeam = (int) boxscore["basicGameData"]["vTeam"]["teamId"];
                DateTime date = DateTime.ParseExact((string) boxscore["basicGameData"]["startDateEastern"], "yyyyMMdd",
                    CultureInfo.InvariantCulture);
                var players = boxscore["stats"]["activePlayers"];
                int homeScore = (int) boxscore["basicGameData"]["hTeam"]["score"];
                int awayScore = (int) boxscore["basicGameData"]["vTeam"]["score"];
                Game gameObj = GetGame(date, hTeam, vTeam, homeScore, awayScore);

                foreach (var player in (JArray) players)
                {
                    int oppId;

                    if (!_context.Players.Any())
                        continue;
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

                    if ((string) player["min"] == null || ((string) player["min"]).Length == 0)
                        continue;
                    AddToDatabase(player, gameObj, date, oppId, score);
                }

                _context.SaveChanges();
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
                int minutesDelay = countOfActivatedGames == 0 ? 10 : 3;
                JobManager.AddJob(new StatsJob(_scoreService, _pushService),
                    s => s.WithName("statsSeed")
                        .ToRunOnceIn(minutesDelay).Minutes());
            }
            else
            {
                _context.Stats
                    .RemoveRange(_context.Stats.Where(stat => stat.Score.Contains("LIVE")));
                _context.SaveChanges();

                JobManager.AddJob(new UserScoreJob(_pushService),
                    s => s.WithName("userScore")
                        .ToRunNow());
                
                JobManager.AddJob(new StatsJob(_scoreService, _pushService),
                    s => s.WithName("statsSeed")
                        .ToRunOnceAt(RuntimeUtils.NEXT_GAME.AddMinutes(5)));
            }
        }
    }
}