using fantasy_hoops.Database;
using fantasy_hoops.Dtos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using fantasy_hoops.Repositories.Interfaces;

namespace fantasy_hoops.Repositories
{
    public class StatsRepository : IStatsRepository
    {

        private readonly GameContext _context;

        public StatsRepository()
        {
            _context = new GameContext();
        }

        public List<StatsDto> GetStats()
        {
            GameContext context = new GameContext();
            return context.Players
                .Include(player => player.Team)
                .Select(player => new StatsDto
                {
                    PlayerID = player.PlayerID,
                    NbaID = player.NbaID,
                    FullName =  player.FullName,
                    FirstName = player.FirstName,
                    LastName = player.LastName,
                    AbbrName = player.AbbrName,
                    Number = player.Number,
                    Position = player.Position,
                    PTS = player.PTS,
                    REB = player.REB,
                    AST = player.AST,
                    STL = player.STL,
                    BLK = player.BLK,
                    TOV = player.TOV,
                    FPPG = player.FPPG,
                    Price = player.Price,
                    Team = new TeamDto
                    {
                        TeamID = player.TeamID,
                        NbaID = player.Team.NbaID,
                        Abbreviation = player.Team.Abbreviation,
                        City = player.Team.City,
                        Name = player.Team.Name,
                        Color = player.Team.Color
                    },
                    Games = player.Stats.Where(stats => stats.PlayerID == player.PlayerID)
                        .Select(stats => new GameStatsDto
                        {
                            StatsID = stats.StatsID,
                            Date = stats.Date,
                            Opponent = context.Teams.Select(opponent => new OpponentDto
                                {
                                    NbaID = opponent.NbaID,
                                    Abbreviation = opponent.Abbreviation
                                }).FirstOrDefault(t => t.NbaID == stats.OppID),
                            Score = stats.Score,
                            MIN = stats.MIN,
                            FGM = stats.FGM,
                            FGA = stats.FGA,
                            FGP = String.Format("{0:#.000}", (stats.FGP / 100)),
                            TPM = stats.TPM,
                            TPA = stats.TPA,
                            TPP = String.Format("{0:#.000}", (stats.TPP / 100)),
                            FTM = stats.FTM,
                            FTA = stats.FTA,
                            FTP = String.Format("{0:#.000}", (stats.FTP / 100)),
                            DREB = stats.DREB,
                            OREB=  stats.OREB,
                            TREB = stats.TREB,
                            AST = stats.AST,
                            BLK = stats.BLK,
                            STL = stats.STL,
                            FLS = stats.FLS,
                            TOV = stats.TOV,
                            PTS = stats.PTS,
                            GS = Math.Round(stats.GS, 1),
                            FP = Math.Round(stats.FP, 1),
                            Price = stats.Price
                        })
                        .OrderByDescending(stats => stats.Date)
                        .ToList()
                })
                .ToList();
        }

        public List<StatsDto> GetStats(int id, int start, int count)
        {
            GameContext context = new GameContext();
            double maxPoints = context.Players.Max(player => player.PTS),
                   maxAssists = context.Players.Max(player => player.AST),
                   maxTurnovers = context.Players.Max(player => player.TOV),
                   maxRebounds = context.Players.Max(player => player.REB),
                   maxBlocks = context.Players.Max(player => player.BLK),
                   maxSteals = context.Players.Max(player => player.STL);

            return context.Players
                .Where(player => player.NbaID == id)
                .Include(player => player.Team)
                .Select(player => new StatsDto
                {
                    PlayerID = player.PlayerID,
                    NbaID = player.NbaID,
                    FullName = player.FullName,
                    FirstName = player.FirstName,
                    LastName = player.LastName,
                    AbbrName = player.AbbrName,
                    Number = player.Number,
                    Position = player.Position,
                    PTS = player.PTS,
                    REB = player.REB,
                    AST = player.AST,
                    STL = player.STL,
                    BLK = player.BLK,
                    TOV = player.TOV,
                    FPPG = player.FPPG,
                    Price = player.Price,
                    Percentages = new StatsPercentagesDto
                    {
                        PTS = Math.Round(player.PTS / maxPoints * 100, 0),
                        AST = Math.Round(player.AST / maxAssists * 100, 0),
                        TOV = Math.Round(player.TOV / maxTurnovers * 100, 0),
                        REB = Math.Round(player.REB / maxRebounds * 100, 0),
                        BLK = Math.Round(player.BLK / maxBlocks * 100, 0),
                        STL = Math.Round(player.STL / maxSteals * 100, 0)
                    },
                    Team = new TeamDto
                    {
                        TeamID = player.TeamID,
                        NbaID = player.Team.NbaID,
                        Abbreviation = player.Team.Abbreviation,
                        City = player.Team.City,
                        Name = player.Team.Name,
                        Color = player.Team.Color
                    },
                    Games = player.Stats
                    .OrderByDescending(stats => stats.Date)
                    .Skip(start)
                    .Take(count)
                    .Select(stats => new GameStatsDto
                    {
                        StatsID = stats.StatsID,
                        Date = stats.Date,
                        Opponent = context.Teams.Select(opponent => new OpponentDto
                        {
                            NbaID = opponent.NbaID,
                            Abbreviation = opponent.Abbreviation
                        }).FirstOrDefault(t => t.NbaID == stats.OppID),
                        Score = stats.Score,
                        MIN = stats.MIN,
                        FGM = stats.FGM,
                        FGA = stats.FGA,
                        FGP = String.Format("{0:#.000}", (stats.FGP / 100)),
                        TPM = stats.TPM,
                        TPA = stats.TPA,
                        TPP = String.Format("{0:#.000}", (stats.TPP / 100)),
                        FTM = stats.FTM,
                        FTA = stats.FTA,
                        FTP = String.Format("{0:#.000}", (stats.FTP / 100)),
                        DREB = stats.DREB,
                        OREB = stats.OREB,
                        TREB = stats.TREB,
                        AST = stats.AST,
                        BLK = stats.BLK,
                        STL = stats.STL,
                        FLS = stats.FLS,
                        TOV = stats.TOV,
                        PTS = stats.PTS,
                        GS = Math.Round(stats.GS, 1),
                        FP = Math.Round(stats.FP, 1),
                        Price = stats.Price
                    })
                    .ToList()
                })
                .ToList();
        }
    }
}
