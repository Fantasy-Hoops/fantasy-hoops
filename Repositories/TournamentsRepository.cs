using System;
using System.Collections.Generic;
using System.Linq;
using fantasy_hoops.Database;
using fantasy_hoops.Helpers;
using fantasy_hoops.Models.Tournaments;
using fantasy_hoops.Repositories.Interfaces;

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

        public Tournament.TournamentType GetTournamentTypeById(int id)
        {
            return _context.TournamentTypes.Find(id);
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

        public bool CreateTournament(Tournament tournament)
        {
            _context.Tournaments.Add(tournament);
            ;
            return _context.SaveChanges() > 0;
        }

        public bool TournamentExists(Tournament tournament)
        {
            return TournamentExists(tournament.Id);
        }

        public bool TournamentExists(string id)
        {
            return _context.Tournaments.Any(tournament => tournament.Id.Equals(id));
        }
    }
}