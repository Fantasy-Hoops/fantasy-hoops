using fantasy_hoops.Models;
using fantasy_hoops.Repositories;
using fantasy_hoops.Repositories.Interfaces;
using fantasy_hoops.Tests.Mocks;
using NUnit.Framework;

namespace fantasy_hoops.Tests.Repositories
{
    public class ScoreRepositoryTests
    {
        private readonly ContextMock.Builder _contextBuilder = new ContextMock.Builder();

        [Test]
        public void TestAnyPlayerStatsExistsTrue()
        {
            var context = _contextBuilder
                .SetPlayers()
                .SetStats()
                .Build();

            IScoreRepository scoreRepository = new ScoreRepository(context);
            bool anyPlayerStatsExist = scoreRepository.AnyPlayerStatsExists(new Player
            {
                NbaID = 123123
            });

            Assert.IsTrue(anyPlayerStatsExist);
        }

        [Test]
        public void TestAnyPlayerStatsExistsFalse()
        {
            var context = _contextBuilder
                .SetPlayers()
                .SetStats()
                .Build();

            IScoreRepository scoreRepository = new ScoreRepository(context);
            bool anyPlayerStatsExist = scoreRepository.AnyPlayerStatsExists(new Player
            {
                NbaID = 456456
            });

            Assert.False(anyPlayerStatsExist);
        }

        [Test]
        public void TestLastFiveAverage()
        {
            var context = _contextBuilder
                .SetPlayers()
                .SetStats()
                .Build();

            IScoreRepository scoreRepository = new ScoreRepository(context);
            double lastFiveAverage = scoreRepository.LastFiveAverage(new Player
            {
                NbaID = 123123
            });

            Assert.AreEqual(30, lastFiveAverage);
        }
    }
}