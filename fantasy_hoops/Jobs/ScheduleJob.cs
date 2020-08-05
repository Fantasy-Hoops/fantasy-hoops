using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using fantasy_hoops.Database;
using fantasy_hoops.Helpers;
using fantasy_hoops.Models;
using fantasy_hoops.Models.Enums;
using FluentScheduler;
using Newtonsoft.Json.Linq;

namespace fantasy_hoops.Jobs
{
    public class ScheduleJob : IJob
    {
        private readonly GameContext _context;
        private string _seasonYear = CommonFunctions.Instance.GetSeasonYear();

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
            GameContext context = new GameContext();
            JArray scheduledGames = GetLeagueSchedule(_seasonYear)["league"]["standard"] as JArray;
            if (scheduledGames == null)
            {
                return;
            }

            Season season = UpdateSeason(scheduledGames);

            foreach (JToken game in scheduledGames)
            {
                string hTeamReference = game["hTeam"]["teamId"].ToString();
                string vTeamReference = game["vTeam"]["teamId"].ToString();
                int hTeamId = context.Teams
                    .Where(team => team.NbaID == int.Parse(hTeamReference))
                    .Select(team => team.TeamID)
                    .FirstOrDefault();
                int vTeamId = context.Teams
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
                    Date = GetDateECT(game),
                    SeasonStage = (SeasonStage)(int)game["seasonStageId"],
                    Status = (GameStatus)(int)game["statusNum"],
                    isTBD = (bool)game["isStartTimeTBD"],
                    HomeTeamID = hTeamId,
                    AwayTeamID = vTeamId,
                    HomeScore = homeScore,
                    AwayScore = awayScore,
                    SeasonId = season.Id
                };

                if (!context.Games.Any(dbGame => dbGame.Reference.Equals(reference)))
                {
                    context.Games.Add(newGame);
                }
            }
            context.SaveChanges();
        }
        
        private JObject GetLeagueSchedule(string year)
        {
            string url = $"http://data.nba.net/10s/prod/v1/{year}/schedule.json";
            HttpWebResponse webResponse = CommonFunctions.Instance.GetResponse(url);
            if (webResponse == null)
                return null;
            string apiResponse = CommonFunctions.Instance.ResponseToString(webResponse);
            JObject json = JObject.Parse(apiResponse);
            return json;
        }

        private Season UpdateSeason(JArray scheduledGames)
        {
            GameContext context = new GameContext();
            Season dbSeason = context.Seasons.Include(season => season.Games).FirstOrDefault(season => season.Year == int.Parse(_seasonYear));
            IEnumerable<DateTime> gamesDates = scheduledGames.AsEnumerable().Select(GetDateECT).ToList();
            DateTime startDate = gamesDates.Min();
            DateTime endDate = gamesDates.Max();
            IEnumerable<SeasonStage> seasonStages = scheduledGames
                .AsEnumerable()
                .Select(game => (SeasonStage) (int) game["seasonStageId"])
                .ToList();
            int preSeasonGames = seasonStages.Count(stage => stage == SeasonStage.PRE_SEASON);
            int regularSeasonGames = seasonStages.Count(stage => stage == SeasonStage.REGULAR_SEASON);
            int allStarBreakGames = seasonStages.Count(stage => stage == SeasonStage.ALL_STAR_BREAK);
            int playoffGames = seasonStages.Count(stage => stage == SeasonStage.PLAYOFFS);
            
            if (dbSeason == null)
            {
                dbSeason = new Season
                {
                    Id = Guid.NewGuid().ToString(),
                    Year = int.Parse(_seasonYear),
                    StartDate = startDate,
                    EndDate = endDate,
                    PreSeasonGames = preSeasonGames,
                    RegularSeasonGames = regularSeasonGames,
                    AllStarBreakGames = allStarBreakGames,
                    PlayoffGames = playoffGames,
                    Games = new List<Game>()
                };
                context.Seasons.Add(dbSeason);
            }
            else
            {
                dbSeason.EndDate = endDate;
                dbSeason.PreSeasonGames = preSeasonGames;
                dbSeason.RegularSeasonGames = regularSeasonGames;
                dbSeason.AllStarBreakGames = allStarBreakGames;
                dbSeason.PlayoffGames = playoffGames;
            }

            context.SaveChanges();
            return dbSeason;
        }

        private DateTime GetDateECT(JToken game)
        {
            DateTime dateUTC = DateTime.Parse(game["startTimeUTC"].ToString());
            return CommonFunctions.Instance.UTCToEastern(dateUTC);
        }
    }
}