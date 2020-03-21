using System.Collections.Generic;
using Castle.Core;
using fantasy_hoops.Models.Tournaments;

namespace fantasy_hoops.Repositories.Interfaces
{
    public interface ITournamentsRepository
    {
        List<Tournament.TournamentType> GetTournamentTypes();
    }
}