using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using fantasy_hoops.Database;
using fantasy_hoops.Dtos;
using fantasy_hoops.Helpers;
using fantasy_hoops.Models;
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

        public List<TournamentTypeDto> GetTournamentTypes()
        {
            return Tournament.TournamentType.Values()
                .Select(type => new TournamentTypeDto
                {
                    Id = type.GetId(),
                    Name = type.ToString()
                })
                .ToList();
        }

        public Tournament.TournamentType GetTournamentTypeById(int id)
        {
            return Tournament.TournamentType.Values().FirstOrDefault(type => type.GetId() == id);
        }

        public List<DateTime> GetUpcomingStartDates()
        {
            return _context.Games
                .AsEnumerable()
                .Where(game => game.Date.HasValue && game.Date.Value.DayOfWeek != DayOfWeek.Sunday)
                .ToList()
                .GroupBy(game => CommonFunctions.GetIso8601WeekOfYear(game.Date.Value))
                .Select(group => group.Min(game => game.Date.Value))
                .Where(date => date > CommonFunctions.EctNow)
                .OrderBy(date => date)
                .ToList();
        }

        public DateTime GetLastEndDate()
        {
            return _context.Games
                .Where(game => game.Date.HasValue)
                .Max(tournament => tournament.Date.Value);
        }

        public Tournament GetTournamentById(string tournamentId)
        {
            return _context.Tournaments
                .FirstOrDefault(tournament => tournament.Id.Equals(tournamentId));
        }

        public Dictionary<string, List<TournamentDto>> GetUserTournaments(string userId)
        {
            List<TournamentDto> createdTournaments =
                _context.Tournaments
                    .Where(tournament => tournament.CreatorID.Equals(userId))
                    .Include(tournament => tournament.Type)
                    .Select(tournament => new TournamentDto
                    {
                        Id = tournament.Id,
                        Type = ((Tournament.TournamentType) tournament.Type).ToString(),
                        StartDate = tournament.StartDate,
                        EndDate = tournament.EndDate,
                        Name = tournament.Name,
                        Description = tournament.Description,
                        ImageURL = tournament.ImageURL,
                        Entrants = tournament.Entrants,
                        Contests = tournament.Contests,
                        DroppedContests = tournament.DroppedContests
                    })
                    .ToList();
            List<TournamentDto> joinedTournaments =
                _context.TournamentUsers
                    .Include(tournamentUser => tournamentUser.Tournament)
                    .Where(tournamentUser => tournamentUser.UserID.Equals(userId)
                                             && !tournamentUser.Tournament.CreatorID.Equals(userId))
                    .Select(tournamentUser => tournamentUser.Tournament)
                    .Include(tournament => tournament.Type)
                    .Select(tournament => new TournamentDto
                    {
                        Id = tournament.Id,
                        Type = ((Tournament.TournamentType) tournament.Type).ToString(),
                        StartDate = tournament.StartDate,
                        EndDate = tournament.EndDate,
                        Name = tournament.Name,
                        Description = tournament.Description,
                        ImageURL = tournament.ImageURL,
                        Entrants = tournament.Entrants,
                        Contests = tournament.Contests,
                        DroppedContests = tournament.DroppedContests
                    })
                    .ToList();

            return new Dictionary<string, List<TournamentDto>>
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

        public bool IsUserInTournament(User user, string tournamentId)
        {
            return IsUserInTournament(user.Id, tournamentId);
        }

        public bool IsUserInTournament(string userId, Tournament tournament)
        {
            return IsUserInTournament(userId, tournament.Id);
        }

        public bool IsUserInTournament(User user, Tournament tournament)
        {
            return IsUserInTournament(user.Id, tournament.Id);
        }

        public List<Tournament> GetTournamentsForStartDate(DateTime startDate)
        {
            return _context.Tournaments
                .Where(tournament => tournament.StartDate.Date == startDate.Date)
                .ToList();
        }

        public List<String> GetTournamentUsersIds(string tournamentId)
        {
            return _context.TournamentUsers
                .Include(tournamentUser => tournamentUser.User)
                .Where(tournamentUser => tournamentUser.TournamentID.Equals(tournamentId))
                .Select(tournamentUser => tournamentUser.UserID)
                .ToList();
        }

        public void AddCreatorToTournament(Tournament tournament)
        {
            if (!_context.TournamentUsers.Any(tournamentUser => tournamentUser.UserID.Equals(tournament.CreatorID)
                                                                && tournamentUser.TournamentID.Equals(tournament.Id)))
            {
                _context.TournamentUsers.Add(new TournamentUser
                {
                    UserID = tournament.CreatorID,
                    TournamentID = tournament.Id
                });
                _context.SaveChanges();
            }
        }

        public void AddUserToTournament(string userId, string tournamentId)
        {
            _context.TournamentUsers.Add(new TournamentUser
            {
                UserID = userId,
                TournamentID = tournamentId
            });
            _context.SaveChanges();
        }

        public bool IsUserInTournament(string userId, string tournamentId)
        {
            bool isInvited = _context.TournamentUsers
                .Any(tournamentUser => tournamentUser.TournamentID.Equals(tournamentId)
                                       && tournamentUser.UserID.Equals(userId));
            bool isCreator = _context.Tournaments.Any(tournament => tournament.Id.Equals(tournamentId)
                                                                    && tournament.CreatorID.Equals(userId));

            return isInvited || isCreator;
        }
    }
}