using System;
using System.Globalization;
using System.Net;
using System.Threading.Tasks;
using fantasy_hoops.Database;
using fantasy_hoops.Enums;
using fantasy_hoops.Helpers;
using fantasy_hoops.Services.Interfaces;
using FluentScheduler;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace fantasy_hoops.Jobs
{
    public class NextGameJob : IJob
    {
        public static DateTime NEXT_GAME = DateTime.UtcNow;
        public static DateTime NEXT_GAME_CLIENT = DateTime.UtcNow;
        public static DateTime NEXT_LAST_GAME = DateTime.UtcNow;
        public static DateTime PREVIOUS_GAME = DateTime.UtcNow;
        public static DateTime PREVIOUS_LAST_GAME = DateTime.UtcNow;

        private const int GAME_OFFSET = 10;
        private static int offset;

        private readonly GameContext _context;
        private readonly IScoreService _scoreService;
        private readonly IPushService _pushService;
        private readonly bool _updatePrice;

        public NextGameJob(IScoreService scoreService, IPushService pushService, bool updatePrice = true)
        {
            _context = new GameContext();
            _scoreService = scoreService;
            _pushService = pushService;
            _updatePrice = updatePrice;
        }

        private void SetClientTime()
        {
            NEXT_GAME_CLIENT = NEXT_GAME;
        }

        private async Task SetPlayersNotPlaying()
        {
            await _context.Players.ForEachAsync(p => p.IsPlaying = false);
            await _context.SaveChangesAsync();
        }

        private string GetDate()
        {
            string url = "http://data.nba.net/10s/prod/v1/today.json";
            HttpWebResponse webResponse = CommonFunctions.GetResponse(url);
            if (webResponse == null)
                return null;
            string apiResponse = CommonFunctions.ResponseToString(webResponse);
            JObject json = JObject.Parse(apiResponse);
            return (string) json["links"]["currentDate"];
        }

        private void SetNextGame(string gameDate)
        {
            if (offset >= GAME_OFFSET)
            {
                NEXT_GAME = new DateTime();
                NEXT_LAST_GAME = new DateTime();
                return;
            }

            JArray games = CommonFunctions.GetGames(gameDate);
            if (games.Count > 0)
            {
                DateTime timeUtc = DateTime.Parse((string) games[0]["startTimeUTC"]);
                if (timeUtc > DateTime.UtcNow)
                {
                    offset = 0;
                    NEXT_GAME = timeUtc;
                    NEXT_LAST_GAME = DateTime.Parse((string) games[games.Count - 1]["startTimeUTC"]);
                }
                else
                {
                    offset++;
                    gameDate = DateTime.ParseExact(gameDate, "yyyyMMdd", CultureInfo.InvariantCulture)
                        .AddDays(offset).ToString("yyyyMMdd");
                    SetNextGame(gameDate);
                }
            }
            else
            {
                offset++;
                gameDate = DateTime.ParseExact(gameDate, "yyyyMMdd", CultureInfo.InvariantCulture)
                    .AddDays(1).ToString("yyyyMMdd");
                SetNextGame(gameDate);
            }
        }

        private void SetLastGame(string gameDate)
        {
            JArray games = CommonFunctions.GetGames(gameDate);
            if (games.Count > 0)
            {
                DateTime timeUtc = DateTime.Parse((string) games[0]["startTimeUTC"]);
                if (timeUtc < DateTime.UtcNow)
                {
                    PREVIOUS_GAME = timeUtc;
                    PREVIOUS_LAST_GAME = DateTime.Parse((string) games[games.Count - 1]["startTimeUTC"]);
                }
                else
                {
                    gameDate = DateTime.ParseExact(gameDate, "yyyyMMdd", CultureInfo.InvariantCulture)
                        .AddDays(-1).ToString("yyyyMMdd");
                    SetLastGame(gameDate);
                }
            }
            else
            {
                gameDate = DateTime.ParseExact(gameDate, "yyyyMMdd", CultureInfo.InvariantCulture)
                    .AddDays(-1).ToString("yyyyMMdd");
                SetLastGame(gameDate);
            }
        }

        public void Execute()
        {
            string gameDate = GetDate();

            SetLastGame(gameDate);
            SetNextGame(gameDate);
            SetClientTime();

            if (offset < GAME_OFFSET)
            {
                JobManager.AddJob(new NextGameJob(_scoreService, _pushService),
                    s => s.WithName(NEXT_GAME.ToLongDateString())
                        .ToRunOnceAt(NEXT_GAME));
                
                JobManager.AddJob(new TournamentsJob(PREVIOUS_GAME),
                    s => s.WithName("TournamentsJob").ToRunNow());

                // All actions run only in production
                if (bool.Parse(Startup.Configuration["IsProduction"] ?? "false"))
                {
                    // Nudge Notifications don't run if game starts in <5 hours
                    if (NEXT_GAME.Subtract(DateTime.UtcNow).TotalMinutes > 295)
                        JobManager.AddJob(() => _pushService.SendNudgeNotifications(),
                            s => s.WithName("nudgeNotifications")
                                .ToRunOnceAt(NEXT_GAME.AddHours(-5)));

                    // Once per 2 days
                    if (CommonFunctions.UTCToEastern(NEXT_GAME).Day % 2 != 0)
                        JobManager.AddJob(new RostersJob(_pushService),
                            s => s.WithName("rostersJob")
                                .ToRunOnceAt(NEXT_GAME.AddMinutes(-5)));

                    // 10 hours after previous last game if project ran before that time
                    // 10 hours after next last game if project ran after that time
                    DateTime previewsRuntime = PREVIOUS_LAST_GAME.AddHours(10);
                    if (DateTime.UtcNow > previewsRuntime)
                        previewsRuntime = NEXT_LAST_GAME.AddHours(10);
                    JobManager.AddJob(new NewsJob(NewsType.PREVIEW),
                        s => s.WithName("previews")
                            .ToRunOnceAt(previewsRuntime));

                    // 5 hours after previous last game if project ran before that time
                    // 5 hours after next last game if project ran after that time
                    DateTime recapsRuntime = PREVIOUS_LAST_GAME.AddHours(5);
                    if (DateTime.UtcNow > recapsRuntime)
                        recapsRuntime = NEXT_LAST_GAME.AddHours(5);
                    JobManager.AddJob(new NewsJob(NewsType.RECAP),
                        s => s.WithName("recaps")
                            .ToRunOnceAt(recapsRuntime));

                    JobManager.AddJob(new PlayersJob(_scoreService, _updatePrice),
                        s => s.WithName("playersJob")
                            .ToRunNow());
                }
                else
                {
                    NEXT_GAME_CLIENT = NEXT_GAME;
                    PlayersJob.PLAYER_POOL_DATE = NEXT_GAME;
                }
            }
            else
            {
                JobManager.AddJob(new NextGameJob(_scoreService, _pushService),
                    s => s.WithName("nextGameJob")
                        .ToRunOnceIn(1)
                        .Hours());
                offset = 0;
                SetPlayersNotPlaying().Wait();
            }

            if (bool.Parse(Startup.Configuration["IsProduction"] ?? "false"))
            {
                Task.Run(() => new StatsJob(_scoreService, _pushService).Execute());
            }
        }
    }
}