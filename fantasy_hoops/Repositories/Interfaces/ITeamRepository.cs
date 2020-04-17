using System;
using System.Linq;
using fantasy_hoops.Models;

namespace fantasy_hoops.Repositories.Interfaces
{
    public interface ITeamRepository
    {

        IQueryable<Object> GetTeams();
        Team GetTeam(int nbaID);
        Team GetTeamById(int id);
        Team GetUnknownTeam();
    }
}
