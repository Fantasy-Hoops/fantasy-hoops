using Castle.Core;
using fantasy_hoops.Models.ViewModels;
using fantasy_hoops.Repositories.Interfaces;

namespace fantasy_hoops.Services.Interfaces
{
    public interface ITournamentsService
    {
        public Pair<bool, string> CreateTournament(CreateTournamentViewModel tournamentModel);
        public bool AcceptInvitation(string tournamentId, string userId);
        public bool DeclineInvitation(string tournamentId, string userId);
    }
}