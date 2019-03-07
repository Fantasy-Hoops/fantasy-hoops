using System.Net;
using System.Linq;
using System.Threading.Tasks;
using System;
using Newtonsoft.Json.Linq;
using fantasy_hoops.Models;
using System.Globalization;
using fantasy_hoops.Helpers;
using FluentScheduler;
using System.Threading;
using Microsoft.EntityFrameworkCore;

namespace fantasy_hoops.Database
{
    public class NewsSeed
    {
        public static void ExtractPreviews(GameContext context)
        {
            if (JobManager.RunningSchedules.Any(s => !s.Name.Equals("previews")))
            {
                JobManager.AddJob(() => ExtractPreviews(context),
                s => s.WithName("previews")
                .ToRunOnceIn(30)
                .Seconds());
                return;
            }

            string today = Today();
            JArray tGames = CommonFunctions.GetGames(today);
            Task.Run(() => GetPreviews(context, today, tGames)).Wait();
        }

        public static void ExtractRecaps(GameContext context)
        {
            if (JobManager.RunningSchedules.Any(s => !s.Name.Equals("recaps")))
            {
                JobManager.AddJob(() => ExtractRecaps(context),
                s => s.WithName("recaps")
                .ToRunOnceIn(30)
                .Seconds());
                return;
            }

            string yesterday = Yesterday();
            JArray yGames = CommonFunctions.GetGames(yesterday);
            Task.Run(() => GetRecaps(context, yesterday, yGames)).Wait();
        }

        private static async Task GetPreviews(GameContext context, string date, JArray games)
        {
            JArray previews = new JArray();
            foreach (JObject game in games)
            {
                string preview = "http://data.nba.net/10s/prod/v1/" + date + "/" + game["gameId"] + "_preview_article.json";
                JObject previewJson = null;
                try
                {
                    HttpWebResponse previewResponse = CommonFunctions.GetResponse(preview);
                    string apiPreviewResponse = CommonFunctions.ResponseToString(previewResponse);
                    previewJson = JObject.Parse(apiPreviewResponse);
                }

                catch
                {
                    continue;
                }

                if (previewJson != null)
                {
                    previewJson.Add("hTeamID", game["hTeam"]["teamId"]);
                    previewJson.Add("vTeamID", game["vTeam"]["teamId"]);
                    previews.Add(previewJson);
                }
            }

            foreach (JObject previewObj in previews)
                await AddToDatabaseAsync(context, previewObj);

            await context.SaveChangesAsync();
        }

        public static async Task GetRecaps(GameContext context, string date, JArray games)
        {
            JArray news = new JArray();
            foreach (JObject game in games)
            {
                string recap = "http://data.nba.net/10s/prod/v1/" + date + "/" + game["gameId"] + "_recap_article.json";
                JObject recapJson = null;
                try
                {
                    HttpWebResponse recapResponse = CommonFunctions.GetResponse(recap);
                    string apiRecapResponse = CommonFunctions.ResponseToString(recapResponse);
                    recapJson = JObject.Parse(apiRecapResponse);
                }
                catch
                {
                    continue;
                }

                if (recapJson != null)
                {
                    recapJson.Add("hTeamID", game["hTeam"]["teamId"]);
                    recapJson.Add("vTeamID", game["vTeam"]["teamId"]);
                    news.Add(recapJson);
                }
            }

            foreach (JObject newsObj in news)
                await AddToDatabaseAsync(context, newsObj);

            await context.SaveChangesAsync();
        }

        private static async Task AddToDatabaseAsync(GameContext context, JToken newsObj)
        {
            var nObj = new News
            {
                Date = DateTime.Parse(newsObj["pubDateUTC"].ToString()),
                Title = (string)newsObj["title"],
                hTeamID = (int)newsObj["hTeamID"],
                vTeamID = (int)newsObj["vTeamID"]
            };

            bool shouldAdd = !await context.News.AnyAsync(x => x.Title.Equals((string)newsObj["title"])
            && x.Date.Equals(DateTime.Parse(newsObj["pubDateUTC"].ToString())));

            if (nObj == null || !shouldAdd)
                return;
            await context.News.AddAsync(nObj);

            JArray paragraphs = (JArray)newsObj["paragraphs"];
            int i = 0;
            foreach (var parObj in paragraphs)
            {
                var paragraph = new Paragraph
                {
                    NewsID = nObj.NewsID,
                    Content = parObj["paragraph"].ToString().Replace("\xFFFD", ""),
                    ParagraphNumber = i++
                };
                await context.Paragraphs.AddAsync(paragraph);
            }
        }

        private static string Today()
        {
            return CommonFunctions.UTCToEastern(NextGame.NEXT_GAME).ToString("yyyyMMdd");
        }

        private static string Yesterday()
        {
            return CommonFunctions.UTCToEastern(NextGame.PREVIOUS_GAME).ToString("yyyyMMdd");
        }
    }
}