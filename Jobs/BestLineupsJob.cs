using System;
using System.Collections.Generic;
using System.Linq;
using fantasy_hoops.Controllers;
using fantasy_hoops.Database;
using fantasy_hoops.Dtos;
using fantasy_hoops.Helpers;
using fantasy_hoops.Models;
using FluentScheduler;
using Microsoft.EntityFrameworkCore;

namespace fantasy_hoops.Jobs
{
    public class BestLineupsJob : IJob
    {
        private GameContext _context;
        
        public BestLineupsJob()
        {
            _context = new GameContext();
        }

        public void Execute()
        {
            var previousGameStats = GetPreviousGameStats();
            var lineupsCombinations = CrossProductFunctions
                .CrossProduct(previousGameStats)
                .Select(lineup => new
                {
                    Players = lineup.Select(l => new PlayersBestLineups
                    {
                        PlayerID = l.Player.PlayerID,
                        FP = l.FP,
                        Price = l.Price
                    }).ToList(),
                    FP = lineup.Sum(l => l.FP),
                    TotalPrice = lineup.Sum(players => players.Price)
                })
                .Where(lineup => lineup.TotalPrice < LineupController.MAX_PRICE)
                .OrderByDescending(lineup => lineup.FP)
                .Take(10)
                .ToList();
            foreach (var lineup in lineupsCombinations)
            {
                var players = lineup.Players.Select(p => p.PlayerID).ToList();
                var bestLineup = _context.BestLineups
                    .Include(x => x.Lineup)
                    .Select(x => new
                    {
                        x.LineupPrice,
                        x.Date,
                        x.TotalFP,
                        playerIds = x.Lineup.Select(l => l.PlayerID).ToList()
                    })
                    .AsEnumerable()
                    .FirstOrDefault(x => Math.Round(x.TotalFP, 1).Equals(Math.Round(lineup.FP, 1))
                                         && x.LineupPrice == lineup.TotalPrice
                                         && x.Date.Equals(NextGameJob.PREVIOUS_GAME.Date)
                                         && x.playerIds.All(players.Contains));
                if (bestLineup == null)
                {
                    _context.BestLineups.Add(new BestLineup
                    {
                        Date = NextGameJob.PREVIOUS_GAME.Date,
                        Lineup = lineup.Players,
                        TotalFP = Math.Round(lineup.FP, 1),
                        LineupPrice = lineup.TotalPrice
                    });
                }
            }
            _context.SaveChanges();
        }

        private IDictionary<string, List<LineupPlayerDto>> GetPreviousGameStats()
        {
            return _context.Stats
                .Include(stats => stats.Player)
                .ThenInclude(player => player.Team)
                .Where(stats => stats.Date.Date.Equals(NextGameJob.PREVIOUS_GAME.Date))
                .Select(stats => new LineupPlayerDto
                {
                    Player = stats.Player,
                    FP = stats.FP,
                    Price = stats.Price
                })
                .AsEnumerable()
                .GroupBy(stats => stats.Player.Position)
                .ToDictionary(group => group.Key, group => group.ToList());
        }
    }
}