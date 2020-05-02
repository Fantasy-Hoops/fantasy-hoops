using fantasy_hoops.Helpers;
using fantasy_hoops.Models;
using fantasy_hoops.Repositories.Interfaces;
using fantasy_hoops.Services;
using fantasy_hoops.Services.Interfaces;
using Moq;
using NUnit.Framework;

namespace fantasy_hoops.Tests.Services
{
    public class ScoreServiceTests
    {
        
        [Test]
        public void TestFantasyPointsHakeemStats()
        {
            IScoreService scoreService = new ScoreService(null);
            // One of most dominant perfomances of all time
            // https://www.basketball-reference.com/boxscores/198703100HOU.html
            var value = scoreService.GetFantasyPoints(38, 7, 10, 6, 7, 12, 4);
            Assert.AreEqual(120.4D, value);
        }

        [Test]
        public void TestFantasyPointsZeroStatsZeroPoints()
        {
            IScoreService scoreService = new ScoreService(null);
            var value = scoreService.GetFantasyPoints(0, 0, 0, 0, 0, 0, 0);
            Assert.AreEqual(0D, value);
        }

        [Test]
        public void TestFantasyPointsTurnoversNegative()
        {
            IScoreService scoreService = new ScoreService(null);
            var value = scoreService.GetFantasyPoints(0, 0, 0, 0, 0, 0, 5);
            Assert.AreEqual(-5D, value);
        }

        [Test]
        public void TestGameScoreHakeemStatsCalculated()
        {
            IScoreService scoreService = new ScoreService(null);
            var value = scoreService.GetGameScore(38, 14, 10, 7, 7, 6, 12, 29, 4, 5, 4);
            Assert.AreEqual(44.4D, value);
        }

        [Test]
        public void TestGameScoreZeroStatsZeroScore()
        {
            IScoreService scoreService = new ScoreService(null);
            var value = scoreService.GetGameScore(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            Assert.AreEqual(0D, value);
        }

        [Test]
        public void TestGameScoreBadStatsNegativeScore()
        {
            IScoreService scoreService = new ScoreService(null);
            var value = scoreService.GetGameScore(0, 0, 0, 0, 0, 0, 0, 3, 5, 5, 4);
            Assert.AreEqual(value, -10.1D);
        }

        [Test]
        public void TestPriceZeroStatsZeroPrice()
        {
            var playerMock = new Mock<Player>();
            
            var scoreRepositoryMock = new Mock<IScoreRepository>();
            scoreRepositoryMock.Setup(mock => mock.AnyPlayerStatsExists(playerMock.Object)).Returns(false);
            scoreRepositoryMock.Setup(mock => mock.LastFiveAverage(playerMock.Object)).Returns(50d);
            
            IScoreService scoreService = new ScoreService(scoreRepositoryMock.Object);
            var value = scoreService.GetPrice(playerMock.Object);
            Assert.AreEqual(CommonFunctions.Instance.PRICE_FLOOR, value);
        }

        [Test]
        public void TestPricePositiveStatsCalculated()
        {
            var playerMock = new Mock<Player>();
            
            var scoreRepositoryMock = new Mock<IScoreRepository>();
            scoreRepositoryMock.Setup(mock => mock.AnyPlayerStatsExists(playerMock.Object)).Returns(true);
            scoreRepositoryMock.Setup(mock => mock.LastFiveAverage(playerMock.Object)).Returns(100d);
            
            IScoreService scoreService = new ScoreService(scoreRepositoryMock.Object);
            
            var value = scoreService.GetPrice(playerMock.Object);
            Assert.AreEqual(140, value);
        }
    }
}