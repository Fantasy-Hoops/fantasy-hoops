using fantasy_hoops.Database;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace fantasy_hoops.Helpers
{
    public class CommonFunctions
    {
        public static HttpWebResponse GetResponse(string url)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                request.KeepAlive = true;
                request.ContentType = "application/json";
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                return response;

            }
            catch (WebException)
            {
                return null;
            }
        }

        public static string ResponseToString(HttpWebResponse response)
        {
            string resp = "";
            using (StreamReader sr = new StreamReader(response.GetResponseStream()))
            {
                resp = sr.ReadToEnd();
            }
            return resp;
        }

        public static JArray GetGames(string date)
        {
            string url = "http://data.nba.net/10s/prod/v1/" + date + "/scoreboard.json";
            HttpWebResponse webResponse = GetResponse(url);
            if (webResponse == null)
                return null;
            string apiResponse = ResponseToString(webResponse);
            JObject json = JObject.Parse(apiResponse);
            return (JArray)json["games"];
        }

        public static int DaysInMonth()
        {
            int year = DateTime.UtcNow.Year;
            int month = DateTime.UtcNow.Month;
            return DateTime.DaysInMonth(year, month);
        }

        // Leaderboards and weekly scores
        public static DateTime GetDate(string type)
        {
            DateTime easternDate = DateTime.UtcNow;
            int dayOfWeek = (int)DateTime.UtcNow.DayOfWeek;
            int dayOfMonth = DateTime.UtcNow.Day;

            if (type.Equals("weekly"))
            {
                int dayOffset = dayOfWeek == 1
                    ? 7
                    : dayOfWeek == 0 ? 6 : dayOfWeek - 1;

                return easternDate.AddDays(-dayOffset);
            }
            if (type.Equals("monthly"))
            {
                int dayOffset = dayOfMonth == 1 ? DaysInMonth() : dayOfMonth - 1;
                return easternDate.AddDays(-dayOffset);
            }
            return NextGame.PREVIOUS_GAME;
        }

        public static string GetSeasonYear()
        {
            string url = "http://data.nba.net/10s/prod/v1/today.json";
            HttpWebResponse webResponse = GetResponse(url);
            if (webResponse == null)
                return null;
            string apiResponse = ResponseToString(webResponse);
            JObject json = JObject.Parse(apiResponse);
            return (string)json["seasonScheduleYear"];
        }
    }
}
