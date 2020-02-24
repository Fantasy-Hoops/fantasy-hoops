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
using Microsoft.EntityFrameworkCore.Internal;

namespace fantasy_hoops.Jobs
{
    public class BestLineupsJob
    {
        private readonly List<(List<LineupPlayerDto>, double)> _playersList = new List<(List<LineupPlayerDto>, double)>();
        
        private readonly GameContext _context;
        private readonly IPushService _pushService;
        private readonly DateTime _date = CommonFunctions.UTCToEastern(NextGameJob.PREVIOUS_GAME).Date;

        public BestLineupsJob(IPushService pushService)
        {
            _context = new GameContext();
            _pushService = pushService;
        }

        public async Task Execute()
        {
            _context.Database.SetCommandTimeout(0);
            var previousGameStats = await GetPreviousGameStats();
            var lineupsCombinations = CrossProductFunctions.CrossProduct(previousGameStats);
            using var lineupsEnumerator = lineupsCombinations.GetEnumerator();
            do
            {
                List<LineupPlayerDto> currentLineup = lineupsEnumerator.Current;
                if (currentLineup == null)
                {
                    continue;
                }

                int lineupPrice = currentLineup.Sum(player => player.Price);
                
                if (lineupPrice > LineupController.MAX_PRICE)
                {
                    continue;
                }
                
                double lineupFP = currentLineup.Sum(player => player.FP);

                if (_playersList.Count < 10)
                {
                    _playersList.Add((currentLineup, lineupFP));
                    continue;
                }

                double minFP = _playersList.Min(x => x.Item2);
                if (lineupFP > minFP)
                {
                    int indexOfMin = _playersList.Select(x => x.Item2).IndexOf(minFP);
                    _playersList.RemoveAt(indexOfMin);
                    _playersList.Add((currentLineup, lineupFP));
                }
            } while (lineupsEnumerator.MoveNext());
            
            _playersList.OrderBy(lineup => lineup.Item2)
                .Select(x => x.Item1)
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
                .ToList().ForEach(async lineup =>
                {
                    bool bestLineupExists = _context.BestLineups
                        .Any(x => Math.Round(x.TotalFP, 1).Equals(Math.Round(lineup.FP, 1))
                                             && x.LineupPrice == lineup.TotalPrice
                                             && x.Date.Equals(_date));
                    if (!bestLineupExists)
                    {
                        await _context.BestLineups.AddAsync(new BestLineup
                        {
                            Date = _date,
                            Lineup = lineup.Players.ToList(),
                            TotalFP = Math.Round(lineup.FP, 1),
                            LineupPrice = lineup.TotalPrice
                        });
                    }
                });

            await _context.SaveChangesAsync();

            PushNotificationViewModel notification =
                new PushNotificationViewModel("Admin notification", "BestLineupsJob completed successfully");
            await _pushService.SendAdminNotification(notification);
        }

        private async Task<IDictionary<string, List<LineupPlayerDto>>> GetPreviousGameStats()
        {
            var allStats = await _context.Stats
                .Where(stats => stats.Date.Date.Equals(_date) && stats.FP > 0)
                .Include(stats => stats.Player)
                .ThenInclude(player => player.Team)
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