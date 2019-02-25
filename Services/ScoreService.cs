﻿using fantasy_hoops.Database;
using fantasy_hoops.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fantasy_hoops.Services
{
    public class ScoreService : IScoreService
    {

        private readonly GameContext _context;

        public ScoreService()
        {
            _context = new GameContext();
        }

        public double GetFantasyPoints(int points, int defensiveRebounds, int offensiveRebounds,
            int assists, int steals, int blocks, int turnovers)
        {
            return Math.Round(points + 1.2 * (defensiveRebounds + offensiveRebounds)
                    + 1.5 * assists + 3 * steals + 3 * blocks - turnovers, 2);
        }

        public double GetGameScore(int points, int fieldGoalsMade, int offensiveRebounds,
            int defensiveRebounds, int steals, int assists, int blocks, int fieldGoalsAttempted,
            int freeThrowsMissed, int fouls, int turnovers)
        {
            return Math.Round(points + 0.4 * fieldGoalsMade + 0.7 * offensiveRebounds
                    + 0.3 * defensiveRebounds + steals + 0.7 * assists + 0.7 * blocks
                    - 0.7 * fieldGoalsAttempted - 0.4 * freeThrowsMissed - 0.4 * fouls - turnovers, 2);
        }

        public int GetPrice(Player p)
        {
            double GSavg = 0;
            if (_context.Stats.Where(x => x.Player.NbaID == p.NbaID).Count() < 1)
                return PlayerSeed.PRICE_FLOOR;

            try
            {
                double GSsum = _context.Stats
                            .Where(x => x.Player.NbaID == p.NbaID)
                            .OrderByDescending(s => s.Date)
                            .Take(5)
                            .Select(s => s.GS)
                            .Sum();

                int GScount = _context.Stats
                            .Where(x => x.Player.NbaID == p.NbaID)
                            .Take(5)
                            .Count();

                GSavg = GSsum / GScount;
            }
            catch { }
            return (int)((p.FPPG + GSavg) * 7 / 5);
        }
    }
}
