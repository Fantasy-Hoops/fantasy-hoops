using System;
using System.Collections.Generic;
using Castle.Core;
using fantasy_hoops.Dtos;
using fantasy_hoops.Models;
using fantasy_hoops.Models.Tournaments;
using fantasy_hoops.Models.ViewModels;

namespace fantasy_hoops.Services.Interfaces
{
    public interface ITournamentsService
    {
        Pair<bool, string> CreateTournament(CreateTournamentViewModel tournamentModel);
        bool AcceptInvitation(string tournamentId, string userId);
        bool DeclineInvitation(string tournamentId, string userId);
        User GetTournamentWinner(TournamentDetailsDto tournamentDetails);
        User GetContestWinner(TournamentDetailsDto tournamentDetails, ContestDto contest);
        User EliminateUser(Tournament dbTournament, TournamentDetailsDto tournamentDetails, ContestDto contest);
        void UpdateStandings(TournamentDetailsDto tournamentDetails, ContestDto contest);
        void SendCancelledTournamentNotifications(Tournament tournament);
        List<Tuple<string, string>>[] GetMatchupsPermutations(List<string> userIds);
        Tournament FromViewModel(CreateTournamentViewModel tournamentModel);
        Pair<List<Contest>, DateTime> GenerateContests(CreateTournamentViewModel model);
        ContestDto GetContestDto(Contest contest);
    }
}