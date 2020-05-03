using Newtonsoft.Json.Linq;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Security.Claims;
using System.Threading.Tasks;
using fantasy_hoops.Models.Enums;

namespace fantasy_hoops.Helpers
{
	public sealed class CommonFunctions
	{
		private CommonFunctions() {}
		
		public static CommonFunctions Instance => Nested.instance;

		private class Nested
		{
			// Explicit static constructor to tell C# compiler
			// not to mark type as beforefieldinit
			static Nested()
			{
			}

			internal static readonly CommonFunctions instance = new CommonFunctions();
		}

		private int? seasonYear;
		
		public string DOMAIN = "fantasyhoops.org";
		
		public string LineupPositionsOrder = "PG|SG|SF|PF|C";
		public int PRICE_FLOOR = 10;

		public DateTime EtcNow()
		{
			return UTCToEastern(DateTime.UtcNow);
		}

		public DateTime UTCToEastern(DateTime UTC)
		{
			TimeZoneInfo eastern = TimeZoneInfo.FindSystemTimeZoneById(Startup.Configuration["TimeZone"]);
			return TimeZoneInfo.ConvertTimeFromUtc(UTC, eastern);
		}

		public  DateTime EasternToUTC(DateTime eastern)
		{
			TimeZoneInfo.ConvertTimeBySystemTimeZoneId(eastern, TimeZoneInfo.Local.Id);
			TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById(Startup.Configuration["TimeZone"]);
			return TimeZoneInfo.ConvertTimeToUtc(eastern, easternZone);
		}

		public  HttpWebResponse GetResponse(string url)
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

		public  string ResponseToString(HttpWebResponse response)
		{
			string resp = "";
			using (StreamReader sr = new StreamReader(response.GetResponseStream()))
			{
				resp = sr.ReadToEnd();
			}
			return resp;
		}

		public  JArray GetGames(string date)
		{
			string url = "http://data.nba.net/10s/prod/v1/" + date + "/scoreboard.json";
			HttpWebResponse webResponse = GetResponse(url);
			if (webResponse == null)
				return null;
			string apiResponse = ResponseToString(webResponse);
			JObject json = JObject.Parse(apiResponse);
			return (JArray)json["games"];
		}

		public  int GetNextGame(int playerId)
		{
			string url = "http://data.nba.net/v2015/json/mobile_teams/nba/" + GetSeasonYear() + "/players/playercard_" + playerId + "_02.json";
			HttpWebResponse webResponse = GetResponse(url);
			if (webResponse == null)
				return -1;
			string apiResponse = ResponseToString(webResponse);
			JObject json = JObject.Parse(apiResponse);
			return (int)json["pl"]["ng"]["otid"];
		}

		public  int DaysInMonth()
		{
			int year = UTCToEastern(DateTime.UtcNow).Year;
			int month = UTCToEastern(DateTime.UtcNow).Month;
			return DateTime.DaysInMonth(year, month);
		}

		// Leaderboards and weekly scores
		public  DateTime GetLeaderboardDate(LeaderboardType type)
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
					return UTCToEastern(RuntimeUtils.PREVIOUS_GAME).Date;
			}
		}

		public int GetSeasonYearInt()
		{
			if (!seasonYear.HasValue)
			{
				seasonYear = int.Parse(GetSeasonYear());
			}

			return seasonYear.Value;
		}

		public string GetSeasonYear()
		{
			string url = "http://data.nba.net/10s/prod/v1/today.json";
			HttpWebResponse webResponse = GetResponse(url);
			if (webResponse == null)
				return null;
			string apiResponse = ResponseToString(webResponse);
			JObject json = JObject.Parse(apiResponse);
			return (string)json["seasonScheduleYear"];
		}

		public int GetIso8601WeekOfYear(DateTime time)
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
        
        public async Task<string> GetImageAsBase64Url(string url)
        {
	        using var client = new HttpClient();
	        var bytes = await client.GetByteArrayAsync(url);
	        return "image/jpeg;base64,     " + Convert.ToBase64String(bytes);
        }
        
        public string GetUsernameFromEmail(string email)
        {
	        if (!IsValidEmail(email))
	        {
		        return null;
	        }
	        int atIndex = email.IndexOf('@');
	        string username = email.Substring(0, atIndex);
	        return username;
        }
        
        public bool IsValidEmail(string email)
        {
	        try {
		        var addr = new MailAddress(email);
		        return addr.Address == email;
	        }
	        catch {
		        return false;
	        }
        }

        public  string GetUserIdFromClaims(ClaimsPrincipal user)
        {
	        return user.Claims.ToList()[0].Value;
        }
        
        public DateTime FirstDayOfWeek(DateTime date)
        {
	        switch (date.DayOfWeek)
	        {
		        case DayOfWeek.Monday:
			        return date;
		        case DayOfWeek.Sunday:
			        return date.AddDays(-6);
		        default:
			        return date.AddDays(-(int)date.DayOfWeek + 1);
	        }
        }

        public DateTime LastDayOfWeek(DateTime date)
        {
	        DateTime lastDayOfWeekDate = FirstDayOfWeek(date).AddDays(6);
	        return lastDayOfWeekDate;
        }

        public LeaderboardType ParseLeaderboardType(string type)
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
