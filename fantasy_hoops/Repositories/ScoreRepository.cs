using System.Data.Entity;
using System.Linq;
using fantasy_hoops.Database;
using fantasy_hoops.Models;
using fantasy_hoops.Repositories.Interfaces;

namespace fantasy_hoops.Repositories
{
    public class ScoreRepository : IScoreRepository
    {
        private readonly GameContext _context;

        public ScoreRepository(GameContext context = null)
        {
            _context = context ?? new GameContext();
        }

        public bool AnyPlayerStatsExists(Player player)
        {
            return new GameContext().Stats.Any(stats => stats.Player.NbaID == player.NbaID);
        }

        public double LastFiveAverage(Player player)
        {
            var playerStats = new GameContext().Stats
                .Where(x => x.Player.NbaID == player.NbaID
                            && !x.Score.ToLower().Contains("live")).ToList();
            if (playerStats.Count == 0)
            {
                return 0.0d;
            }
            
            return playerStats
                .OrderByDescending(s => s.Date)
                .Select(s => s.GS)
                .Take(5)
                .Average();
        }
    }
}