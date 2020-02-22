using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fantasy_hoops.Controllers;
using fantasy_hoops.Database;
using fantasy_hoops.Dtos;
using fantasy_hoops.Helpers;
using fantasy_hoops.Models;
using fantasy_hoops.Models.ViewModels;
using fantasy_hoops.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace fantasy_hoops.Jobs
{
    public class BestLineupsJob
    {
        private readonly GameContext _context;
        private readonly IPushService _pushService;
        private readonly DateTime _date = CommonFunctions.UTCToEastern(NextGameJob.PREVIOUS_GAME.Date);

        public BestLineupsJob(IPushService pushService)
        {
            _context = new GameContext();
            _pushService = pushService;
        }

        public async Task Execute()
        {
            _context.Database.SetCommandTimeout(0);
            var previousGameStats = await GetPreviousGameStats();
            var lineupsCombinations = CrossProductFunctions.CrossProduct(previousGameStats)
                .Select(lineup => new
                {
                    Players = lineup.Select(l => new PlayersBestLineups
                    {
                        PlayerID = l.Player.PlayerID,
                        FP = l.FP,
                        Price = l.Price
                    }),
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
                var bestLineups = await _context.BestLineups
                    .Include(x => x.Lineup)
                    .Select(x => new
                    {
                        x.LineupPrice,
                        x.Date,
                        x.TotalFP,
                        playerIds = x.Lineup.Select(l => l.PlayerID).ToList()
                    })
                    .ToListAsync();
                var bestLineup = bestLineups
                    .FirstOrDefault(x => Math.Round(x.TotalFP, 1).Equals(Math.Round(lineup.FP, 1))
                                         && x.LineupPrice == lineup.TotalPrice
                                         && x.Date.Equals(_date)
                                         && x.playerIds.All(players.Contains));
                if (bestLineup == null)
                {
                    await _context.BestLineups.AddAsync(new BestLineup
                    {
                        Date = _date,
                        Lineup = lineup.Players,
                        TotalFP = Math.Round(lineup.FP, 1),
                        LineupPrice = lineup.TotalPrice
                    });
                }
            }

            await _context.SaveChangesAsync();

            PushNotificationViewModel notification =
                new PushNotificationViewModel("Admin notification", "BestLineupsJob completed successfully");
            await _pushService.SendAdminNotification(notification);
        }

        private async Task<IDictionary<string, List<LineupPlayerDto>>> GetPreviousGameStats()
        {
            var allStats = await _context.Stats
                .Include(stats => stats.Player)
                .ThenInclude(player => player.Team)
                .Where(stats => stats.Date.Date.Equals(_date))
                .Select(stats => new LineupPlayerDto
                {
                    Player = stats.Player,
                    FP = stats.FP,
                    Price = stats.Price
                })
                .ToListAsync();
            return allStats.GroupBy(stats => stats.Player.Position)
                .ToDictionary(group => group.Key, group => group.ToList());
        }
    }
}