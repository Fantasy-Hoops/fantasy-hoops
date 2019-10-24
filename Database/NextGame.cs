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
	public class NextGame : IJob
	{
		public static DateTime NEXT_GAME = DateTime.UtcNow;
		public static DateTime NEXT_GAME_CLIENT = DateTime.UtcNow;
		public static DateTime NEXT_LAST_GAME = DateTime.UtcNow;
		public static DateTime PREVIOUS_GAME = DateTime.UtcNow;
		public static DateTime PREVIOUS_LAST_GAME = DateTime.UtcNow;

		private const int GAME_OFFSET = 10;
		private static int offset = 0;

        private readonly GameContext _context;
        private readonly IScoreService _scoreService;
        private readonly IPushService _pushService;
        private readonly bool _updatePrice;

        public NextGame(IScoreService scoreService, IPushService pushService, bool updatePrice = true)
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
			return (string)json["links"]["currentDate"];
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
								.AddDays(1).ToString("yyyyMMdd");
				SetNextGame(gameDate);
			}
		}

		private void SetLastGame(string gameDate)
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

        public void Execute()
        {
            var sth = JobManager.RunningSchedules;
            string gameDate = GetDate();

            SetLastGame(gameDate);
            SetNextGame(gameDate);
            SetClientTime();

            if (offset < GAME_OFFSET)
            {
                JobManager.AddJob(new NextGame(_scoreService, _pushService),
                                s => s.WithName(NEXT_GAME.ToLongDateString())
                                .ToRunOnceAt(NEXT_GAME));

                // All actions run only in production
                if (bool.Parse(Environment.GetEnvironmentVariable("IS_PRODUCTION")))
                {
                    // Nudge Notifications don't run if game starts in <2 hours
                    if (NEXT_GAME.Subtract(DateTime.UtcNow).TotalMinutes > 115)
                        JobManager.AddJob(() => _pushService.SendNudgeNotifications().Wait(),
                                        s => s.WithName("nudgeNotifications")
                                        .ToRunOnceAt(NEXT_GAME.AddHours(-2)));

                    // Once per 2 days
                    if (CommonFunctions.UTCToEastern(NEXT_GAME).Day % 2 != 0)
                        JobManager.AddJob(new Seed(_pushService),
                                        s => s.WithName("seed")
                                        .ToRunOnceAt(NEXT_GAME.AddMinutes(-5)));

                    // 10 hours after previous last game if project ran before that time
                    // 10 hours after next last game if project ran after that time
                    DateTime previewsRuntime = PREVIOUS_LAST_GAME.AddHours(10);
                    if (DateTime.UtcNow > previewsRuntime)
                        previewsRuntime = NEXT_LAST_GAME.AddHours(10);
                    JobManager.AddJob(new NewsSeed(NewsSeed.NewsType.PREVIEWS),
                            s => s.WithName("previews")
                            .ToRunOnceAt(previewsRuntime));

                    // 5 hours after previous last game if project ran before that time
                    // 5 hours after next last game if project ran after that time
                    DateTime recapsRuntime = PREVIOUS_LAST_GAME.AddHours(5);
                    if (DateTime.UtcNow > recapsRuntime)
                        recapsRuntime = NEXT_LAST_GAME.AddHours(5);
                    JobManager.AddJob(new NewsSeed(NewsSeed.NewsType.RECAPS),
                            s => s.WithName("recaps")
                            .ToRunOnceAt(recapsRuntime));
                }

                JobManager.AddJob(new PlayerSeed(_scoreService, _updatePrice),
                    s => s.WithName("playerSeed")
                    .ToRunNow());
            }
            else
            {
                JobManager.AddJob(new NextGame(_scoreService, _pushService),
                                s => s.WithName("nextGame")
                                .ToRunOnceIn(1)
                                .Hours());
                offset = 0;
                SetPlayersNotPlaying().Wait();
            }

            JobManager.AddJob(new StatsSeed(_scoreService, _pushService),
                            s => s.WithName("statsSeed")
                            .ToRunNow());
        }
    }
}
