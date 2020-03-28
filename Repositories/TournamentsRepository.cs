using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using fantasy_hoops.Database;
using fantasy_hoops.Helpers;
using fantasy_hoops.Models.Tournaments;
using fantasy_hoops.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore.Internal;

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

        public Dictionary<string, List<Tournament>> GetUserTournaments(string userId)
        {
            List<Tournament> createdTournaments =
                _context.Tournaments.Where(tournament => tournament.CreatorID.Equals(userId)).ToList();
            List<Tournament> joinedTournaments =
                _context.TournamentUsers
                    .Include(tournamentUser => tournamentUser.Tournament)
                    .Where(tournamentUser => tournamentUser.UserID.Equals(userId))
                    .Select(tournamentUser => tournamentUser.Tournament)
                    .ToList();

            return new Dictionary<string, List<Tournament>>
            {
                {"created", createdTournaments},
                {"joined", joinedTournaments}
            };
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

        public bool TournamentNameExists(string name)
        {
            return _context.Tournaments.Any(tournament => tournament.Name.ToUpper().Equals(name.ToUpper()));
        }
    }
}