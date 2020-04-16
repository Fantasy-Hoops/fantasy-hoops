using System;
using System.Linq;
using fantasy_hoops.Database;
using fantasy_hoops.Models;
using fantasy_hoops.Repositories.Interfaces;

namespace fantasy_hoops.Repositories
{
    public class PlayerRepository : IPlayerRepository
    {
        private readonly GameContext _context;

        public PlayerRepository()
        {
            _context = new GameContext();
        }

        public IQueryable<Object> GetActivePlayers()
        {
            return _context.Players
                .Where(player => bool.Parse(Startup.Configuration["UseMock"])
                    ? Mocks.Players.PlayerPool.Contains(player.PlayerID)
                    : player.IsPlaying && !player.IsInGLeague)
                .Select(player => new
                {
                    playerId = player.PlayerID,
                    id = player.NbaID,
                    player.FullName,
                    player.FirstName,
                    player.LastName,
                    player.AbbrName,
                    team = new
                    {
                        player.Team.TeamID,
                        player.Team.Abbreviation,
                        TeamColor = player.Team.Color,
                        opp = player.Team.NextOppFormatted
                    },
                    player.Price,
                    player.Position,
                    player.FPPG,
                    injuryStatus = player.Injury != null ? player.Injury.Status : "Active"
                })
                .OrderByDescending(p => p.Price)
                .AsQueryable();
        }

        private bool IsPlayerActive(Player player)
        {
            if (player.Team == null)
            {
                return false;
            }
            
            if (bool.Parse(Startup.Configuration["UseMock"]))
            {
                return Mocks.Players.PlayerPool.Contains(player.PlayerID);
            }

            return player.IsPlaying && !player.IsInGLeague;
        }

        public IQueryable<Object> GetPlayer(int id)
        {
            return _context.Players.Where(player => player.NbaID == id)
                .Select(player => new
                {
                    player.PlayerID,
                    player.NbaID,
                    player.FullName,
                    player.FirstName,
                    player.LastName,
                    player.AbbrName,
                    player.Number,
                    player.Position,
                    player.PTS,
                    player.REB,
                    player.AST,
                    player.STL,
                    player.BLK,
                    player.TOV,
                    player.FPPG,
                    player.Price,
                    injuryStatus = player.Injury != null ? player.Injury.Status : "Active",
                    Team = new
                    {
                        player.TeamID,
                        player.Team.NbaID,
                        player.Team.Abbreviation,
                        player.Team.City,
                        player.Team.Name,
                        player.Team.Color
                    },
                });
        }
    }
}