using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using fantasy_hoops.Database;
using fantasy_hoops.Helpers;
using fantasy_hoops.Models;
using FluentScheduler;
using Newtonsoft.Json.Linq;

namespace fantasy_hoops.Jobs
{
    public class NewsJob : IJob
    {
        private readonly GameContext _context;
        private readonly NewsType _newsType;

        public NewsJob(NewsType newsType)
        {
            _context = new GameContext();
            _newsType = newsType;
        }

        public void ExtractPreviews()
        {
            string today = Today();
            JArray tGames = CommonFunctions.GetGames(today);
            GetPreviews(today, tGames);
        }

        public void ExtractRecaps()
        {
            string yesterday = Yesterday();
            JArray yGames = CommonFunctions.GetGames(yesterday);
            GetRecaps(yesterday, yGames);
        }

        private void GetPreviews(string date, JArray games)
        {
            JArray previews = new JArray();
            foreach (JObject game in games)
            {
                string preview = "http://data.nba.net/10s/prod/v1/" + date + "/" + game["gameId"] + "_preview_article.json";
                JObject previewJson;
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

            foreach (var jToken in previews)
            {
                var previewObj = (JObject) jToken;
                AddToDatabaseAsync(previewObj, NewsType.PREVIEW);
            }

            _context.SaveChanges();
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

            foreach (var jToken in news)
            {
                var newsObj = (JObject) jToken;
                AddToDatabaseAsync(newsObj, NewsType.RECAP);
            }

            _context.SaveChanges();
        }

        private void AddToDatabaseAsync(JToken newsObj, NewsType type)
        {
            Team hTeam = _context.Teams.FirstOrDefault(team => team.NbaID == (int) newsObj["hTeamID"]);
            Team vTeam = _context.Teams.FirstOrDefault(team => team.NbaID == (int) newsObj["vTeamID"]);
            var nObj = new News
            {
                Type = type,
                Date = DateTime.Parse(newsObj["pubDateUTC"].ToString()),
                Title = (string)newsObj["title"],
                hTeamID = hTeam?.TeamID ?? -1,
                vTeamID = vTeam?.TeamID ?? -1
            };

            JArray paragraphs = (JArray)newsObj["paragraphs"];
            bool shouldAdd = !_context.News
                .Include(news => news.hTeam)
                .Include(news => news.vTeam)
                .AsEnumerable()
                .Any(x => x.hTeamID == nObj.hTeamID
                          && x.vTeamID == nObj.vTeamID
                          && x.Date.Equals(nObj.Date));

            if (!shouldAdd)
                return;
            _context.News.Add(nObj);

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
                _context.Paragraphs.Add(paragraph);
            }
            _context.SaveChanges();
        }

        private string Today()
        {
            return CommonFunctions.UTCToEastern(NextGameJob.NEXT_GAME).ToString("yyyyMMdd");
        }

        private string Yesterday()
        {
            return CommonFunctions.UTCToEastern(NextGameJob.PREVIOUS_GAME.AddDays(-1)).ToString("yyyyMMdd");
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