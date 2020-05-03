using System;
using System.Collections.Generic;
using System.Linq;
using fantasy_hoops.Dtos;
using fantasy_hoops.Models.Tournaments;
using fantasy_hoops.Repositories;
using fantasy_hoops.Repositories.Interfaces;
using fantasy_hoops.Tests.Mocks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using NUnit.Framework;

namespace fantasy_hoops.Tests.Repositories
{
    public class TournamentsRepositoryTests
    {
        private readonly ContextMock.Builder _contextBuilder = new ContextMock.Builder();
        
        [SetUp]
        public void SetUp()
        {
            var config = new MemoryConfigurationProvider(new MemoryConfigurationSource());
            config.Add("UseMock", "false");
            config.Add("TimeZone", "EST");
            Startup.Configuration = new ConfigurationRoot(new List<IConfigurationProvider>
            {
                config
            });
        }
        
        [Test]
        public void TestGetTournamentTypes()
        {
            ITournamentsRepository tournamentsRepository = new TournamentsRepository();
            var types =tournamentsRepository.GetTournamentTypes();

            Assert.NotNull(types);
            Assert.AreEqual(2, types.Count);
        }

        [Test]
        public void TestGetUpcomingStartDates()
        {
            var context = _contextBuilder
                .SetGames()
                .Build();
            
            ITournamentsRepository tournamentsRepository = new TournamentsRepository(context);
            List<DateTime> startDates = tournamentsRepository.GetUpcomingStartDates();
            
            Assert.NotNull(startDates);
            Assert.AreEqual(1, startDates.Count);
            Assert.AreEqual(new DateTime(2020, 05, 05).Date, startDates[0]);
        }

        [Test]
        public void TestGetTournamentById()
        {
            var context = _contextBuilder
                .SetTournaments()
                .SetGames()
                .Build();

            ITournamentsRepository tournamentsRepository = new TournamentsRepository(context);
            Tournament tournament = tournamentsRepository.GetTournamentById("tournament1");
            
            Assert.NotNull(tournament);
        }

        [Test]
        public void TestGetTournamentDetails()
        {
            var context = _contextBuilder
                .SetTournaments()
                .Build();
            
            ITournamentsRepository tournamentsRepository = new TournamentsRepository(context);
            TournamentDetailsDto tournamentDetails = tournamentsRepository.GetTournamentDetails("tournament1");
            
            Assert.NotNull(tournamentDetails);
            Assert.AreEqual("Tournament 1 Title", tournamentDetails.Title);
            Assert.AreEqual("Tournament 1 Description", tournamentDetails.Description);
            Assert.AreEqual(new DateTime(2020, 04, 02, 19, 00, 00), tournamentDetails.StartDate);
            Assert.AreEqual(new DateTime(2021, 04, 02, 19, 00 ,00), tournamentDetails.EndDate);
            Assert.AreEqual(1, tournamentDetails.Contests.Count);
            
        }

        [Test]
        public void TestGetTournamentsForStartDate()
        {
            var context = _contextBuilder
                .SetTournaments()
                .Build();
            
            ITournamentsRepository tournamentsRepository = new TournamentsRepository(context);
            List<Tournament> tournaments = tournamentsRepository.GetTournamentsForStartDate(new DateTime(2020, 04, 20));
            
            Assert.NotNull(tournaments);
            Assert.AreEqual(1, tournaments.Count);
        }

        [Test]
        public void TestUpdateTournamentUserStats()
        {
            var context = _contextBuilder
                .SetTournamentsUsers()
                .Build();

            TournamentUser tournamentUser = context.TournamentUsers.FirstOrDefault();
            
            ITournamentsRepository tournamentsRepository = new TournamentsRepository(context);
            tournamentsRepository.UpdateTournamentUserStats(tournamentUser, 16, 24, 81);
            
            Assert.NotNull(tournamentUser);
            Assert.AreEqual(16, tournamentUser.Wins);
            Assert.AreEqual(24, tournamentUser.Losses);
            Assert.AreEqual(81, tournamentUser.Points);
        }
    }
}