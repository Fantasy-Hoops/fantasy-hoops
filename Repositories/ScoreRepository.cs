using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fantasy_hoops.Database;
using fantasy_hoops.Models;

namespace fantasy_hoops.Repositories
{
    public class ScoreRepository : IScoreRepository
    {

        private readonly GameContext _context;

        public ScoreRepository(GameContext context)
        {
            _context = context;
        }

        public bool AnyPlayerStatsExists(Player player)
        {
            return _context.Stats.Any(stats => stats.Player.NbaID == player.NbaID);
        }

        public double LastFiveSum(Player player)
        {
            return _context.Stats
                            .Where(x => x.Player.NbaID == player.NbaID)
                            .OrderByDescending(s => s.Date)
                            .Take(5)
                            .Select(s => s.GS)
                            .Sum();
        }

        public int LastGamesCount(Player player)
        {
            return _context.Stats
                            .Where(x => x.Player.NbaID == player.NbaID)
                            .Take(5)
                            .Count();
        }
    }
}
