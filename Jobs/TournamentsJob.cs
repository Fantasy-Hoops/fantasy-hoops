using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using Castle.Core;
using fantasy_hoops.Database;
using fantasy_hoops.Models.Tournaments;
using fantasy_hoops.Repositories;
using fantasy_hoops.Repositories.Interfaces;
using FluentScheduler;

namespace fantasy_hoops.Jobs
{
    public class TournamentsJob : IJob
    {
        private readonly DateTime _runtimeDate;
        private readonly ITournamentsRepository _tournamentsRepository;
        private readonly GameContext _context;

        public TournamentsJob(DateTime runtimeDate)
        {
            _runtimeDate = runtimeDate;
            _tournamentsRepository = new TournamentsRepository();
            _context = new GameContext();
        }

        public void Execute()
        {
            // if (_tournamentsRepository.GetUpcomingStartDates().Contains(_runtimeDate))
            // {
                StartNewTournaments();
            // }
            
            if (_runtimeDate.DayOfWeek == DayOfWeek.Sunday)
            {
                CalculateContests();
            }
        }

        private void StartNewTournaments()
        {
            List<Tournament> tournamentsToStart = _tournamentsRepository.GetTournamentsForStartDate(_runtimeDate);
            foreach (Tournament tournament in tournamentsToStart)
            {
                if ((Tournament.TournamentType) tournament.Type == Tournament.TournamentType.MATCHUPS)
                {
                    StartMatchupsTournament(tournament);
                } else if ((Tournament.TournamentType) tournament.Type == Tournament.TournamentType.ONE_FOR_ALL)
                {
                    _context.Tournaments.Find(tournament.Id).IsActive = true;
                    _context.SaveChanges();
                }
            }
        }

        private void CalculateContests()
        {
        }

        private void StartMatchupsTournament(Tournament tournament)
        {
            List<string> tournamentUsers = _tournamentsRepository.GetTournamentUsersIds(tournament.Id);
            List<Tuple<int, string, string>> res = new List<Tuple<int, string, string>>();
            for (int i = 0; i < tournamentUsers.Count; i++)
            {
                for (int j = i + 1; j < tournamentUsers.Count; j++)
                {
                    res.Add(new Tuple<int, string, string>(j, tournamentUsers[i], tournamentUsers[j]));
                    res.Add(new Tuple<int, string, string>(tournamentUsers.Count + i, tournamentUsers[j], tournamentUsers[i]));
                }
            }

            foreach (var contest in _context.Contests
                .Where(x => x.TournamentId.Equals(tournament.Id))
                .ToList().Select((value, index) => (value, index)))
            {
                contest.value.ContestPairs = new List<MatchupPair>();
                res.Where(record => record.Item1 == contest.index + 1)
                    .ToList()
                    .ForEach(contestPair =>
                    {
                        MatchupPair dbMatchupPair = _context.TournamentMatchups
                            .Find(tournament.Id, res[contest.index].Item2, res[contest.index].Item3, contest.value.Id);
                        if (dbMatchupPair == null)
                        {
                            contest.value.ContestPairs.Add(new MatchupPair
                            {
                                ContestId = contest.value.Id,
                                TournamentID = tournament.Id,
                                FirstUserID = res[contest.index].Item2,
                                SecondUserID = res[contest.index].Item3
                            });
                        }
                    });
            }
            _context.Tournaments.Find(tournament.Id).IsActive = true;
            _context.SaveChanges();
        }
    }
}