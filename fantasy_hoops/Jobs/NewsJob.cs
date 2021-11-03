using System;
using System.Linq;
using fantasy_hoops.Database;
using fantasy_hoops.Helpers;
using fantasy_hoops.Models;
using fantasy_hoops.Models.Enums;
using Newtonsoft.Json.Linq;

namespace fantasy_hoops.Jobs
{
    public class NewsJob : ICronJob
    {
        private void ExtractPreviews(NewsType newsType)
        {
            string today = Today();
            var todayGames = CommonFunctions.Instance.GetGames(today);
            GetNews(today, todayGames, newsType);
        }

        private void ExtractRecaps(NewsType newsType)
        {
            string yesterday = Yesterday();
            var yesterdayGames = CommonFunctions.Instance.GetGames(yesterday);
            GetNews(yesterday, yesterdayGames, newsType);
        }
        
        private void GetNews(string dateString, JArray games, NewsType newsType)
        {
            var news = new JArray();
            foreach (var token in games)
            {
                var game = (JObject) token;
                var homeTeamId = game["hTeam"]["teamId"];
                var visitingTeaMid = game["vTeam"]["teamId"];
                
                string article = "http://data.nba.net/10s/prod/v1/" + dateString + "/" + game["gameId"]
                                 + "_" + GetNewsApiEndpoint(newsType) + ".json";

                var previewResponse = CommonFunctions.Instance.GetResponse(article);
                // no news for this game exists
                if (previewResponse == null) continue;

                var apiPreviewResponse = CommonFunctions.Instance.ResponseToString(previewResponse);
                var articleJson = JObject.Parse(apiPreviewResponse);

                articleJson.Add("hTeamID", homeTeamId);
                articleJson.Add("vTeamID", visitingTeaMid);
                news.Add(articleJson);
            }
            
            foreach (var jToken in news)
            {
                var previewObj = (JObject) jToken;
                AddToDatabase(previewObj, NewsType.PREVIEW);
            }
        }

        private string GetNewsApiEndpoint(NewsType newsType)
        {
            switch (newsType)
            {
                case NewsType.PREVIEW:
                    return "preview_article";
                case NewsType.RECAP:
                    return "recap_article";
                default:
                    throw new ArgumentOutOfRangeException(nameof(newsType), newsType, "Undefined API endpoint");
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
                Title = (string) newsObj["title"],
                hTeamID = hTeam?.TeamID ?? -1,
                vTeamID = vTeam?.TeamID ?? -1
            };

            JArray paragraphs = (JArray) newsObj["paragraphs"];
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

        public void Execute(NewsType newsType)
        {
            
            switch (newsType)
            {
                case NewsType.PREVIEW:
                    ExtractPreviews(newsType);
                    break;
                case NewsType.RECAP:
                    ExtractRecaps(newsType);
                    break;
            }
        }
    }
}