using fantasy_hoops.Models;
using System;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Threading.Tasks;
using fantasy_hoops.Helpers;
using System.Collections;
using System.Collections.Generic;
using FluentScheduler;
using Microsoft.EntityFrameworkCore;
using fantasy_hoops.Models.ViewModels;
using fantasy_hoops.Services;

namespace fantasy_hoops.Database
{
    public class Seed
    {
        public static void Initialize(GameContext context)
        {
            if (JobManager.RunningSchedules.Any(s => !s.Name.Equals("seed")))
            {
                JobManager.AddJob(() => Initialize(context),
                s => s.WithName("seed")
                .ToRunOnceIn(30)
                .Seconds());
                return;
            }

            Task.Run(() => Calculate(context)).Wait();
        }

        private static async Task Calculate(GameContext context)
        {
            List<JToken> teams = await GetTeams();
            System.Threading.Thread.Sleep(1000);
            var dbPlayers = context.Players;
            var dbTeams = context.Teams;
            foreach (var team in teams)
            {
                int teamNbaId = (int)team["reference"];

                if (await dbTeams.AnyAsync(t => t.NbaID == teamNbaId))
                    goto Roster;

                var teamObj = new Team
                {
                    Name = (string)team["name"],
                    City = (string)team["market"],
                    NbaID = teamNbaId,
                    Color = GetTeamColor(teamNbaId)
                };
                await dbTeams.AddAsync(teamObj);

            Roster:
                Team dbTeam = dbTeams.Where(t => t.NbaID == teamNbaId).FirstOrDefault();
                List<JToken> roster = GetRoster((string)team["id"]);
                System.Threading.Thread.Sleep(1000);
                foreach (var player in roster)
                {
                    if (player["reference"] == null)
                        continue;
                    int playerNbaId = (int)player["reference"];
                    bool gLeagueStatus = player["status"].ToString().Equals("D-LEAGUE") ? true : false;
                    if (await dbPlayers.AnyAsync(p => p.NbaID == playerNbaId))
                    {
                        Player dbPlayer = dbPlayers.Where(p => p.NbaID == playerNbaId).FirstOrDefault();
                        if (dbPlayer.Team == null)
                            dbPlayer.Team = dbTeam;

                        // UPDATE EXISTING
                        if (dbPlayer.Team.NbaID == teamNbaId)
                        {
                            dbPlayer.IsInGLeague = gLeagueStatus;
                            continue;
                        }
                        else
                        {
                            var newTeam = dbTeams.Where(t => t.NbaID == teamNbaId).FirstOrDefault();
                            dbPlayer.Team = newTeam;
                            dbPlayer.Number = (int)player["jersey_number"];
                            dbPlayer.IsInGLeague = gLeagueStatus;
                        }
                    }
                    else
                    {
                        // ADD NEW
                        try
                        {
                            string abbrName = "";
                            if (player["first_name"] != null
                                && player["first_name"].ToString().Length > 1)
                                abbrName = string.Format("{0}. {1}", player["first_name"].ToString()[0], player["last_name"].ToString());
                            else
                                abbrName = player["last_name"].ToString();
                            var playerObj = new Player
                            {
                                FullName = (string)player["full_name"],
                                FirstName = (string)player["first_name"],
                                LastName = (string)player["last_name"],
                                AbbrName = abbrName,
                                Position = (string)player["primary_position"],
                                NbaID = (int)player["reference"],
                                Number = (int)player["jersey_number"],
                                Price = PlayerSeed.PRICE_FLOOR,
                                FPPG = 0.0,
                                PTS = 0.0,
                                REB = 0.0,
                                AST = 0.0,
                                STL = 0.0,
                                BLK = 0.0,
                                TeamID = dbTeam.TeamID,
                                Team = dbTeam,
                                IsPlaying = false,
                                Status = "Active",
                                IsInGLeague = gLeagueStatus
                            };
                            await dbPlayers.AddAsync(playerObj);
                            if (playerObj.Position.Equals("NA"))
                                await PushService.Instance.Value.SendAdminNotification(new PushNotificationViewModel("Fantasy Hoops Notification", string.Format("Player '{0}' with position {1} was added to database.", playerObj.FullName, playerObj.Position)));
                        }
                        catch (ArgumentNullException)
                        {
                            continue;
                        }
                    }
                }
            }
            await context.SaveChangesAsync();
        }

        private static async Task<List<JToken>> GetTeams()
        {
            HttpWebResponse webResponse = null;
            string teamsURL = "http://api.sportradar.us/nba/trial/v4/en/seasons/" + CommonFunctions.GetSeasonYear()
                + "/REG/rankings.json?api_key=" + Environment.GetEnvironmentVariable("API_KEY");
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(teamsURL);
                request.Method = "GET";
                request.KeepAlive = true;
                request.ContentType = "application/json";
                webResponse = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException e)
            {
                PushNotificationViewModel notification =
                    new PushNotificationViewModel("Fantasy Hoops Admin Notification", "Sportradar API Key has expired! Please change the API Key to new one. " +
                    "Error message: " + e.Message);
                await PushService.Instance.Value.SendAdminNotification(notification);
                return new List<JToken>();
            }
            if (webResponse != null)
            {
                string expiration = webResponse.Headers.Get(3);
                int callsLeft = 1000 - (int.Parse(webResponse.Headers.Get(12)) + 30);
                PushNotificationViewModel notification =
                    new PushNotificationViewModel("Fantasy Hoops Admin Notification", "Sportradar API calls left: " + callsLeft);
                await PushService.Instance.Value.SendAdminNotification(notification);
            }
            string responseString = CommonFunctions.ResponseToString(webResponse);
            List<JToken> teams = new List<JToken>();
            JObject json = JObject.Parse(responseString);
            json["conferences"].ToList().ForEach(conf =>
            {
                conf["divisions"].ToList().ForEach(div =>
                {
                    div["teams"].ToList().ForEach(team => teams.Add(team));
                });
            });
            return teams;
        }

        private static List<JToken> GetRoster(string teamId)
        {
            string rosterURL = "http://api.sportradar.us/nba/trial/v4/en/teams/" + teamId + "/profile.json?api_key=" + Environment.GetEnvironmentVariable("API_KEY");
            HttpWebResponse webResponse = CommonFunctions.GetResponse(rosterURL);
            if (webResponse == null)
                return new List<JToken>();
            string responseString = CommonFunctions.ResponseToString(webResponse);

            JObject json = JObject.Parse(responseString);
            return json["players"].ToList();
        }

        public static async Task UpdateTeamColors(GameContext context)
        {
            foreach (var team in context.Teams)
            {
                team.Color = GetTeamColor(team.NbaID);
            }
            await context.SaveChangesAsync();
        }

        private static string GetTeamColor(int id)
        {
            switch (id)
            {
                // RAPTORS
                case 1610612761:
                    return "#CD1141";
                // CELTICS
                case 1610612738:
                    return "#008248";
                // 76ERS
                case 1610612755:
                    return "#002B5C";
                // KNICKS
                case 1610612752:
                    return "#F58426";
                // NETS
                case 1610612751:
                    return "#000000";
                // CAVALIERS
                case 1610612739:
                    return "#690031";
                // PACERS
                case 1610612754:
                    return "#002D62";
                // BUCKS
                case 1610612749:
                    return "#00471B";
                // PISTONS
                case 1610612765:
                    return "#E01E38";
                // BULLS
                case 1610612741:
                    return "#CE1141";
                // WIZARDS
                case 1610612764:
                    return "#E31837";
                // HEAT
                case 1610612748:
                    return "#98002E";
                // HORNETS
                case 1610612766:
                    return "#1D1160";
                // MAGIC
                case 1610612753:
                    return "#0B77BD";
                // HAWKS
                case 1610612737:
                    return "#E03A3E";
                // TRAIL BLAZERS
                case 1610612757:
                    return "#E13A3E";
                // TIMBERWOLVES
                case 1610612750:
                    return "#002B5C";
                // THUNDER
                case 1610612760:
                    return "#EF3B24";
                // NUGGETS
                case 1610612743:
                    return "#00285E";
                // JAZZ
                case 1610612762:
                    return "#0C2340";
                // WARRIORS
                case 1610612744:
                    return "#243E90";
                // CLIPPERS
                case 1610612746:
                    return "#ED174C";
                // LAKERS
                case 1610612747:
                    return "#552583";
                // KINGS
                case 1610612758:
                    return "#5A2D81";
                // SUNS
                case 1610612756:
                    return "#E56020";
                // ROCKETS
                case 1610612745:
                    return "#C21F32";
                // PELICANS
                case 1610612740:
                    return "#85714D";
                // SPURS
                case 1610612759:
                    return "#8A8D8F";
                // MAVERICKS
                case 1610612742:
                    return "#007DC5";
                // GRIZZLIES
                case 1610612763:
                    return "#12173F";
                default:
                    return "#C4CED4";
            }
        }
    }
}
