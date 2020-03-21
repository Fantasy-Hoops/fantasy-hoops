using System.Collections.Generic;
using System.Linq;
using Castle.Core;
using fantasy_hoops.Database;
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
    }
}