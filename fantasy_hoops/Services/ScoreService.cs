using fantasy_hoops.Models;
using System;
using fantasy_hoops.Helpers;
using fantasy_hoops.Repositories.Interfaces;
using fantasy_hoops.Services.Interfaces;

namespace fantasy_hoops.Services
{
    public class ScoreService : IScoreService
    {
        private readonly IScoreRepository _repository;

        public ScoreService(IScoreRepository repository)
        {
            _repository = repository;
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
            if (!_repository.AnyPlayerStatsExists(p))
                return CommonFunctions.PRICE_FLOOR;

            double GSavg = _repository.LastFiveAverage(p);
            return (int) ((p.FPPG + GSavg) * 7 / 5);
        }
    }
}