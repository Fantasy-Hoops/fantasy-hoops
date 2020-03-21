using System;
using System.Globalization;
using System.Linq;
using System.Net;
using fantasy_hoops.Database;
using fantasy_hoops.Helpers;
using fantasy_hoops.Models;
using FluentScheduler;
using Newtonsoft.Json.Linq;

namespace fantasy_hoops.Jobs
{
    public class ScheduleJob : IJob
    {
        private readonly GameContext _context;
        private string _seasonYear = CommonFunctions.SEASON_YEAR;

        public ScheduleJob(string seasonYear = "")
        {
            _context = new GameContext();
            if (!string.IsNullOrEmpty(seasonYear))
            {
                try
                {
                    _seasonYear = DateTime.ParseExact(seasonYear, "yyyy", CultureInfo.InvariantCulture).Year.ToString();
                }
                catch
                {
                    // ignored
                }
            }
        }
        
        public void Execute()
        {
            JArray scheduledGames = GetLeagueSchedule(_seasonYear)["league"]["standard"] as JArray;
            if (scheduledGames == null)
            {
                return;
            }

            foreach (JToken game in scheduledGames)
            {
                DateTime dateUTC = DateTime.Parse(game["startTimeUTC"].ToString());
                DateTime dateECT = CommonFunctions.UTCToEastern(dateUTC);

                string hTeamReference = game["hTeam"]["teamId"].ToString();
                string vTeamReference = game["vTeam"]["teamId"].ToString();
                int hTeamId = _context.Teams
                    .Where(team => team.NbaID == int.Parse(hTeamReference))
                    .Select(team => team.TeamID)
                    .FirstOrDefault();
                int vTeamId = _context.Teams
                    .Where(team => team.NbaID == int.Parse(vTeamReference))
                    .Select(team => team.TeamID)
                    .FirstOrDefault();

                if (hTeamId == 0 || vTeamId == 0)
                {
                    continue;
                }

                int homeScore;
                int awayScore;
                int.TryParse(game["hTeam"]["score"].ToString(), out homeScore);
                int.TryParse(game["vTeam"]["score"].ToString(), out awayScore);

                string reference = game["gameId"].ToString();
                
                Game newGame = new Game
                {
                    Reference = reference,
                    Date = dateECT,
                    IsFinished = int.Parse(game["statusNum"].ToString()) == 3,
                    isTBD = (bool)game["isStartTimeTBD"],
                    HomeTeamID = hTeamId,
                    AwayTeamID = vTeamId,
                    HomeScore = homeScore,
                    AwayScore = awayScore
                };

                if (!_context.Games.Any(dbGame => dbGame.Reference.Equals(reference)))
                {
                    _context.Games.Add(newGame);
                }
            }
            _context.SaveChanges();
        }
        
        private JObject GetLeagueSchedule(string year)
        {
            string url = $"http://data.nba.net/10s/prod/v1/{year}/schedule.json";
            HttpWebResponse webResponse = CommonFunctions.GetResponse(url);
            if (webResponse == null)
                return null;
            string apiResponse = CommonFunctions.ResponseToString(webResponse);
            JObject json = JObject.Parse(apiResponse);
            return json;
        }
    }
}