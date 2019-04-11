﻿using FluentScheduler;
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

        private const int GAME_OFFSET = 10;
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

                // All actions run only in production
                if (bool.Parse(Environment.GetEnvironmentVariable("IS_PRODUCTION")))
                {
                    // Nudge Notifications don't run if game starts in <2 hours
                    if (NEXT_GAME.Subtract(DateTime.UtcNow).TotalMinutes > 115)
                        JobManager.AddJob(() => PushService.Instance.Value.SendNudgeNotifications().Wait(),
                                s => s.WithName("nudgeNotifications")
                                .ToRunOnceAt(NEXT_GAME.AddHours(-2)));

                    // Once per 2 days
                    if (CommonFunctions.UTCToEastern(NEXT_GAME).Day % 2 != 0)
                        JobManager.AddJob(() => Seed.Initialize(context),
                                s => s.WithName("seed")
                                .ToRunOnceAt(NEXT_GAME.AddMinutes(-5)));

                    // If previous game isn't finished run StatsSeed now
                    if (DateTime.UtcNow < PREVIOUS_LAST_GAME.AddHours(2).AddMinutes(30))
                        JobManager.AddJob(() => StatsSeed.Initialize(context),
                                s => s.WithName("statsSeed")
                                .ToRunNow());
                    else // If previous game is finished, run 15min after next game starts
                        JobManager.AddJob(() => StatsSeed.Initialize(context),
                                s => s.WithName("statsSeed")
                                .ToRunOnceAt(NEXT_GAME.AddMinutes(15)));

                    // 10 hours after previous last game if project ran before that time
                    // 10 hours after next last game if project ran after that time
                    DateTime previewsRuntime = PREVIOUS_LAST_GAME.AddHours(10);
                    if (DateTime.UtcNow > previewsRuntime)
                        previewsRuntime = NEXT_LAST_GAME.AddHours(10);
                    JobManager.AddJob(() => NewsSeed.ExtractPreviews(context),
                        s => s.WithName("previews")
                        .ToRunOnceAt(previewsRuntime));

                    // 5 hours after previous last game if project ran before that time
                    // 5 hours after next last game if project ran after that time
                    DateTime recapsRuntime = PREVIOUS_LAST_GAME.AddHours(5);
                    if (DateTime.UtcNow > recapsRuntime)
                        recapsRuntime = NEXT_LAST_GAME.AddHours(5);
                    JobManager.AddJob(() => NewsSeed.ExtractRecaps(context),
                        s => s.WithName("recaps")
                        .ToRunOnceAt(recapsRuntime));
                }


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
