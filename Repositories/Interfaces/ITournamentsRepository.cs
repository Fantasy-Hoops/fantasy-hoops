using System;
using System.Collections.Generic;
using fantasy_hoops.Dtos;
using fantasy_hoops.Models;
using fantasy_hoops.Models.Tournaments;

namespace fantasy_hoops.Repositories.Interfaces
{
    public interface ITournamentsRepository
    {
        List<TournamentTypeDto> GetTournamentTypes();
        Tournament.TournamentType GetTournamentTypeById(int id);
        List<DateTime> GetUpcomingStartDates();
        DateTime GetLastEndDate();
        Tournament GetTournamentById(string tournamentId);
        Dictionary<string, List<TournamentDto>> GetUserTournaments(string userId);
        bool CreateTournament(Tournament tournament);
        bool TournamentExists(string id);
        bool TournamentExists(Tournament tournament);
        bool TournamentNameExists(string name);
        bool IsUserInTournament(string userId, string tournamentId);
        bool IsUserInTournament(User user, string tournamentId);
        bool IsUserInTournament(string userId, Tournament tournament);
        bool IsUserInTournament(User user, Tournament tournament);
        List<Tournament> GetTournamentsForStartDate(DateTime startDate);
        List<String> GetTournamentUsersIds(string tournamentId);
        void AddCreatorToTournament(Tournament tournament);
        void AddUserToTournament(string userId, string tournamentId);
    }
}