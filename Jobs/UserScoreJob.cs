using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using fantasy_hoops.Database;
using fantasy_hoops.Dtos;
using fantasy_hoops.Helpers;
using fantasy_hoops.Models;
using fantasy_hoops.Models.Tournaments;
using fantasy_hoops.Models.ViewModels;
using fantasy_hoops.Repositories;
using fantasy_hoops.Repositories.Interfaces;
using fantasy_hoops.Services;
using fantasy_hoops.Services.Interfaces;
using FluentScheduler;

namespace fantasy_hoops.Jobs
{
    public class UserScoreJob : IJob
    {
        private static readonly Stack<GameScorePushNotificationModel> _usersPlayed =
            new Stack<GameScorePushNotificationModel>();

        private readonly GameContext _context;
        private readonly IPushService _pushService;
        private readonly ITournamentsRepository _tournamentsRepository;
        private readonly ITournamentsService _tournamentsService;

        public UserScoreJob(IPushService pushService)
        {
            _context = new GameContext();
            _pushService = pushService;
            _tournamentsRepository = new TournamentsRepository();
            _tournamentsService = new TournamentsService(_tournamentsRepository);
        }

        private async Task SendPushNotifications()
        {
            while (_usersPlayed.Count > 0)
            {
                var user = _usersPlayed.Pop();
                PushNotificationViewModel notification =
                    new PushNotificationViewModel("Fantasy Hoops Game Score",
                        $"Game has finished! Your lineup scored {user.Score} FP")
                    {
                        Actions = new List<NotificationAction> {new NotificationAction("leaderboard", "🏆 Leaderboard")}
                    };
                await _pushService.Send(user.UserID, notification);
            }
        }

        public void Execute()
        {
            var todayStats = _context.Stats
                .Where(stats => stats.Date.Equals(CommonFunctions.UTCToEastern(NextGameJob.PREVIOUS_GAME).Date))
                .Select(stats => new {stats.PlayerID, stats.FP});

            var allLineups = _context.UserLineups
                .Include(lineup => lineup.User)
                .Where(lineup => lineup.Date.Equals(CommonFunctions.UTCToEastern(NextGameJob.PREVIOUS_GAME).Date) &&
                                 !lineup.IsCalculated)
                .Include(lineup => lineup.User)
                .ToList();

            if (allLineups.Count == 0)
                return;

            _context.Users
                .AsEnumerable()
                .Except(allLineups.Select(lineup => lineup.User).AsEnumerable())
                .ToList()
                .ForEach(user => user.Streak = 0);

            foreach (var lineup in allLineups)
            {
                var selectedPlayers = new List<int> {lineup.PgID, lineup.SgID, lineup.SfID, lineup.PfID, lineup.CID};
                lineup.FP =
                    Math.Round(todayStats
                        .Where(stats => selectedPlayers.Contains(stats.PlayerID))
                        .Select(stats => stats.FP)
                        .Sum(), 1);
                lineup.IsCalculated = true;

                lineup.User.Streak++;

                _usersPlayed.Push(new GameScorePushNotificationModel
                {
                    UserID = lineup.User.Id,
                    Score = lineup.FP
                });

                var gs = new GameScoreNotification
                {
                    ReceiverID = lineup.User.Id,
                    ReadStatus = false,
                    DateCreated = DateTime.UtcNow,
                    Score = lineup.FP
                };
                _context.GameScoreNotifications.Add(gs);
            }

            _context.SaveChanges();

            UpdateActiveTournamentsScores(allLineups);
            SendPushNotifications().Wait();

            Task.Run(() => new BestLineupsJob().Execute());
            Task.Run(() => new AchievementsJob(_pushService, null, null).ExecuteAllAchievements());
        }

        public void UpdateActiveTournamentsScores(List<UserLineup> allLineups)
        {
            List<ContestDto> currentContests = _tournamentsRepository.GetAllCurrentContests();
            foreach (var contest in currentContests)
            {
                TournamentDetailsDto tournamentDetails =
                    _tournamentsRepository.GetTournamentDetails(contest.TournamentId);
                Tournament dbTournament = _context.Tournaments.Find(contest.TournamentId);

                bool isContestFinished = contest.ContestEnd.DayOfWeek == DateTime.UtcNow.DayOfWeek;
                bool isTournamentFinished = contest.ContestEnd.Date == dbTournament.EndDate.Date;

                foreach (var matchup in contest.Matchups)
                {
                    double? firstUserScore =
                        allLineups.FirstOrDefault(lineup => lineup.UserID.Equals(matchup.FirstUser.UserId))?.FP;
                    double? secondUserScore =
                        allLineups.FirstOrDefault(lineup => lineup.UserID.Equals(matchup.SecondUser.UserId))?.FP;

                    MatchupPair matchupPair = _context.TournamentMatchups
                        .Find(dbTournament.Id, matchup.FirstUser.UserId, matchup.SecondUser.UserId);
                    if (matchupPair == null)
                    {
                        continue;
                    }

                    matchupPair.FirstUserScore += firstUserScore ?? 0.0;
                    matchupPair.SecondUserScore += secondUserScore ?? 0.0;
                    _context.SaveChanges();
                }


                if (isContestFinished)
                {
                    Contest dbContest = _context.Contests.Find(contest.Id);
                    if (dbContest == null)
                    {
                        continue;
                    }
                    
                    _tournamentsService.UpdateStandings(tournamentDetails, contest);
                    if ((Tournament.TournamentType) dbTournament.Type == Tournament.TournamentType.ONE_FOR_ALL)
                    {
                        dbContest.DroppedUserId =
                            _tournamentsService.EliminateUser(dbTournament, tournamentDetails, contest)?.Id;
                    }

                    dbContest.Winner = _tournamentsService.GetContestWinner(tournamentDetails, contest);
                    dbContest.IsFinished = true;
                }

                if (isTournamentFinished)
                {
                    dbTournament.IsActive = false;
                    dbTournament.IsFinished = true;
                    dbTournament.Winner = _tournamentsService.GetTournamentWinner(dbTournament.Id);
                }

                _context.SaveChanges();
            }
        }
    }
}