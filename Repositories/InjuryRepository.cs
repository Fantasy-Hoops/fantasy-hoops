﻿using System;
using System.Linq;
using fantasy_hoops.Database;
using fantasy_hoops.Repositories.Interfaces;

namespace fantasy_hoops.Repositories
{
    public class InjuryRepository : IInjuryRepository
    {

        private readonly GameContext _context;
        public InjuryRepository()
        {
            _context = new GameContext();
        }

        public IQueryable<Object> GetInjuries()
        {
            return _context.Injuries
                .Where(injury => injury.Date > DateTime.UtcNow.AddDays(-3))
                .Select(x => new {
                    x.InjuryID,
                    date = x.Date,
                    Player = new
                    {
                        x.Player.NbaID,
                        x.Player.FullName,
                        x.Player.FirstName,
                        x.Player.LastName,
                        x.Player.AbbrName,
                        x.Player.Position,
                        Team = new
                        {
                            x.Player.Team.NbaID,
                            x.Player.Team.City,
                            x.Player.Team.Name,
                            x.Player.Team.Abbreviation,
                            x.Player.Team.Color
                        }
                    },
                    x.Status,
                    Injury = x.InjuryTitle,
                    x.Title,
                    x.Description,
                    x.Link
                })
                .OrderByDescending(inj => inj.date);
        }

    }
}
