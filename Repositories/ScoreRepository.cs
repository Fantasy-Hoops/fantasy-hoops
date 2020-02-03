using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fantasy_hoops.Database;
using fantasy_hoops.Models;
using fantasy_hoops.Repositories.Interfaces;

namespace fantasy_hoops.Repositories
{
    public class ScoreRepository : IScoreRepository
    {

        private readonly GameContext _context;

        public ScoreRepository()
        {
            _context = new GameContext();
        }

        public bool AnyPlayerStatsExists(Player player)
        {
            return _context.Stats.Any(stats => stats.Player.NbaID == player.NbaID);
        }

        public double LastFiveAverage(Player player)
        {
            return _context.Stats
                            .Where(x => x.Player.NbaID == player.NbaID)
                            .OrderByDescending(s => s.Date)
                            .Take(5)
                            .Select(s => s.GS)
                            .Average();
        }
    }
}
