using System;
using System.Collections.Generic;
using Castle.Core;
using Castle.Core.Internal;
using fantasy_hoops.Helpers;
using fantasy_hoops.Models.Enums;
using fantasy_hoops.Models.Tournaments;
using fantasy_hoops.Models.ViewModels;
using fantasy_hoops.Repositories.Interfaces;
using fantasy_hoops.Services;
using fantasy_hoops.Tests.Mocks;
using Moq;
using NUnit.Framework;

namespace fantasy_hoops.Tests.Services
{
    public class TournamentsServiceTests
    {
        private const string ROUTE_PATH = "/tournaments/invitations/";
        private const string TOURNAMENT_ICON_URL = "003-hands.bc2b8ba1.svg";
        
        [Test]
        public void TestFourUsersSchedule()
        {
            TournamentsService _tournamentsService = new TournamentsService(null, null, null, null, null);
            
            List<string> userIds = new List<string>
            {
                "A", "B", "C", "D"
            };
            List<Tuple<string, string>>[] generatedPermutations = _tournamentsService.GetMatchupsPermutations(userIds);
            Assert.NotNull(generatedPermutations);
            Assert.AreEqual(6, generatedPermutations.Length);
            Assert.AreEqual(2, generatedPermutations[0].Count);
            Assert.AreEqual(2, generatedPermutations[1].Count);
            Assert.AreEqual(2, generatedPermutations[2].Count);
            Assert.AreEqual(2, generatedPermutations[3].Count);
            Assert.AreEqual(2, generatedPermutations[4].Count);
            Assert.AreEqual(2, generatedPermutations[5].Count);
            foreach (var pairs in generatedPermutations)
            {
                Tuple<string, string> firstPair = pairs[0];
                Tuple<string, string> secondPair = pairs[1];
                Assert.IsTrue(firstPair.Item1 != firstPair.Item2);
                Assert.IsTrue(firstPair.Item1 != secondPair.Item1);
                Assert.IsTrue(firstPair.Item1 != secondPair.Item2);
                Assert.IsTrue(firstPair.Item2 != secondPair.Item1);
                Assert.IsTrue(firstPair.Item2 != secondPair.Item2);
                Assert.IsTrue(secondPair.Item1 != secondPair.Item2);
            }
        }
        
        [Test]
        public void TestFiveUsersSchedule()
        {
            TournamentsService _tournamentsService = new TournamentsService(null, null, null, null, null);
            
            List<string> userIds = new List<string>
            {
                "A", "B", "C", "D", "E"
            };
            List<Tuple<string, string>>[] generatedPermutations = _tournamentsService.GetMatchupsPermutations(userIds);
            Assert.Null(generatedPermutations);
        }
        
        [Test]
        public void TestZeroUsersSchedule()
        {
            TournamentsService _tournamentsService = new TournamentsService(null, null, null, null, null);
            
            List<string> userIds = new List<string>();
            List<Tuple<string, string>>[] generatedPermutations = _tournamentsService.GetMatchupsPermutations(userIds);
            Assert.Null(generatedPermutations);
        }

        [Test]
        public void TestCreateTournament()
        {
            var tournamentRepositoryMock = new Mock<ITournamentsRepository>();
            var notificationRepositoryMock = new NotificationRepositoryMock();
            
            tournamentRepositoryMock.Setup(mock => mock.CreateTournament(It.IsAny<Tournament>())).Returns(true);
            tournamentRepositoryMock.Setup(mock => mock.AddCreatorToTournament(It.IsAny<Tournament>()));
            tournamentRepositoryMock.Setup(mock => mock.ChangeInvitationStatus(It.IsAny<string>(), "AAA", RequestStatus.PENDING)).Returns(true);
            tournamentRepositoryMock.Setup(mock => mock.ChangeInvitationStatus(It.IsAny<string>(), "BBB", RequestStatus.PENDING)).Returns(true);
            tournamentRepositoryMock.Setup(mock => mock.ChangeInvitationStatus(It.IsAny<string>(), "CCC", RequestStatus.PENDING)).Returns(true);
            tournamentRepositoryMock.Setup(mock => mock.GetUpcomingStartDates())
                .Returns(new List<DateTime>{DateTime.Now.Date, DateTime.Now.Date.AddDays(2)});
            
            CreateTournamentViewModel model = new CreateTournamentViewModel
            {
                TournamentIcon = TOURNAMENT_ICON_URL,
                Contests = 2,
                UserFriends = new List<string>
                {
                    "AAA", "BBB", "CCC"
                }
            };
            TournamentsService _tournamentsService = new TournamentsService(tournamentRepositoryMock.Object, notificationRepositoryMock, null, null, null);
            Pair<bool, string> result = _tournamentsService.CreateTournament(model);
            Assert.NotNull(result);
            Assert.IsTrue(result.First);
            Assert.IsTrue(IsTournamentInvitationLinkValid(result.Second));
        }

        private bool IsTournamentInvitationLinkValid(string invitationUrl)
        {
            if (invitationUrl.IsNullOrEmpty())
            {
                return false;
            }

            bool containsDomain = invitationUrl.Contains($"https://{CommonFunctions.Instance.DOMAIN}");
            bool containsRoutePath = invitationUrl.Contains(ROUTE_PATH);
            string tournamentId = invitationUrl.Substring(invitationUrl.LastIndexOf('/') + 1);
            
            Guid guidOutput;
            bool isValidGuid = Guid.TryParse(tournamentId, out guidOutput);

            return containsDomain && containsRoutePath && isValidGuid;
        }

        [Test]
        public void TestGenerateContestsSuccess()
        {
            var notificationRepositoryMock = new Mock<INotificationRepository>();
            var tournamentRepositoryMock = new Mock<ITournamentsRepository>();
            tournamentRepositoryMock.Setup(mock => mock.GetUpcomingStartDates())
                .Returns(new List<DateTime>
                {
                    new DateTime(2020, 04, 27),
                    new DateTime(2020, 05, 04),
                    new DateTime(2020, 05, 11),
                });
            CreateTournamentViewModel model = new CreateTournamentViewModel
            {
                Contests = 3,
                StartDate = new DateTime(2020, 04, 27),
            };
            TournamentsService _tournamentsService = new TournamentsService(tournamentRepositoryMock.Object, notificationRepositoryMock.Object, null, null, null);
            
            Pair<List<Contest>, DateTime> contestsWithEndDate = _tournamentsService.GenerateContests(model);
            
            Assert.NotNull(contestsWithEndDate);
            Assert.AreEqual(3, contestsWithEndDate.First.Count);
            Assert.AreEqual(new DateTime(2020, 05, 18), contestsWithEndDate.Second);
        }
    }
}