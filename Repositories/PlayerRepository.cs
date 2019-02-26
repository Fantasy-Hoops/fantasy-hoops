using System;
using System.Linq;
using fantasy_hoops.Database;

namespace fantasy_hoops.Repositories
{
    public class PlayerRepository : IPlayerRepository
    {

        private readonly GameContext _context;

        public PlayerRepository(GameContext context)
        {
            _context = context;
        }

        public IQueryable<Object> GetActivePlayers()
        {
            return _context.Players.Where(x => x.IsPlaying)
                .Select(x => new
                {
                    playerId = x.PlayerID,
                    id = x.NbaID,
                    x.FullName,
                    x.FirstName,
                    x.LastName,
                    x.AbbrName,
                    team = new
                    {
                        x.Team.TeamID,
                        x.Team.Abbreviation,
                        TeamColor = x.Team.Color
                    },
                    x.Price,
                    x.Position,
                    x.FPPG,
                    injuryStatus = x.Status
                })
                .OrderByDescending(p => p.Price);
        }

        public IQueryable<Object> GetPlayer(int id)
        {
            return _context.Players.Where(x => x.NbaID == id)
                .Select(x => new
                {
                    x.PlayerID,
                    x.NbaID,
                    x.FullName,
                    x.FirstName,
                    x.LastName,
                    x.AbbrName,
                    x.Number,
                    x.Position,
                    x.PTS,
                    x.REB,
                    x.AST,
                    x.STL,
                    x.BLK,
                    x.TOV,
                    x.FPPG,
                    x.Price,
                    injuryStatus = x.Status,
                    Team = new
                    {
                        x.TeamID,
                        x.Team.NbaID,
                        x.Team.Abbreviation,
                        x.Team.City,
                        x.Team.Name,
                        x.Team.Color
                    },
                });
        }
    }
}
