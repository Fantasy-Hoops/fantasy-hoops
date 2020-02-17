using System;
using System.Collections.Generic;
using System.Linq;
using fantasy_hoops.Database;
using fantasy_hoops.Dtos;
using fantasy_hoops.Helpers;
using fantasy_hoops.Jobs;
using Microsoft.EntityFrameworkCore;

namespace fantasy_hoops.Repositories.Interfaces
{
    public class BestLineupsRepository : IBestLineupsRepository
    {
        private GameContext _context;
        
        public BestLineupsRepository()
        {
            _context = new GameContext();
        }

        public List<BestLineupDto> GetBestLineups(string date)
        {
            DateTime dateTime = CommonFunctions.UTCToEastern(NextGameJob.PREVIOUS_GAME.Date);
            if (date != null)
            {
                dateTime = DateTime.Parse(date);
            }
            
            return _context.BestLineups
                .Where(l => l.Date == dateTime)
                .Include(lineup => lineup.Lineup)
                .ThenInclude(lineup => lineup.Player)
                .Select(lineup => new BestLineupDto
                {
                    Date = lineup.Date,
                    Lineup = lineup.Lineup
                        .Select(l => new LineupPlayerDto
                        {
                            Player = l.Player,
                            FP = l.FP,
                            Price = l.Price
                        }).ToList(),
                    FP = lineup.TotalFP,
                    Price = lineup.LineupPrice
                })
                .ToList();
        }
    }
}