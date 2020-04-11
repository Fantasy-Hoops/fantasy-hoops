using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using fantasy_hoops.Database;
using fantasy_hoops.Helpers;
using fantasy_hoops.Models;
using fantasy_hoops.Models.Notifications;
using fantasy_hoops.Models.ViewModels;
using fantasy_hoops.Services.Interfaces;
using FluentScheduler;
using Newtonsoft.Json.Linq;

namespace fantasy_hoops.Jobs
{
    public class InjuriesJob : IJob
    {
        private readonly GameContext _context;
        private readonly IPushService _pushService;

        private static readonly Stack<InjuryPushNotificationViewModel> LineupsAffected =
            new Stack<InjuryPushNotificationViewModel>();

        public InjuriesJob(IPushService pushService)
        {
            _context = new GameContext();
            _pushService = pushService;
        }

        private static JArray GetInjuries()
        {
            HttpWebResponse webResponse =
                CommonFunctions.GetResponse("https://www.fantasylabs.com/api/players/news/2/");
            string myResponse = CommonFunctions.ResponseToString(webResponse);
            JArray injuries = JArray.Parse(myResponse);
            return injuries;
        }

        private void AddToDatabase(JToken injury, DateTime? dateModified)
        {
            Player injuryPlayer = _context.Players.FirstOrDefault(x => x.NbaID == (int) injury["PrimarySourceKey"]);

            if (injuryPlayer == null)
                return;

            var injuryObj = new Injury
            {
                Title = injury.Value<string>("Title") != null ? (string) injury["Title"] : null,
                Status = injury.Value<string>("PlayerStatus") != null ? (string) injury["PlayerStatus"] : null,
                InjuryTitle = injury.Value<string>("Injury") != null ? (string) injury["Injury"] : null,
                Description = injury.Value<string>("News") != null ? (string) injury["News"] : null,
                Date = dateModified,
                Link = injury.Value<string>("Link") != null ? (string) injury["Link"] : null,
                Player = injuryPlayer,
                PlayerID = injuryPlayer.PlayerID
            };

            var dbInjury = _context.Injuries
                .FirstOrDefault(inj => inj.Player.NbaID == (int) injury["PrimarySourceKey"]);

            string statusBefore = dbInjury?.Status;
            string statusAfter = injuryObj.Status;


            if (dbInjury == null)
            {
                _context.Injuries.Add(injuryObj);
            }
            else
            {
                dbInjury.Title = injuryObj.Title;
                dbInjury.Status = injuryObj.Status;
                dbInjury.InjuryTitle = injuryObj.InjuryTitle;
                dbInjury.Description = injuryObj.Description;
                dbInjury.Date = injuryObj.Date;
                dbInjury.Link = injuryObj.Link;
                dbInjury.Player = injuryPlayer;
                dbInjury.PlayerID = injuryPlayer.PlayerID;
            }

            _context.Injuries.Update(dbInjury);
            _context.SaveChanges();

            if (statusAfter != null && (statusAfter.Equals("Active") && !injuryPlayer.IsPlaying))
            {
                if (injuryPlayer.Team.Players.Any(p => p.IsPlaying))
                {
                    injuryPlayer.IsPlaying = true;
                }
            }

            if (statusBefore != null && !statusBefore.Equals(statusAfter))
                UpdateNotifications(dbInjury, statusBefore, statusAfter);
        }

        private void UpdateNotifications(Injury injury, string statusBefore, string statusAfter)
        {
            foreach (var lineup in _context.UserLineups
                .Where(x => x.Date.Equals(CommonFunctions.UTCToEastern(NextGameJob.NEXT_GAME).Date)
                            && (x.PgID == injury.PlayerID
                                || x.SgID == injury.PlayerID
                                || x.SfID == injury.PlayerID
                                || x.PfID == injury.PlayerID
                                || x.CID == injury.PlayerID)))
            {
                LineupsAffected.Push(new InjuryPushNotificationViewModel
                {
                    UserID = lineup.UserID,
                    StatusBefore = statusBefore,
                    StatusAfter = statusAfter,
                    FullName = injury.Player.FullName,
                    PlayerNbaID = injury.Player.NbaID
                });
                var inj = new InjuryNotification
                {
                    ReceiverID = lineup.UserID,
                    ReadStatus = false,
                    DateCreated = DateTime.UtcNow,
                    PlayerID = injury.PlayerID,
                    InjuryStatus = injury.Status,
                    InjuryDescription = injury.InjuryTitle
                };

                if (!_context.InjuryNotifications
                    .Any(x => x.InjuryStatus.Equals(inj.InjuryStatus) && x.PlayerID == inj.PlayerID))
                    _context.InjuryNotifications.Add(inj);
                _context.SaveChanges();
            }
        }

        private async Task SendPushNotifications()
        {
            while (LineupsAffected.Count > 0)
            {
                var lineup = LineupsAffected.Pop();
                PushNotificationViewModel notification =
                    new PushNotificationViewModel(lineup.FullName,
                        $"Status changed from {lineup.StatusBefore} to {lineup.StatusAfter}!")
                    {
                        Image = Environment.GetEnvironmentVariable("IMAGES_SERVER_NAME") + "/content/images/players/" +
                                lineup.PlayerNbaID + ".png",
                        Actions = new List<NotificationAction> {new NotificationAction("lineup", "🤾🏾‍♂️ Lineup")}
                    };
                await _pushService.Send(lineup.UserID, notification);
            }
        }

        public void Execute()
        {
            int seasonYear = int.Parse(CommonFunctions.SEASON_YEAR);
            IEnumerable<JToken> injuries = GetInjuries()
                .Where(inj => inj.Value<string>("ModifiedDate") == null
                              || DateTime.Parse(inj.Value<string>("ModifiedDate")) >=
                              new DateTime(seasonYear - 1, 8, 1)).AsEnumerable();
            foreach (JToken injury in injuries)
            {
                if (injury.Value<int?>("PrimarySourceKey") == null)
                    continue;
                var nbaId = (int) injury["PrimarySourceKey"];

                DateTime? dateModified = new DateTime?();
                if (injury.Value<string>("ModifiedDate") != null)
                {
                    dateModified = DateTime.Parse(injury["ModifiedDate"].ToString()).AddHours(4);
                    dateModified = dateModified.Value.IsDaylightSavingTime()
                        ? dateModified.Value.AddHours(-1)
                        : dateModified;
                }

                if (_context.Injuries
                    .Where(inj => inj.Player.NbaID == nbaId)
                    .Any(inj => inj.Status.Equals((string) injury["PlayerStatus"])
                                && dateModified.Equals(inj.Date)))
                    continue;

                try
                {
                    AddToDatabase(injury, dateModified);
                }
                catch (Exception)
                {
                    continue;
                }
            }

            _context.SaveChanges();
            Task.Run(() => SendPushNotifications()).Wait();
        }
    }
}