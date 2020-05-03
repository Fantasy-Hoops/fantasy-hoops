using System;
using System.Collections.Generic;
using System.Linq;
using fantasy_hoops.Dtos;
using fantasy_hoops.Helpers;
using fantasy_hoops.Models;
using fantasy_hoops.Models.ViewModels;
using fantasy_hoops.Repositories;
using fantasy_hoops.Repositories.Interfaces;
using fantasy_hoops.Tests.Mocks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using NUnit.Framework;

namespace fantasy_hoops.Tests.Repositories
{
    public class LineupRepositoryTests
    {
        private readonly ContextMock.Builder _contextBuilder = new ContextMock.Builder();
        
        [SetUp]
        public void SetUp()
        {
            RuntimeUtils.NEXT_GAME = new DateTime(2020, 04, 20, 12 ,00,00);
            var config = new MemoryConfigurationProvider(new MemoryConfigurationSource());
            config.Add("TimeZone", "EST");
            Startup.Configuration = new ConfigurationRoot(new List<IConfigurationProvider>
            {
                config
            });
        }
        
        [Test]
        public void TestGetLineup()
        {
            var context = _contextBuilder
                .SetUsers()
                .SetPlayers()
                .SetUserLineups()
                .Build();
            
            ILineupRepository lineupRepository = new LineupRepository(context);
            UserLineup userLineup = lineupRepository.GetLineup("xxx");
            
            Assert.NotNull(userLineup);
            Assert.AreEqual(123, userLineup.PgID);
            Assert.AreEqual(456, userLineup.SgID);
            Assert.AreEqual(321, userLineup.SfID);
            Assert.AreEqual(654, userLineup.PfID);
            Assert.AreEqual(789, userLineup.CID);
        }

        [Test]
        public void TestAddLineup()
        {
            var context = _contextBuilder
                .SetPlayers()
                .SetUserLineups()
                .Build();
            
            ILineupRepository lineupRepository = new LineupRepository(context);
                lineupRepository.AddLineup(new SubmitLineupViewModel());
                
                Assert.AreEqual(3, context.UserLineups.Count());
        }

        [Test]
        public void TestUpdateLineup()
        {
            var context = _contextBuilder
                .SetPlayers()
                .SetUserLineups()
                .Build();
            
            ILineupRepository lineupRepository = new LineupRepository(context);
            lineupRepository.UpdateLineup(new SubmitLineupViewModel
            {
                UserID = "xxx",
                CID = 444
            });
            
            Assert.AreEqual(2, context.UserLineups.Count());
            Assert.True(context.UserLineups.Any(lineup => lineup.UserID.Equals("xxx") && lineup.CID == 444));
        }

        [Test]
        public void TestGetLineupPrice()
        {
            var context = _contextBuilder
                .SetPlayers()
                .SetUserLineups()
                .Build();
            
            ILineupRepository lineupRepository = new LineupRepository(context);
            int price = lineupRepository.GetLineupPrice(new SubmitLineupViewModel
            {
                PgID = 123,
                PgPrice = 60,
                SgID = 456,
                SgPrice = 60,
                SfID = 321,
                SfPrice = 60,
                PfID = 654,
                PfPrice = 60,
                CID = 789,
                CPrice = 60
            });
            
            Assert.AreEqual(300, price);
        }

        [Test]
        public void TestArePricesCorrect()
        {
            var context = _contextBuilder
                .SetPlayers()
                .Build();
            
            ILineupRepository lineupRepository = new LineupRepository(context);
            bool arePricesCorrect = lineupRepository.ArePricesCorrect(new SubmitLineupViewModel
            {
                PgID = 123,
                PgPrice = 60,
                SgID = 456,
                SgPrice = 60,
                SfID = 321,
                SfPrice = 60,
                PfID = 654,
                PfPrice = 60,
                CID = 789,
                CPrice = 60
            });
            
            Assert.True(arePricesCorrect);
        }

        [Test]
        public void TestIsUpdatingTrue()
        {
            var context = _contextBuilder
                .SetUsers()
                .SetUserLineups()
                .Build();
            
            ILineupRepository lineupRepository = new LineupRepository(context);
            bool isUpdating = lineupRepository.IsUpdating("xxx");
            
            Assert.True(isUpdating);
        }

        [Test]
        public void TestIsUpdatingFalse()
        {
            var context = _contextBuilder
                .SetUsers()
                .Build();
            
            ILineupRepository lineupRepository = new LineupRepository(context);
            bool isUpdating = lineupRepository.IsUpdating("ppp");
            
            Assert.False(isUpdating);
        }

        [Test]
        public void TestAreNotPlayingPlayersTrue()
        {
            var context = _contextBuilder
                .SetPlayers()
                .SetUserLineups()
                .Build();
            
            ILineupRepository lineupRepository = new LineupRepository(context);
            bool areNotPlayingPlayers = lineupRepository.AreNotPlayingPlayers(new SubmitLineupViewModel
            {
                PgID = 123,
                SgID = 456,
                SfID = 321,
                PfID = 654,
                CID = 789
            });
            
            Assert.True(areNotPlayingPlayers);
        }

        [Test]
        public void TestAreNotPlayingPlayersFalse()
        {
            var context = _contextBuilder
                .SetPlayers()
                .SetUserLineups()
                .Build();

            foreach (var player in context.Players)
            {
                player.IsPlaying = true;
            }
            context.SaveChanges();
            
            ILineupRepository lineupRepository = new LineupRepository(context);
            bool areNotPlayingPlayers = lineupRepository.AreNotPlayingPlayers(new SubmitLineupViewModel
            {
                PgID = 123,
                SgID = 456,
                SfID = 321,
                PfID = 654,
                CID = 789
            });
            
            Assert.False(areNotPlayingPlayers);
            
            foreach (var player in context.Players)
            {
                player.IsPlaying = false;
            }
            context.SaveChanges();
        }

        [Test]
        public void TestGetUsersSelectedIds()
        {
            var context = _contextBuilder
                .SetUsers()
                .SetUserLineups()
                .Build();
            
            ILineupRepository lineupRepository = new LineupRepository(context);
            List<String> userSelectedIds = lineupRepository.GetUserSelectedIds();
            
            Assert.AreEqual(2, userSelectedIds.Count);
            Assert.AreEqual("xxx", userSelectedIds[0]);
            Assert.AreEqual("yyy", userSelectedIds[1]);
        }

        [Test]
        public void TestGetUserCurrentLineup()
        {
            var context = _contextBuilder
                .SetPlayers()
                .SetUsers()
                .SetUserLineups()
                .Build();
            
            ILineupRepository lineupRepository = new LineupRepository(context);
            UserLeaderboardRecordDto userLineup = lineupRepository.GetUserCurrentLineup("xxx");
            
            Assert.NotNull(userLineup);
            Assert.AreEqual("2020-04-20", userLineup.LongDate);
        }

        [Test]
        public void TestGetRecentLineups()
        {
            var context = _contextBuilder
                .SetUserLineups(new List<UserLineup>
                {
                    new UserLineup
                    {
                        ID = 111,
                        UserID = "ppp",
                        IsCalculated = true
                    },
                    new UserLineup
                    {
                        ID = 222,
                        UserID = "ppp",
                        IsCalculated = true
                    },
                    new UserLineup
                    {
                        ID = 333,
                        UserID = "ppp",
                        IsCalculated = true
                    }
                })
                .Build();
            
            ILineupRepository lineupRepository = new LineupRepository(context);
            List<UserLeaderboardRecordDto> recentLineups= lineupRepository.GetRecentLineups("ppp", 0, 2);
            
            Assert.NotNull(recentLineups);
            Assert.AreEqual(2, recentLineups.Count);
        }
    }
}