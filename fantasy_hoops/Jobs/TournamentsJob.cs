using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Core.Internal;
using fantasy_hoops.Database;
using fantasy_hoops.Dtos;
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
            _tournamentsService = new TournamentsService(_tournamentsRepository, new NotificationRepository(),
                new LeaderboardRepository(), new UserRepository(), new PushService());
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
            }
        }

        public void StartMatchupsTournament(Tournament tournament)
        {
            List<string> tournamentUsers = _tournamentsRepository.GetTournamentUsersIds(tournament.Id);
            List<Tuple<string, string>>[] matchupsPermutations =
                _tournamentsService.GetMatchupsPermutations(tournamentUsers);

            if (matchupsPermutations.IsNullOrEmpty())
            {
                _context.Tournaments.Find(tournament.Id).Status = TournamentStatus.CANCELLED;
                _context.SaveChanges();
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
            _context.SaveChanges();
        }

        public void StartOneForAllTournament(Tournament tournament)
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
            _context.SaveChanges();
        }

        public void SimulateOneForAllTournament(Tournament t)
        {
            Stack<String> droppedUsersIds = new Stack<string>();
            Tournament tournament = _context.Tournaments.Find(t.Id);
            foreach (var contest in _context.Contests
                .Where(contest => contest.TournamentId.Equals(tournament.Id)).ToList())
            {
                Tuple<String, double> contestWinnerIdAndScore = Tuple.Create("", double.MinValue);
                foreach (var matchupPair in _context.TournamentMatchups
                    .Where(matchup => matchup.ContestId == contest.Id).ToList())
                {
                    if (droppedUsersIds.Contains(matchupPair.FirstUserID) ||
                        droppedUsersIds.Contains(matchupPair.SecondUserID))
                    {
                        continue;
                    }
                    
                    double userScore = GetRandomNumber(900.0, 1200.0);
                    if (userScore > contestWinnerIdAndScore.Item2)
                    {
                        contestWinnerIdAndScore = Tuple.Create(matchupPair.FirstUserID, userScore);
                    }
                    matchupPair.FirstUserScore = userScore;
                    matchupPair.SecondUserScore = userScore;
                    _context.SaveChanges();
                }

                TournamentDetailsDto tournamentDetails = _tournamentsRepository.GetTournamentDetails(t.Id);
                ContestDto contestDto = _tournamentsService.GetContestDto(contest);
                _tournamentsService.UpdateStandings(tournamentDetails, contestDto);
                string droppedUserId = _tournamentsService.EliminateUser(tournament, tournamentDetails, contestDto)?.Id;
                contest.DroppedUserId = droppedUserId;
                droppedUsersIds.Push(droppedUserId);
                contest.Winner = _context.Users.Find(contestWinnerIdAndScore.Item1);
                contest.IsFinished = true;
                _context.SaveChanges();
                new AchievementsJob(new PushService()).ExecuteContestWinnerAchievement(contest.Winner);
            }
            tournament.Status = TournamentStatus.FINISHED;
            _context.SaveChanges();
            TournamentUser tournamentUser = _context.TournamentUsers
                .Where(tuser => tuser.TournamentID.Equals(tournament.Id))
                .OrderByDescending(tuser => tuser.Points)
                .FirstOrDefault();
            tournament.Winner = _context.Users.Find(tournamentUser.UserID);
            _context.SaveChanges();
            new AchievementsJob(new PushService()).ExecuteTournamentWinnerAchievement(tournament.Winner);
        }

        public void SimulateMatchupsTournament(Tournament t)
        {
            Tournament tournament = _context.Tournaments.Find(t.Id);
            foreach (var contest in _context.Contests
                .Where(contest => contest.TournamentId.Equals(tournament.Id)).ToList())
            {
                foreach (var matchupPair in _context.TournamentMatchups
                    .Where(matchup => matchup.ContestId == contest.Id).ToList())
                {
                    matchupPair.FirstUserScore = GetRandomNumber(900.0, 1200.0);
                    matchupPair.SecondUserScore = GetRandomNumber(900.0, 1200.0);
                    TournamentUser firstUser = _context.TournamentUsers
                        .FirstOrDefault(tuser => tuser.TournamentID.Equals(tournament.Id)
                                                 && tuser.UserID.Equals(matchupPair.FirstUserID));
                    TournamentUser secondUser = _context.TournamentUsers
                        .FirstOrDefault(tuser => tuser.TournamentID.Equals(tournament.Id)
                                                 && tuser.UserID.Equals(matchupPair.SecondUserID));
                    if (firstUser == null || secondUser == null)
                    {
                        continue;
                    }

                    if (matchupPair.FirstUserScore > matchupPair.SecondUserScore)
                    {
                        firstUser.Wins += 1;
                        secondUser.Losses += 1;
                    }
                    else if (matchupPair.SecondUserScore > matchupPair.FirstUserScore)
                    {
                        firstUser.Losses += 1;
                        secondUser.Wins += 1;
                    }
                }

                contest.IsFinished = true;
            }

            _context.SaveChanges();
            TournamentUser winnerTUser = _context.TournamentUsers
                .Where(tuser => tuser.TournamentID.Equals(tournament.Id))
                .OrderByDescending(tuser => tuser.Wins - tuser.Losses)
                .FirstOrDefault();

            tournament.Winner = _context.Users.FirstOrDefault(user => user.Id.Equals(winnerTUser.UserID));
            tournament.Status = TournamentStatus.FINISHED;

            _context.SaveChanges();
            new AchievementsJob(new PushService()).ExecuteTournamentWinnerAchievement(tournament.Winner);
        }

        private double GetRandomNumber(double minimum, double maximum)
        {
            Random random = new Random();
            return Math.Round(random.NextDouble() * (maximum - minimum) + minimum, 1);
        }
    }
}