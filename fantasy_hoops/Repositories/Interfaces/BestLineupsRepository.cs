using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using fantasy_hoops.Database;
using fantasy_hoops.Dtos;
using fantasy_hoops.Helpers;
using Microsoft.EntityFrameworkCore;

namespace fantasy_hoops.Repositories.Interfaces
{
    public class BestLineupsRepository : IBestLineupsRepository
    {
        private readonly GameContext _context;
        
        public BestLineupsRepository()
        {
            _context = new GameContext();
        }

        public List<BestLineupDto> GetBestLineups(string date, int from, int limit)
        {
            DateTime dateTime;
            if (date != null && date.Length == 8)
            {
                dateTime = DateTime.ParseExact(date, "yyyyMMdd", CultureInfo.InvariantCulture);
            }
            else
            {
                dateTime = _context.BestLineups.Max(lineup => lineup.Date);
            }
            
            return _context.BestLineups
                .Where(lineup => lineup.Date.Equals(dateTime))
                .Include(lineup => lineup.Lineup)
                .ThenInclude(lineup => lineup.Player)
                .ThenInclude(player => player.Team)
                .Select(lineup => new BestLineupDto
                {
                    Date = lineup.Date,
                    Lineup = lineup.Lineup
                        .Select(l => new LineupPlayerDto
                        {
                            Player = l.Player,
                            TeamColor = l.Player.Team.Color,
                            FP = l.FP,
                            Price = l.Price
                        })
                        .OrderBy(p => CommonFunctions.LineupPositionsOrder.IndexOf(p.Player.Position))
                        .ToList(),
                    FP = lineup.TotalFP,
                    Price = lineup.LineupPrice
                })
                .OrderByDescending(lineup => lineup.FP)
                .Skip(from)
                .Take(limit)
                .ToList();
        }
    }
}