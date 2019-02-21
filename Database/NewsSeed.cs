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

namespace fantasy_hoops.Database
{
    public class NewsSeed
    {
        public static void ExtractPreviews(GameContext context)
        {
            while (JobManager.RunningSchedules.Any(s => !s.Name.Contains("previews")))
                Thread.Sleep(15000);

            string today = Today();
            JArray tGames = CommonFunctions.GetGames(today);
            GetPreviews(context, today, tGames);
        }

        public static void ExtractRecaps(GameContext context)
        {
            while (JobManager.RunningSchedules.Any(s => !s.Name.Contains("recaps")))
                Thread.Sleep(15000);

            string yesterday = Yesterday();
            JArray yGames = CommonFunctions.GetGames(yesterday);
            GetRecaps(context, yesterday, yGames);
        }

        private static void GetPreviews(GameContext context, string date, JArray games)
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
                AddToDatabase(context, previewObj);

            context.SaveChanges();
        }

        public static void GetRecaps(GameContext context, string date, JArray games)
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
                AddToDatabase(context, newsObj);

            context.SaveChanges();
        }

        private static void AddToDatabase(GameContext context, JToken newsObj)
        {
            var nObj = new News
            {
                Date = DateTime.Parse(newsObj["pubDateUTC"].ToString()),
                Title = (string)newsObj["title"],
                hTeamID = (int)newsObj["hTeamID"],
                vTeamID = (int)newsObj["vTeamID"]
            };

            bool shouldAdd = !context.News.Any(x => x.Title.Equals((string)newsObj["title"])
            && x.Date.Equals(DateTime.Parse(newsObj["pubDateUTC"].ToString())));

            if (nObj == null || !shouldAdd)
                return;
            context.News.Add(nObj);

            JArray paragraphs = (JArray)newsObj["paragraphs"];
            int i = 0;
            foreach (var parObj in paragraphs)
            {
                var paragraph = new Paragraph
                {
                    NewsID = nObj.NewsID,
                    Content = (string)parObj["paragraph"],
                    ParagraphNumber = i++
                };
                context.Paragraphs.Add(paragraph);
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