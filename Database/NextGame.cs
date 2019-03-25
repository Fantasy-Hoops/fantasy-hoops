using FluentScheduler;
using Newtonsoft.Json.Linq;
using System;
using System.Globalization;
using System.Net;
using System.Threading.Tasks;
using fantasy_hoops.Helpers;
using System.Linq;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using fantasy_hoops.Services;

namespace fantasy_hoops.Database
{
    public class NextGame
    {
        public static DateTime NEXT_GAME = DateTime.UtcNow;
        public static DateTime NEXT_GAME_CLIENT = DateTime.UtcNow;
        public static DateTime NEXT_LAST_GAME = DateTime.UtcNow;
        public static DateTime PREVIOUS_GAME = DateTime.UtcNow;
        public static DateTime PREVIOUS_LAST_GAME = DateTime.UtcNow;

        private const int GAME_OFFSET = 2;
        private static int offset = 0;

        public static void SetClientTime()
        {
            NEXT_GAME_CLIENT = NEXT_GAME;
        }

        public static void Initialize(GameContext context, bool updatePrices = true)
        {
            string gameDate = GetDate();

            SetLastGame(gameDate);
            SetNextGame(gameDate);
            SetClientTime();

            if (offset < GAME_OFFSET)
            {
                JobManager.AddJob(() => Initialize(context),
                    s => s.WithName(NEXT_GAME.ToLongDateString())
                    .ToRunOnceAt(NEXT_GAME));

                if (NEXT_GAME.Subtract(DateTime.UtcNow).TotalMinutes > 115)
                    JobManager.AddJob(() => PushService.Instance.Value.SendNudgeNotifications().Wait(),
                        s => s.WithName("nudgeNotifications")
                        .ToRunOnceAt(NEXT_GAME.AddHours(-2)));

                DateTime nextRun = NEXT_LAST_GAME;
                if (DateTime.UtcNow < PREVIOUS_LAST_GAME.AddHours(2).AddMinutes(30))
                    nextRun = PREVIOUS_LAST_GAME;

                if (CommonFunctions.UTCToEastern(nextRun).Day % 2 != 0)
                    JobManager.AddJob(() => Seed.Initialize(context),
                        s => s.WithName("seed")
                        .ToRunOnceAt(NEXT_GAME.AddMinutes(-5)));

                if (DateTime.UtcNow < PREVIOUS_LAST_GAME.AddHours(2).AddMinutes(30)
                    && bool.Parse(Environment.GetEnvironmentVariable("UPDATE_PLAYER_SEED")))
                    JobManager.AddJob(() => StatsSeed.Initialize(context),
                        s => s.WithName("statsSeed")
                        .ToRunNow());
                else
                    JobManager.AddJob(() => StatsSeed.Initialize(context),
                        s => s.WithName("statsSeed")
                        .ToRunOnceAt(nextRun.AddHours(2).AddMinutes(30)));

                DateTime previewsRuntime = PREVIOUS_LAST_GAME.AddHours(10);
                if (DateTime.UtcNow > previewsRuntime)
                    previewsRuntime = NEXT_LAST_GAME.AddHours(10);
                JobManager.AddJob(() => NewsSeed.ExtractPreviews(context),
                    s => s.WithName("previews")
                    .ToRunOnceAt(previewsRuntime));

                JobManager.AddJob(() => NewsSeed.ExtractRecaps(context),
                    s => s.WithName("recaps")
                .ToRunOnceAt(nextRun.AddHours(5)));

                JobManager.AddJob(() => PlayerSeed.Initialize(context, updatePrices),
                     s => s.WithName("playerSeed")
                     .ToRunNow());
            }
            else
            {
                JobManager.AddJob(() => Initialize(context),
                    s => s.WithName("nextGame")
                    .ToRunOnceIn(1)
                    .Hours());
                offset = 0;
                Task.Run(() => SetPlayersNotPlaying(context)).Wait();
            }
        }

        private static async Task SetPlayersNotPlaying(GameContext context)
        {
            await context.Players.ForEachAsync(p => p.IsPlaying = false);
            await context.SaveChangesAsync();
        }

        private static string GetDate()
        {
            string url = "http://data.nba.net/10s/prod/v1/today.json";
            HttpWebResponse webResponse = CommonFunctions.GetResponse(url);
            if (webResponse == null)
                return null;
            string apiResponse = CommonFunctions.ResponseToString(webResponse);
            JObject json = JObject.Parse(apiResponse);
            return (string)json["links"]["currentDate"];
        }

        private static void SetNextGame(string gameDate)
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
                DateTime timeUTC = DateTime.Parse((string)games[0]["startTimeUTC"]);
                if (timeUTC > DateTime.UtcNow)
                {
                    offset = 0;
                    NEXT_GAME = timeUTC;
                    NEXT_LAST_GAME = DateTime.Parse((string)games[games.Count - 1]["startTimeUTC"]);
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
                    .AddDays(offset).ToString("yyyyMMdd");
                SetNextGame(gameDate);
            }
        }

        private static void SetLastGame(string gameDate)
        {
            JArray games = CommonFunctions.GetGames(gameDate);
            if (games.Count > 0)
            {
                DateTime timeUTC = DateTime.Parse((string)games[0]["startTimeUTC"]);
                if (timeUTC < DateTime.UtcNow)
                {
                    PREVIOUS_GAME = timeUTC;
                    PREVIOUS_LAST_GAME = DateTime.Parse((string)games[games.Count - 1]["startTimeUTC"]);
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
    }
}
