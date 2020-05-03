using System;
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
    public class NewsJob : IJob
    {
        private readonly NewsType _newsType;

        public NewsJob(NewsType newsType)
        {
            _newsType = newsType;
        }

        private void ExtractPreviews()
        {
            string today = Today();
            JArray tGames = CommonFunctions.Instance.GetGames(today);
            GetPreviews(today, tGames);
        }

        private void ExtractRecaps()
        {
            string yesterday = Yesterday();
            JArray yGames = CommonFunctions.Instance.GetGames(yesterday);
            GetRecaps(yesterday, yGames);
        }

        private void GetPreviews(string date, JArray games)
        {
            JArray previews = new JArray();
            foreach (var jToken in games)
            {
                var game = (JObject) jToken;
                string preview = "http://data.nba.net/10s/prod/v1/" + date + "/" + game["gameId"] + "_preview_article.json";
                JObject previewJson;
                try
                {
                    HttpWebResponse previewResponse = CommonFunctions.Instance.GetResponse(preview);
                    string apiPreviewResponse = CommonFunctions.Instance.ResponseToString(previewResponse);
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

            foreach (var jToken in previews)
            {
                var previewObj = (JObject) jToken;
                AddToDatabase(previewObj, NewsType.PREVIEW);
            }
        }

        private void GetRecaps(string date, JArray games)
        {
            JArray news = new JArray();
            foreach (var jToken in games)
            {
                var game = (JObject) jToken;
                string recap = "http://data.nba.net/10s/prod/v1/" + date + "/" + game["gameId"] + "_recap_article.json";
                JObject recapJson;
                try
                {
                    HttpWebResponse recapResponse = CommonFunctions.Instance.GetResponse(recap);
                    string apiRecapResponse = CommonFunctions.Instance.ResponseToString(recapResponse);
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

            foreach (var jToken in news)
            {
                var newsObj = (JObject) jToken;
                AddToDatabase(newsObj, NewsType.RECAP);
            }
        }

        private void AddToDatabase(JToken newsObj, NewsType type)
        {
            GameContext context = new GameContext();
            Team hTeam = context.Teams.FirstOrDefault(team => team.NbaID == (int) newsObj["hTeamID"]);
            Team vTeam = context.Teams.FirstOrDefault(team => team.NbaID == (int) newsObj["vTeamID"]);
            var nObj = new News
            {
                Type = type,
                Date = DateTime.Parse(newsObj["pubDateUTC"].ToString()),
                Title = (string)newsObj["title"],
                hTeamID = hTeam?.TeamID ?? -1,
                vTeamID = vTeam?.TeamID ?? -1
            };

            JArray paragraphs = (JArray)newsObj["paragraphs"];
            bool shouldAdd = !context.News
                .AsEnumerable()
                .Any(x => x.hTeamID == nObj.hTeamID
                          && x.vTeamID == nObj.vTeamID
                          && nObj.Date >= x.Date);

            if (!shouldAdd)
                return;
            context.News.Add(nObj);
            context.SaveChanges();

            string firstParagraph = paragraphs[0]["paragraph"].ToString();
            int beginIndex = firstParagraph.IndexOf("(AP)", StringComparison.Ordinal);

            if (beginIndex != -1)
                paragraphs[0]["paragraph"] = firstParagraph.Substring(beginIndex + 5);
            int i = 0;
            foreach (var parObj in paragraphs)
            {
                var paragraph = new Paragraph
                {
                    NewsID = nObj.NewsID,
                    Content = parObj["paragraph"].ToString().Replace("\xFFFD", ""),
                    ParagraphNumber = i++
                };
                context.Paragraphs.Add(paragraph);
            }

            context.SaveChanges();
        }

        private string Today()
        {
            return CommonFunctions.Instance.UTCToEastern(RuntimeUtils.NEXT_GAME).ToString("yyyyMMdd");
        }

        private string Yesterday()
        {
            return CommonFunctions.Instance.UTCToEastern(RuntimeUtils.PREVIOUS_GAME.AddDays(-1)).ToString("yyyyMMdd");
        }

        public void Execute()
        {
            switch (_newsType)
            {
                case NewsType.PREVIEW:
                    ExtractPreviews();
                    break;
                case NewsType.RECAP:
                    ExtractRecaps();
                    break;
            }
        }
    }
}