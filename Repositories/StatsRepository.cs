using fantasy_hoops.Database;
using fantasy_hoops.Models;
using System;
using System.Linq;

namespace fantasy_hoops.Repositories
{
    public class StatsRepository : IStatsRepository
    {

        private readonly GameContext _context;

        public StatsRepository()
        {
            _context = new GameContext();
        }

        public IQueryable<object> GetStats()
        {
            return _context.Players
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
                    Team = new
                    {
                        x.TeamID,
                        x.Team.NbaID,
                        x.Team.Abbreviation,
                        x.Team.City,
                        x.Team.Name,
                        x.Team.Color
                    },
                    Games = x.Stats.Where(s => s.PlayerID == x.PlayerID)
                        .Select(s => new
                        {
                            s.StatsID,
                            s.Date,
                            Opponent = _context.Teams.Where(t => t.NbaID == s.OppID)
                                .Select(t => new
                                {
                                    t.NbaID,
                                    t.Abbreviation
                                })
                                .FirstOrDefault(),
                            s.Score,
                            s.MIN,
                            s.FGM,
                            s.FGA,
                            FGP = String.Format("{0:#.000}", (s.FGP / 100)),
                            s.TPM,
                            s.TPA,
                            TPP = String.Format("{0:#.000}", (s.TPP / 100)),
                            s.FTM,
                            s.FTA,
                            FTP = String.Format("{0:#.000}", (s.FTP / 100)),
                            s.DREB,
                            s.OREB,
                            s.TREB,
                            s.AST,
                            s.BLK,
                            s.STL,
                            s.FLS,
                            s.TOV,
                            s.PTS,
                            GS = Math.Round(s.GS, 1),
                            FP = Math.Round(s.FP, 1),
                            s.Price
                        })
                        .OrderByDescending(s => s.Date)
                });
        }

        public IQueryable<object> GetStats(int id, int start, int count)
        {
            double maxPoints = _context.Players.Max(player => player.PTS),
                   maxAssists = _context.Players.Max(player => player.AST),
                   maxTurnovers = _context.Players.Max(player => player.TOV),
                   maxRebounds = _context.Players.Max(player => player.REB),
                   maxBlocks = _context.Players.Max(player => player.BLK),
                   maxSteals = _context.Players.Max(player => player.STL);

            return _context.Players
                .Where(x => x.NbaID == id)
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
                    Percentages = new
                    {
                        PTS = Math.Round(x.PTS / maxPoints * 100, 0),
                        AST = Math.Round(x.AST / maxAssists * 100, 0),
                        TOV = Math.Round(x.TOV / maxTurnovers * 100, 0),
                        REB = Math.Round(x.REB / maxRebounds * 100, 0),
                        BLK = Math.Round(x.BLK / maxBlocks * 100, 0),
                        STL = Math.Round(x.STL / maxSteals * 100, 0)
                    },
                    Team = new
                    {
                        x.TeamID,
                        x.Team.NbaID,
                        x.Team.Abbreviation,
                        x.Team.City,
                        x.Team.Name,
                        x.Team.Color
                    },
                    Games = x.Stats
                    .OrderByDescending(s => s.Date)
                    .Skip(start)
                    .Take(count)
                    .Select(s => new
                    {
                        s.StatsID,
                        s.Date,
                        Opponent = _context.Teams.Where(t => t.NbaID == s.OppID)
                            .Select(t => new
                            {
                                t.NbaID,
                                t.Abbreviation
                            })
                            .FirstOrDefault(),
                        s.Score,
                        s.MIN,
                        s.FGM,
                        s.FGA,
                        FGP = String.Format("{0:#.000}", (s.FGP / 100)),
                        s.TPM,
                        s.TPA,
                        TPP = String.Format("{0:#.000}", (s.TPP / 100)),
                        s.FTM,
                        s.FTA,
                        FTP = String.Format("{0:#.000}", (s.FTP / 100)),
                        s.DREB,
                        s.OREB,
                        s.TREB,
                        s.AST,
                        s.BLK,
                        s.STL,
                        s.FLS,
                        s.TOV,
                        s.PTS,
                        GS = Math.Round(s.GS, 1),
                        FP = Math.Round(s.FP, 1),
                        s.Price
                    })
                });
        }
    }
}
