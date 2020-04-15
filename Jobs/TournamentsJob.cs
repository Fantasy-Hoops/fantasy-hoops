using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using Castle.Core;
using fantasy_hoops.Database;
using fantasy_hoops.Dtos;
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

        public TournamentsJob(DateTime runtimeDate = new DateTime())
        {
            _runtimeDate = runtimeDate;
            _tournamentsRepository = new TournamentsRepository();
            _context = new GameContext();
        }

        public void Execute()
        {
            if (_tournamentsRepository.GetUpcomingStartDates().Contains(_runtimeDate))
            {
                StartNewTournaments();
            }

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
                }
                else if ((Tournament.TournamentType) tournament.Type == Tournament.TournamentType.ONE_FOR_ALL)
                {
                    StartOneForAllTournament(tournament);
                }

                _context.SaveChanges();
            }
        }

        private void CalculateContests()
        {
        }

        private void StartMatchupsTournament(Tournament tournament)
        {
            List<string> tournamentUsers = _tournamentsRepository.GetTournamentUsersIds(tournament.Id);
            List<Tuple<int, string, string>> matchupsVariations = new List<Tuple<int, string, string>>();
            for (int i = 0; i < tournamentUsers.Count; i++)
            {
                for (int j = i + 1; j < tournamentUsers.Count; j++)
                {
                    matchupsVariations.Add(new Tuple<int, string, string>(j, tournamentUsers[i], tournamentUsers[j]));
                    matchupsVariations.Add(new Tuple<int, string, string>(tournamentUsers.Count + i, tournamentUsers[j],
                        tournamentUsers[i]));
                }
            }

            foreach (var contest in _context.Contests
                .Where(x => x.TournamentId.Equals(tournament.Id))
                .ToList().Select((value, index) => (value, index)))
            {
                contest.value.Matchups = new List<MatchupPair>();
                matchupsVariations.Where(record => record.Item1 == contest.index + 1)
                    .ToList()
                    .ForEach(contestPair =>
                    {
                        MatchupPair dbMatchupPair = _context.TournamentMatchups
                            .Find(tournament.Id, matchupsVariations[contest.index].Item2,
                                matchupsVariations[contest.index].Item3, contest.value.Id);
                        if (dbMatchupPair == null)
                        {
                            contest.value.Matchups.Add(new MatchupPair
                            {
                                ContestId = contest.value.Id,
                                TournamentID = tournament.Id,
                                FirstUserID = matchupsVariations[contest.index].Item2,
                                SecondUserID = matchupsVariations[contest.index].Item3
                            });
                        }
                    });
            }

            _context.Tournaments.Find(tournament.Id).IsActive = true;
        }

        private void StartOneForAllTournament(Tournament tournament)
        {
            List<string> tournamentUsers = _tournamentsRepository.GetTournamentUsersIds(tournament.Id);
            foreach (var contest in _context.Contests.Where(x => x.TournamentId.Equals(tournament.Id)).ToList())
            {
                contest.Matchups = tournamentUsers.Select(tournamentUser => new MatchupPair
                {
                    TournamentID = tournament.Id,
                    ContestId = contest.Id,
                    FirstUserID = tournamentUser,
                    SecondUserID = tournamentUser
                }).ToList();
            }

            _context.Tournaments.Find(tournament.Id).IsActive = true;
        }
    }
}