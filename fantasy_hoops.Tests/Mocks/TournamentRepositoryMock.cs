using System;
using System.Collections.Generic;
using fantasy_hoops.Dtos;
using fantasy_hoops.Models;
using fantasy_hoops.Models.Enums;
using fantasy_hoops.Models.Tournaments;
using fantasy_hoops.Repositories.Interfaces;

namespace fantasy_hoops.Tests.Mocks
{
    public class TournamentRepositoryMock : ITournamentsRepository
    {
        public TournamentRepositoryMock()
        {
        }

        public List<TournamentTypeDto> GetTournamentTypes()
        {
            throw new NotImplementedException();
        }

        public Tournament.TournamentType GetTournamentTypeById(int id)
        {
            throw new NotImplementedException();
        }

        public List<DateTime> GetUpcomingStartDates()
        {
            throw new NotImplementedException();
        }

        public DateTime GetLastEndDate()
        {
            throw new NotImplementedException();
        }

        public Tournament GetTournamentById(string tournamentId)
        {
            throw new NotImplementedException();
        }

        public TournamentDetailsDto GetTournamentDetails(string tournamentId)
        {
            throw new NotImplementedException();
        }

        public TournamentDetailsDto GetTournamentDetails(string userId, string tournamentId)
        {
            throw new NotImplementedException();
        }

        public List<TournamentDetailsDto> GetAllTournamentsDetails()
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, List<TournamentDto>> GetUserTournaments(string userId)
        {
            throw new NotImplementedException();
        }

        public bool CreateTournament(Tournament tournament)
        {
            throw new NotImplementedException();
        }

        public bool TournamentExists(string id)
        {
            throw new NotImplementedException();
        }

        public bool TournamentExists(Tournament tournament)
        {
            throw new NotImplementedException();
        }

        public bool TournamentNameExists(string name)
        {
            throw new NotImplementedException();
        }

        public bool IsUserInTournament(string userId, string tournamentId)
        {
            throw new NotImplementedException();
        }

        public bool IsUserInvited(string userId, string tournamentId)
        {
            throw new NotImplementedException();
        }

        public bool IsUserInTournament(User user, string tournamentId)
        {
            throw new NotImplementedException();
        }

        public bool IsUserInTournament(string userId, Tournament tournament)
        {
            throw new NotImplementedException();
        }

        public bool IsUserInTournament(User user, Tournament tournament)
        {
            throw new NotImplementedException();
        }

        public List<Tournament> GetTournamentsForStartDate(DateTime startDate)
        {
            throw new NotImplementedException();
        }

        public List<string> GetTournamentUsersIds(string tournamentId)
        {
            throw new NotImplementedException();
        }

        public TournamentUser GetTournamentUser(string tournamentId, string userId)
        {
            throw new NotImplementedException();
        }

        public void AddCreatorToTournament(Tournament tournament)
        {
            throw new NotImplementedException();
        }

        public bool AddUserToTournament(string userId, string tournamentId)
        {
            throw new NotImplementedException();
        }

        public List<Contest> GetTournamentContests(string tournamentId)
        {
            throw new NotImplementedException();
        }

        public List<ContestDto> GetAllCurrentContests()
        {
            throw new NotImplementedException();
        }

        public bool DeleteTournament(string tournamentId)
        {
            throw new NotImplementedException();
        }

        public List<TournamentDto> GetTournamentInvitations(string userId)
        {
            throw new NotImplementedException();
        }

        public bool ChangeInvitationStatus(string tournamentId, string userId, RequestStatus status = RequestStatus.NO_REQUEST)
        {
            throw new NotImplementedException();
        }

        public User SetTournamentUserEliminated(TournamentUserDto tournamentUser)
        {
            throw new NotImplementedException();
        }

        public Contest GetContestById(int contestId)
        {
            throw new NotImplementedException();
        }

        public void UpdateTournamentUserStats(TournamentUser tournamentUser, int wins, int losses, int points)
        {
            throw new NotImplementedException();
        }
    }
}