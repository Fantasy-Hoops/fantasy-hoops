using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Core.Internal;
using fantasy_hoops.Database;
using fantasy_hoops.Models.Enums;
using fantasy_hoops.Models.Tournaments;
using fantasy_hoops.Repositories;
using fantasy_hoops.Repositories.Interfaces;
using fantasy_hoops.Services;
using fantasy_hoops.Services.Interfaces;
using FluentScheduler;

namespace fantasy_hoops.Jobs
{
    public class TournamentsJob : IJob
    {
        private readonly DateTime _runtimeDate;
        private readonly GameContext _context;
        private readonly ITournamentsRepository _tournamentsRepository;
        private readonly ITournamentsService _tournamentsService;

        public TournamentsJob(DateTime runtimeDate = new DateTime())
        {
            _runtimeDate = runtimeDate;
            _context = new GameContext();
            _tournamentsRepository = new TournamentsRepository();
            _tournamentsService = new TournamentsService(_tournamentsRepository, new NotificationRepository(), new LeaderboardRepository(), new UserRepository(), new PushService());
        }

        public void Execute()
        {
            List<DateTime> upcomingStartDates =
                _tournamentsRepository.GetUpcomingStartDates().Select(date => date.Date).ToList();
            if (upcomingStartDates.Contains(_runtimeDate.Date))
            {
                StartNewTournaments();
            }
        }

        private void StartNewTournaments()
        {
            List<Tournament> tournamentsToStart = _tournamentsRepository.GetTournamentsForStartDate(_runtimeDate);
            foreach (Tournament tournament in tournamentsToStart)
            {
                if (tournament.Status != TournamentStatus.CREATED)
                {
                    continue;
                }

                int joinedUsersCount = _tournamentsRepository.GetTournamentUsersIds(tournament.Id).Count;
                if (tournament.DroppedContests >= joinedUsersCount)
                {
                    tournament.DroppedContests = joinedUsersCount - 1;
                    _context.SaveChanges();
                }

                if ((Tournament.TournamentType) tournament.Type == Tournament.TournamentType.MATCHUPS)
                {
                    if (joinedUsersCount % 2 != 0)
                    {
                        tournament.Status = TournamentStatus.CANCELLED;
                        _tournamentsService.SendCancelledTournamentNotifications(tournament);
                        continue;
                    }

                    StartMatchupsTournament(tournament);
                }
                else if ((Tournament.TournamentType) tournament.Type == Tournament.TournamentType.ONE_FOR_ALL)
                {
                    StartOneForAllTournament(tournament);
                }

                _context.SaveChanges();
            }
        }

        private void StartMatchupsTournament(Tournament tournament)
        {
            List<string> tournamentUsers = _tournamentsRepository.GetTournamentUsersIds(tournament.Id);
            List<Tuple<string, string>>[] matchupsPermutations =
                _tournamentsService.GetMatchupsPermutations(tournamentUsers);

            if (matchupsPermutations.IsNullOrEmpty())
            {
                _context.Tournaments.Find(tournament.Id).Status = TournamentStatus.CANCELLED;
                return;
            }

            var tournamentContests = _context.Contests
                .Where(x => x.TournamentId.Equals(tournament.Id))
                .OrderBy(contest => contest.ContestStart)
                .ToList();
            int i = 0;
            foreach (var contest in tournamentContests)
            {
                contest.Matchups = new List<MatchupPair>();
                foreach (Tuple<string, string> usersPair in matchupsPermutations[i])
                {
                    MatchupPair dbMatchupPair = _context.TournamentMatchups
                        .Find(tournament.Id, usersPair.Item1, usersPair.Item2, contest.Id);
                    if (dbMatchupPair == null)
                    {
                        contest.Matchups.Add(new MatchupPair
                        {
                            ContestId = contest.Id,
                            TournamentID = tournament.Id,
                            FirstUserID = usersPair.Item1,
                            SecondUserID = usersPair.Item2
                        });
                    }
                }
                i = i + 1 >= matchupsPermutations.Length ? 0 : i + 1;
            }

            _context.Tournaments.Find(tournament.Id).Status = TournamentStatus.ACTIVE;
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

            _context.Tournaments.Find(tournament.Id).Status = TournamentStatus.ACTIVE;
        }
    }
}