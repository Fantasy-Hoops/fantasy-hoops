using System;
using System.Collections.Generic;
using fantasy_hoops.Dtos;
using fantasy_hoops.Models;
using fantasy_hoops.Models.Enums;
using fantasy_hoops.Models.Tournaments;
using fantasy_hoops.Models.ViewModels;

namespace fantasy_hoops.Repositories.Interfaces
{
    public interface ITournamentsRepository
    {
        List<TournamentTypeDto> GetTournamentTypes();
        Tournament.TournamentType GetTournamentTypeById(int id);
        List<DateTime> GetUpcomingStartDates();
        Tournament GetTournamentById(string tournamentId);
        TournamentDetailsDto GetTournamentDetails(string tournamentId);
        TournamentDetailsDto GetTournamentDetails(string userId, string tournamentId);
        List<TournamentDetailsDto> GetAllTournamentsDetails();
        Dictionary<string, List<TournamentDto>> GetUserTournaments(string userId);
        bool CreateTournament(Tournament tournament);
        bool TournamentExists(string id);
        bool TournamentNameExists(string name);
        bool IsUserInTournament(string userId, string tournamentId);
        bool IsUserInvited(string userId, string tournamentId);
        List<Tournament> GetTournamentsForStartDate(DateTime startDate);
        List<String> GetTournamentUsersIds(string tournamentId);
        TournamentUser GetTournamentUser(string tournamentId, string userId);
        void AddCreatorToTournament(Tournament tournament);
        bool AddUserToTournament(string userId, string tournamentId);
        List<ContestDto> GetAllCurrentContests();
        bool DeleteTournament(Tournament tournament);
        bool DeleteTournament(string tournamentId);
        List<TournamentDto> GetTournamentInvitations(string userId);
        bool ChangeInvitationStatus(string tournamentId, string userId, RequestStatus status = RequestStatus.NO_REQUEST);
        User SetTournamentUserEliminated(TournamentUserDto tournamentUser);
        void UpdateTournamentUserStats(TournamentUser tournamentUser, int wins, int losses, int points);
        void RemoveUserMatchup(string userId, int contestId);
        bool UpdateTournament(Tournament tournament, CreateTournamentViewModel model);
        List<MatchupPair> GetContestMatchups(int contestId);
    }
}