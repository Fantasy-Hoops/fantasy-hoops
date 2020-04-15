using Castle.Core;
using fantasy_hoops.Dtos;
using fantasy_hoops.Models;
using fantasy_hoops.Models.Tournaments;
using fantasy_hoops.Models.ViewModels;
using fantasy_hoops.Repositories.Interfaces;

namespace fantasy_hoops.Services.Interfaces
{
    public interface ITournamentsService
    {
        Pair<bool, string> CreateTournament(CreateTournamentViewModel tournamentModel);
        bool AcceptInvitation(string tournamentId, string userId);
        bool DeclineInvitation(string tournamentId, string userId);
        User GetTournamentWinner(string tournamentId);
        User GetContestWinner(TournamentDetailsDto tournamentDetails, ContestDto contest);
        User EliminateUser(Tournament dbTournament, TournamentDetailsDto tournamentDetails, ContestDto contest);
        void UpdateStandings(TournamentDetailsDto tournamentDetails, ContestDto contest);
        void SendCancelledTournamentNotifications(Tournament tournament);
    }
}