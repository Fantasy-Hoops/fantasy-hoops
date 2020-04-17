using Newtonsoft.Json.Linq;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using fantasy_hoops.Enums;
using fantasy_hoops.Jobs;

namespace fantasy_hoops.Helpers
{
	public class CommonFunctions
	{
		public const string DOMAIN = "fantasyhoops.org";
		public static string SEASON_YEAR = GetSeasonYear();
		public static DateTime EctNow = UTCToEastern(DateTime.UtcNow);

		public static string LineupPositionsOrder = "PG|SG|SF|PF|C";

		public static DateTime UTCToEastern(DateTime UTC)
		{
			TimeZoneInfo eastern = TimeZoneInfo.FindSystemTimeZoneById(Startup.Configuration["TimeZone"]);
			return TimeZoneInfo.ConvertTimeFromUtc(UTC, eastern);
		}

		public static DateTime EasternToUTC(DateTime eastern)
		{
			TimeZoneInfo.ConvertTimeBySystemTimeZoneId(eastern, TimeZoneInfo.Local.Id);
			TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById(Startup.Configuration["TimeZone"]);
			return TimeZoneInfo.ConvertTimeToUtc(eastern, easternZone);
		}

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

		public static int GetNextGame(int playerId)
		{
			string url = "http://data.nba.net/v2015/json/mobile_teams/nba/" + GetSeasonYear() + "/players/playercard_" + playerId + "_02.json";
			HttpWebResponse webResponse = GetResponse(url);
			if (webResponse == null)
				return -1;
			string apiResponse = ResponseToString(webResponse);
			JObject json = JObject.Parse(apiResponse);
			return (int)json["pl"]["ng"]["otid"];
		}

		public static int DaysInMonth()
		{
			int year = UTCToEastern(DateTime.UtcNow).Year;
			int month = UTCToEastern(DateTime.UtcNow).Month;
			return DateTime.DaysInMonth(year, month);
		}

		// Leaderboards and weekly scores
		public static DateTime GetLeaderboardDate(LeaderboardType type)
		{
			DateTime easternDate = UTCToEastern(DateTime.UtcNow);
			int dayOfWeek = (int)UTCToEastern(DateTime.UtcNow).DayOfWeek;
			int dayOfMonth = UTCToEastern(DateTime.UtcNow).Day;
			int dayOffset;
			
			switch (type)
			{
				case LeaderboardType.WEEKLY:
					dayOffset = dayOfWeek == 1
						? 7
						: dayOfWeek == 0 ? 6 : dayOfWeek - 1;
					return easternDate.AddDays(-dayOffset).Date;
				case LeaderboardType.MONTHLY:
					dayOffset = dayOfMonth == 1 ? DaysInMonth() : dayOfMonth - 1;
					return easternDate.AddDays(-dayOffset).Date;
				default:
					return UTCToEastern(NextGameJob.PREVIOUS_GAME).Date;
			}
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

		public static int GetIso8601WeekOfYear(DateTime time)
		{
			// Seriously cheat.  If its Monday, Tuesday or Wednesday, then it'll 
			// be the same week# as whatever Thursday, Friday or Saturday are,
			// and we always get those right
			DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
			if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
			{
				time = time.AddDays(3);
			}

			// Return the week of our adjusted day
			return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
		}
        
        public static async Task<string> GetImageAsBase64Url(string url)
        {
	        using var client = new HttpClient();
	        var bytes = await client.GetByteArrayAsync(url);
	        return "image/jpeg;base64,     " + Convert.ToBase64String(bytes);
        }
        
        public static string GetUsernameFromEmail(string email)
        {
	        int atIndex = email.IndexOf('@');
	        string username = email.Substring(0, atIndex);
	        return username;
        }

        public static string GetUserIdFromClaims(ClaimsPrincipal user)
        {
	        return user.Claims.ToList()[0].Value;
        }
        
        public static DateTime FirstDayOfWeek(DateTime date)
        {
	        int offset =  -1 * (7 + (date.DayOfWeek - DayOfWeek.Monday) % 7);
	        DateTime firstDayOfWeekDate = date.AddDays(offset);
	        return firstDayOfWeekDate;
        }

        public static DateTime LastDayOfWeek(DateTime date)
        {
	        DateTime lastDayOfWeekDate = FirstDayOfWeek(date).AddDays(6);
	        return lastDayOfWeekDate;
        }

        public static LeaderboardType ParseLeaderboardType(string type)
        {
	        switch (type)
	        {
		        case "daily":
			        return LeaderboardType.DAILY;
		        case "weekly":
			        return LeaderboardType.WEEKLY;
		        case "monthly":
			        return LeaderboardType.MONTHLY;
		        default:
			        return LeaderboardType.WEEKLY;
	        }
        }
    }
}
