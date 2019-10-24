using System.Net;
using System.Linq;
using System.Threading.Tasks;
using System;
using Newtonsoft.Json.Linq;
using fantasy_hoops.Models;
using fantasy_hoops.Helpers;
using FluentScheduler;

namespace fantasy_hoops.Database
{
    public class NewsSeed : IJob
    {
        private readonly GameContext _context;
        private readonly NewsType _newsType;

        public NewsSeed(NewsType newsType)
        {
            _context = new GameContext();
            _newsType = newsType;
        }

        public enum NewsType { PREVIEWS, RECAPS };

        public void ExtractPreviews()
        {
            string today = Today();
            JArray tGames = CommonFunctions.GetGames(today);
            GetPreviews(today, tGames).Wait();
        }

        public void ExtractRecaps()
        {
            string yesterday = Yesterday();
            JArray yGames = CommonFunctions.GetGames(yesterday);
            GetRecaps(yesterday, yGames).Wait();
        }

        private async Task GetPreviews(string date, JArray games)
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
                await AddToDatabaseAsync(previewObj);

            await _context.SaveChangesAsync();
        }

        private async Task GetRecaps(string date, JArray games)
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
                await AddToDatabaseAsync(newsObj);

            await _context.SaveChangesAsync();
        }

        private async Task AddToDatabaseAsync(JToken newsObj)
        {
            var nObj = new News
            {
                Date = DateTime.Parse(newsObj["pubDateUTC"].ToString()),
                Title = (string)newsObj["title"],
                hTeamID = (int)newsObj["hTeamID"],
                vTeamID = (int)newsObj["vTeamID"]
            };

            bool shouldAdd = !_context.News.Any(x => x.Title.Equals((string)newsObj["title"]));

            if (nObj == null || !shouldAdd)
                return;
            await _context.News.AddAsync(nObj);

            JArray paragraphs = (JArray)newsObj["paragraphs"];

            string firstParagraph = paragraphs[0]["paragraph"].ToString();
            int beginIndex = firstParagraph.IndexOf("(AP)");

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
                await _context.Paragraphs.AddAsync(paragraph);
            }
        }

        private string Today()
        {
            return CommonFunctions.UTCToEastern(NextGame.NEXT_GAME).ToString("yyyyMMdd");
        }

        private string Yesterday()
        {
            return CommonFunctions.UTCToEastern(NextGame.PREVIOUS_GAME).ToString("yyyyMMdd");
        }

        public void Execute()
        {
            switch (_newsType)
            {
                case NewsType.PREVIEWS:
                    ExtractPreviews();
                    break;
                case NewsType.RECAPS:
                    ExtractRecaps();
                    break;
                default:
                    break;
            }
        }
    }
}