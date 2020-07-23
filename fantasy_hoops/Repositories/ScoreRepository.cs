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
            return new GameContext().Stats
                .Where(x => x.Player.NbaID == player.NbaID
                            && !x.Score.ToLower().Contains("live"))
                .OrderByDescending(s => s.Date)
                .Take(5)
                .Select(s => s.GS)
                .Average();
        }
    }
}