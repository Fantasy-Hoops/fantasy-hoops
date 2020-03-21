using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Core;
using fantasy_hoops.Database;
using fantasy_hoops.Helpers;
using fantasy_hoops.Models.Tournaments;
using fantasy_hoops.Repositories.Interfaces;
using Microsoft.OpenApi.Extensions;

namespace fantasy_hoops.Repositories
{
    public class TournamentsRepository : ITournamentsRepository
    {
        private readonly GameContext _context;

        public TournamentsRepository()
        {
            _context = new GameContext();
        }
        
        public List<Tournament.TournamentType> GetTournamentTypes()
        {
            return _context.TournamentTypes
                .Distinct()
                .ToList();
        }

        public List<DateTime> GetUpcomingStartDates()
        {
            return _context.Games
                .Where(game => game.Date.HasValue && game.Date > DateTime.UtcNow)
                .ToList()
                .GroupBy(game => CommonFunctions.GetIso8601WeekOfYear(game.Date.Value))
                .Select(group => group.Min(game => game.Date.Value))
                .OrderBy(date => date)
                .ToList();
        }
    }
}