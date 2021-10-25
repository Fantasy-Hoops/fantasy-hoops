using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using fantasy_hoops.Database;
using fantasy_hoops.Helpers;
using fantasy_hoops.Models;
using fantasy_hoops.Models.ViewModels;
using fantasy_hoops.Repositories;
using fantasy_hoops.Repositories.Interfaces;
using fantasy_hoops.Services.Interfaces;
using Newtonsoft.Json.Linq;

namespace fantasy_hoops.Jobs
{
    public class RostersJob : ICronJob
    {
        private const int ApiPlanQuota = 1000;
        private const int NbaTeamsCount = 30;

        private readonly GameContext _context;
        private readonly IPushService _pushService;
        private readonly ITeamRepository _teamRepository;

        public RostersJob(GameContext context, IPushService pushService)
        {
            _context = context;
            _pushService = pushService;
            _teamRepository = new TeamRepository();
        }


        public void Execute()
        {
            var teams = GetTeams();
            if (teams.Count == 0) return;

            Thread.Sleep(1000);
            var dbTeams = _context.Teams.ToList();

            foreach (var team in teams)
            {
                var teamNbaId = (int) team["reference"];
                var dbTeam = dbTeams.Find(t => t.NbaID == teamNbaId);
                if (dbTeam == null)
                {
                    dbTeam = createDbTeam(team, teamNbaId);
                    _context.Teams.Add(dbTeam);
                    _context.SaveChanges();
                }

                var unknownTeam = _teamRepository.GetUnknownTeam();
                var teamPlayers = GetTeamPlayers((string) team["id"]);
                Thread.Sleep(1000);

                // Update inactive players with no team
                var teamPlayersIds = teamPlayers.Select(r => (int) r["reference"]).ToList();
                foreach (var player in _context.Players.Where(player =>
                    player.TeamID == dbTeam.TeamID && !teamPlayersIds.Contains(player.NbaID)))
                    player.TeamID = unknownTeam.TeamID;

                foreach (var player in teamPlayers)
                {
                    var playerNbaId = (int) player["reference"];
                    var isInGLeague = player["status"] != null && player["status"].ToString().Equals("D-LEAGUE");
                    var dbPlayer = _context.Players.FirstOrDefault(p => p.NbaID == playerNbaId);
                    if (dbPlayer == null)
                    {
                        dbPlayer = CreateDbPlayer(player, dbTeam.TeamID, isInGLeague);
                        if (dbPlayer == null) continue;

                        _context.Players.Add(dbPlayer);
                        if (player["primary_position"] != null &&
                            "na".Equals(player["primary_position"].ToString().ToLower()))
                            _pushService.SendAdminNotification(
                                new PushNotificationViewModel(
                                    "Fantasy Hoops Notification",
                                    $"Player '{dbPlayer.FullName}' with position {dbPlayer.Position} was added to database.")
                            );

                        continue;
                    }

                    if (player["jersey_number"] != null) dbPlayer.Number = player["jersey_number"].ToString();

                    dbPlayer.TeamID = dbTeam.TeamID;
                    dbPlayer.IsInGLeague = isInGLeague;
                }

                _context.SaveChanges();
            }
        }

        private Player CreateDbPlayer(JToken player, int teamId, bool isInGLeague)
        {
            if (player["reference"] == null) return null;

            var abbrName = player["first_name"] != null && player["first_name"].ToString().Length > 1
                ? $"{player["first_name"].ToString()[0]}. {player["last_name"]}"
                : player["last_name"].ToString();

            return new Player
            {
                FullName = (string) player["full_name"],
                FirstName = (string) player["first_name"],
                LastName = (string) player["last_name"],
                AbbrName = abbrName,
                Position = (string) player["primary_position"],
                NbaID = (int) player["reference"],
                Number = player["jersey_number"] != null ? player["jersey_number"].ToString() : "0",
                Price = CommonFunctions.Instance.PRICE_FLOOR,
                PreviousPrice = CommonFunctions.Instance.PRICE_FLOOR,
                FPPG = 0.0,
                PTS = 0.0,
                REB = 0.0,
                AST = 0.0,
                STL = 0.0,
                BLK = 0.0,
                TeamID = teamId,
                IsPlaying = false,
                IsInGLeague = isInGLeague,
                Injury = new Injury {Status = "Active"}
            };
        }

        private Team createDbTeam(JToken team, int teamNbaId)
        {
            return new Team
            {
                Name = (string) team["name"],
                City = (string) team["market"],
                NbaID = teamNbaId,
                Color = TeamUtils.GetTeamColor(teamNbaId)
            };
        }

        private List<JToken> GetTeams()
        {
            HttpWebResponse webResponse;
            var teamsUrl = "http://api.sportradar.us/nba/trial/v7/en/seasons/" +
                           CommonFunctions.Instance.GetSeasonYear()
                           + "/REG/rankings.json?api_key=" + Startup.Configuration["Sportradar:ApiKey"];
            try
            {
                var request = (HttpWebRequest) WebRequest.Create(teamsUrl);
                request.Method = "GET";
                request.KeepAlive = true;
                request.ContentType = "application/json";
                webResponse = (HttpWebResponse) request.GetResponse();
            }
            catch (WebException e)
            {
                var expiredNotification =
                    new PushNotificationViewModel("Fantasy Hoops Admin Notification",
                        "Sportradar API Key has expired! Please change the API Key to new one. " +
                        "Error message: " + e.Message);
                _pushService.SendAdminNotification(expiredNotification);
                return new List<JToken>();
            }

            var expiration = webResponse.GetResponseHeader("Expires");
            var callsLeft = ApiPlanQuota -
                            (int.Parse(webResponse.GetResponseHeader("X-Plan-Quota-Current")) + NbaTeamsCount);
            var notification =
                new PushNotificationViewModel("Fantasy Hoops Admin Notification",
                    $"Sportradar API calls left: {callsLeft}. Expires: {expiration}");
            _pushService.SendAdminNotification(notification);

            var responseString = CommonFunctions.Instance.ResponseToString(webResponse);
            var teams = new List<JToken>();
            var json = JObject.Parse(responseString);
            if (json["conferences"] == null || json["conferences"] == null || json["conferences"] == null)
                return new List<JToken>();

            json["conferences"].ToList().ForEach(conf =>
            {
                conf["divisions"].ToList().ForEach(div =>
                {
                    div["teams"].ToList().ForEach(team => teams.Add(team));
                });
            });
            return teams;
        }

        private List<JToken> GetTeamPlayers(string teamId)
        {
            var rosterUrl = "http://api.sportradar.us/nba/trial/v7/en/teams/" + teamId + "/profile.json?api_key=" +
                            Startup.Configuration["Sportradar:ApiKey"];
            var webResponse = CommonFunctions.Instance.GetResponse(rosterUrl);
            if (webResponse == null)
                return new List<JToken>();
            var responseString = CommonFunctions.Instance.ResponseToString(webResponse);

            var json = JObject.Parse(responseString);
            if (json["players"] == null) return new List<JToken>();

            return json["players"].Where(player => player["reference"] != null).ToList();
        }
    }
}