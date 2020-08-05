using System;
using System.Linq;
using fantasy_hoops.Database;
using fantasy_hoops.Models;
using fantasy_hoops.Repositories.Interfaces;

namespace fantasy_hoops.Repositories
{
    public class TeamRepository : ITeamRepository
    {

        private readonly GameContext _context;

        public TeamRepository()
        {
            _context = new GameContext();
        }

        public IQueryable<Object> GetTeams()
        {
            return new GameContext().Teams
                .Select(x => new
                {
                    x.TeamID,
                    x.NbaID,
                    x.City,
                    x.Name,
                    x.Color
                })
                .OrderBy(x => x.Name);
        }

        public Team GetTeam(int nbaID)
        {
            return new GameContext().Teams.FirstOrDefault(x => x.NbaID == nbaID);
        }

        public Team GetTeamById(int id)
        {
            return new GameContext().Teams.FirstOrDefault(x => x.TeamID == id);
        }

        public Team GetUnknownTeam()
        {
            return new GameContext().Teams.FirstOrDefault(t => t.NbaID == 0);
        }
    }
}
