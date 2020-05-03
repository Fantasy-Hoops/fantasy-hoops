using System;
using System.Collections.Generic;
using fantasy_hoops.Helpers;
using fantasy_hoops.Models.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using NUnit.Framework;

namespace fantasy_hoops.Tests.Helpers
{
    public class CommonFunctionsTests
    {
        [SetUp]
        public void SetUp()
        {
            var config = new MemoryConfigurationProvider(new MemoryConfigurationSource());
            config.Add("TimeZone", "EST");
            Startup.Configuration = new ConfigurationRoot(new List<IConfigurationProvider>
            {
                config
            });
        }
        
        [Test]
        public void TestUTCtoEasternConversion01am()
        {
            DateTime utcDate = new DateTime(2020, 01, 01, 01, 00, 00);
            DateTime easternDate = CommonFunctions.Instance.UTCToEastern(utcDate);
            
            Assert.AreEqual(new DateTime(2019, 12, 31, 20, 00 ,00), easternDate);
        }
        
        [Test]
        public void TestUTCtoEasternConversion12pm()
        {
            DateTime utcDate = new DateTime(2020, 01, 01, 12, 00, 00);
            DateTime easternDate = CommonFunctions.Instance.UTCToEastern(utcDate);
            
            Assert.AreEqual(new DateTime(2020, 01, 01, 07, 00 ,00), easternDate);
        }
        
        [Test]
        public void TestEasternToUTCConversion11pm()
        {
            DateTime easternDate = new DateTime(2020, 01, 01, 23, 00, 00);
            DateTime utcDate = CommonFunctions.Instance.EasternToUTC(easternDate);
            
            Assert.AreEqual(new DateTime(2020, 01, 02, 04, 00 ,00), utcDate);
        }
        
        [Test]
        public void TestEasternToUTCConversion12pm()
        {
            DateTime easternDate = new DateTime(2020, 01, 01, 12, 00, 00);
            DateTime utcDate = CommonFunctions.Instance.EasternToUTC(easternDate);
            
            Assert.AreEqual(new DateTime(2020, 01, 01, 17, 00 ,00), utcDate);
        }

        [Test]
        public void TestGetLeaderboardDateDaily()
        {
            DateTime dailyDate = CommonFunctions.Instance.GetLeaderboardDate(LeaderboardType.DAILY);
            Assert.NotNull(dailyDate);
            Assert.AreEqual(DateTime.UtcNow.Date,  dailyDate);
        }

        [Test]
        public void TestGetLeaderboardDateWeekly()
        {
            DateTime expectedDate = CommonFunctions.Instance.FirstDayOfWeek(DateTime.UtcNow).Date;
            DateTime weeklyDate = CommonFunctions.Instance.GetLeaderboardDate(LeaderboardType.WEEKLY);
            Assert.NotNull(weeklyDate);
            Assert.AreEqual(expectedDate,  weeklyDate);
        }

        [Test]
        public void TestGetLeaderboardDateMonthly()
        {
            DateTime expectedDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
            DateTime monthlyDate = CommonFunctions.Instance.GetLeaderboardDate(LeaderboardType.MONTHLY);
            Assert.NotNull(monthlyDate);
            Assert.AreEqual(expectedDate,  monthlyDate);
        }

        [Test]
        public void TestGetWeekNumberForDateFirstWeek()
        {
            DateTime firstWeekDate = new DateTime(2020, 01, 01);
            int weekNumber = CommonFunctions.Instance.GetIso8601WeekOfYear(firstWeekDate);
            Assert.AreEqual(1, weekNumber);
        }

        [Test]
        public void TestGetWeekNumberForDateLastWeek()
        {
            DateTime lastWeekDate = new DateTime(2020, 12, 31);
            int weekNumber = CommonFunctions.Instance.GetIso8601WeekOfYear(lastWeekDate);
            Assert.AreEqual(53, weekNumber);
        }

        [Test]
        public void TestGetWeekNumberForDateWeek24()
        {
            DateTime week24Date = new DateTime(2020, 06, 08);
            int weekNumber = CommonFunctions.Instance.GetIso8601WeekOfYear(week24Date);
            Assert.AreEqual(24, weekNumber);
        }

        [Test]
        public void TestGetUsernameFromEmailWrongSuccess()
        {
            String username = CommonFunctions.Instance.GetUsernameFromEmail("test@test.com");
            Assert.AreEqual("test", username);
        }

        [Test]
        public void TestGetUsernameFromEmailWrongEmail()
        {
            String username = CommonFunctions.Instance.GetUsernameFromEmail("randomText");
            Assert.Null(username);
        }

        [Test]
        public void TestFirstDayOfWeekSunday()
        {
            DateTime firsDayOfWeekExpected = new DateTime(2020, 05, 04);
            DateTime sundayDate = new DateTime(2020, 05, 10);
            DateTime firstDayOfWeekDate = CommonFunctions.Instance.FirstDayOfWeek(sundayDate);
            
            Assert.AreEqual(firsDayOfWeekExpected, firstDayOfWeekDate);
        }

        [Test]
        public void TestFirstDayOfWeekMonday()
        {
            DateTime firsDayOfWeekExpected = new DateTime(2020, 05, 04);
            DateTime mondayDate = new DateTime(2020, 05, 04);
            DateTime firstDayOfWeekDate = CommonFunctions.Instance.FirstDayOfWeek(mondayDate);
            
            Assert.AreEqual(firsDayOfWeekExpected, firstDayOfWeekDate);
        }

        [Test]
        public void TestFirstDayOfWeekWednesday()
        {
            DateTime firsDayOfWeekExpected = new DateTime(2020, 05, 04);
            DateTime wednesdayDate = new DateTime(2020, 05, 06);
            DateTime firstDayOfWeekDate = CommonFunctions.Instance.FirstDayOfWeek(wednesdayDate);
            
            Assert.AreEqual(firsDayOfWeekExpected, firstDayOfWeekDate);
        }
    }
}