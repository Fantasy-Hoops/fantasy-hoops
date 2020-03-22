using System;
using System.Collections.Generic;
using fantasy_hoops.Models.Tournaments;

namespace fantasy_hoops.Repositories.Interfaces
{
    public interface ITournamentsRepository
    {
        List<Tournament.TournamentType> GetTournamentTypes();
        Tournament.TournamentType GetTournamentTypeById(int id);
        List<DateTime> GetUpcomingStartDates();
        bool CreateTournament(Tournament tournament);
        bool TournamentExists(string id);
        bool TournamentExists(Tournament tournament);
    }
}