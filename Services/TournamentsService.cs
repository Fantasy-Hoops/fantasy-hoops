using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Core;
using fantasy_hoops.Dtos;
using fantasy_hoops.Enums;
using fantasy_hoops.Helpers;
using fantasy_hoops.Models;
using fantasy_hoops.Models.Notifications;
using fantasy_hoops.Models.Tournaments;
using fantasy_hoops.Models.ViewModels;
using fantasy_hoops.Repositories;
using fantasy_hoops.Repositories.Interfaces;
using fantasy_hoops.Services.Interfaces;

namespace fantasy_hoops.Services
{
    public class TournamentsService : ITournamentsService
    {
        private readonly ITournamentsRepository _tournamentsRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly ILeaderboardRepository _leaderboardRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPushService _pushService;

        public TournamentsService(ITournamentsRepository tournamentsRepository)
        {
            _tournamentsRepository = tournamentsRepository;
            _notificationRepository = new NotificationRepository();
            _leaderboardRepository = new LeaderboardRepository();
            _userRepository = new UserRepository();
            _pushService = new PushService();
        }

        public Pair<bool, string> CreateTournament(CreateTournamentViewModel tournamentViewModel)
        {
            Tournament newTournament = FromViewModel(tournamentViewModel);

            bool isCreated = _tournamentsRepository.CreateTournament(newTournament);
            if (!isCreated)
            {
                return new Pair<bool, string>(isCreated, "");
            }

            _tournamentsRepository.AddCreatorToTournament(newTournament);

            string inviteUrl = GenerateInviteUrl(newTournament.Id);
            SendInvitations(newTournament, tournamentViewModel.UserFriends);

            return new Pair<bool, string>(isCreated, inviteUrl);
        }

        private Tournament FromViewModel(CreateTournamentViewModel tournamentModel)
        {
            Tournament.TournamentType tournamentType = (Tournament.TournamentType) tournamentModel.TournamentType;
            Pair<List<Contest>, DateTime> contestsWithEndDate = GenerateContests(tournamentModel);
            return new Tournament
            {
                Id = Guid.NewGuid().ToString(),
                CreatorID = tournamentModel.CreatorId,
                Type = tournamentType.GetId(),
                StartDate = tournamentModel.StartDate,
                EndDate = contestsWithEndDate.Second,
                Title = tournamentModel.TournamentTitle,
                Description = tournamentModel.TournamentDescription,
                Entrants = tournamentModel.Entrants,
                DroppedContests = tournamentModel.DroppedContests,
                ImageURL = ParseIconPath(tournamentModel.TournamentIcon),
                Contests = contestsWithEndDate.First
            };
        }

        private string ParseIconPath(string imageUrl)
        {
            int nameStartIndex = imageUrl.LastIndexOf("/", StringComparison.Ordinal) + 1;
            string[] pathParts = imageUrl.Substring(nameStartIndex).Split(".");
            return string.Join('.', pathParts[0], pathParts[2]);
        }

        private string GenerateInviteUrl(string tournamentId)
        {
            if (string.IsNullOrEmpty(tournamentId))
            {
                return null;
            }

            return $"https://{CommonFunctions.DOMAIN}/tournaments/invitations/{tournamentId}";
        }

        private Pair<List<Contest>, DateTime> GenerateContests(CreateTournamentViewModel model)
        {
            List<Contest> contests = new List<Contest>();
            List<DateTime> contestStartDates = _tournamentsRepository.GetUpcomingStartDates()
                .Where(date => date >= model.StartDate).ToList();
            for (int i = 0; i < contestStartDates.Count && i < model.Contests; i++)
            {
                contests.Add(new Contest
                {
                    ContestNumber = i + 1,
                    ContestStart = contestStartDates[i],
                    ContestEnd = CommonFunctions.LastDayOfWeek(contestStartDates[i]).AddDays(1),
                    Matchups = new List<MatchupPair>()
                });
            }

            DateTime endDate = contests
                .Select(contest => contest.ContestEnd)
                .OrderByDescending(date => date)
                .LastOrDefault();

            return new Pair<List<Contest>, DateTime>(contests, endDate);
        }

        private void SendInvitations(Tournament tournament, List<string> invitedUsersIds)
        {
            invitedUsersIds.ForEach(invitedUserId =>
            {
                _notificationRepository
                    .AddTournamentRequestNotification(tournament, invitedUserId, tournament.CreatorID);
                _tournamentsRepository.ChangeInvitationStatus(tournament.Id, invitedUserId,
                    RequestStatus.PENDING);
            });
        }

        public bool AcceptInvitation(string tournamentId, string userId)
        {
            bool userAdded = _tournamentsRepository.AddUserToTournament(userId, tournamentId);
            if (!userAdded)
            {
                return false;
            }

            bool statusChanged =
                _tournamentsRepository.ChangeInvitationStatus(tournamentId, userId, RequestStatus.ACCEPTED);

            return statusChanged;
        }

        public bool DeclineInvitation(string tournamentId, string userId)
        {
            return _tournamentsRepository.ChangeInvitationStatus(tournamentId, userId, RequestStatus.DECLINED);
        }

        public User GetTournamentWinner(string tournamentId)
        {
            // TODO
            return null;
        }

        public User GetContestWinner(TournamentDetailsDto tournamentDetails, ContestDto contest)
        {
            UserLeaderboardRecordDto contestWinner = _leaderboardRepository.GetUserLeaderboard(0,
                    Int32.MaxValue, LeaderboardType.WEEKLY, null,
                    CommonFunctions.GetIso8601WeekOfYear(contest.ContestStart),
                    contest.ContestStart.Year)
                .Where(user => tournamentDetails.Standings
                    .Select(tournamentUser => tournamentUser.UserId)
                    .Contains(user.UserId)
                )
                .OrderByDescending(user => user.FP)
                .FirstOrDefault();

            if (contestWinner == null)
            {
                return null;
            }

            return _userRepository.GetUserById(contestWinner.UserId);
        }

        public User EliminateUser(Tournament dbTournament, TournamentDetailsDto tournamentDetails, ContestDto contest)
        {
            if (dbTournament.DroppedContests == 0)
            {
                return null;
            }

            int tournamentContestCount = tournamentDetails.Contests.Count;
            int tournamentDroppedContests = dbTournament.DroppedContests;
            int currentContestNumber = contest.ContestNumber;
            int firstDroppedContest = tournamentContestCount - tournamentDroppedContests + 1;

            List<TournamentUserDto> tournamentStandings = tournamentDetails.Standings
                .OrderBy(user => user.Points)
                .ToList();
            if (currentContestNumber >= firstDroppedContest)
            {
                int droppedUserIndex = currentContestNumber - firstDroppedContest;
                TournamentUserDto eliminatedUser = tournamentStandings[droppedUserIndex];
                return _tournamentsRepository.SetTournamentUserEliminated(eliminatedUser);
            }

            return null;
        }

        public void UpdateStandings(TournamentDetailsDto tournamentDetails, ContestDto contest)
        {
            List<MatchupPairDto> orderedContestUsers = contest.Matchups
                .OrderByDescending(matchup => matchup.FirstUserScore)
                .ToList();

            if ((Tournament.TournamentType) tournamentDetails.Type == Tournament.TournamentType.ONE_FOR_ALL)
            {
                for (int i = orderedContestUsers.Count - 1; i >= 0; i--)
                {
                    TournamentUser tournamentUser = _tournamentsRepository
                        .GetTournamentUser(tournamentDetails.Id, orderedContestUsers[i].FirstUser.UserId);
                    _tournamentsRepository.UpdateTournamentUserStats(tournamentUser, tournamentUser.Wins,
                        tournamentUser.Losses, tournamentUser.Points + i);
                }
            }

            if ((Tournament.TournamentType) tournamentDetails.Type == Tournament.TournamentType.MATCHUPS)
            {
                foreach (var matchup in orderedContestUsers)
                {
                    TournamentUser firstTournamentUser = _tournamentsRepository
                        .GetTournamentUser(tournamentDetails.Id, matchup.FirstUser.UserId);
                    TournamentUser secondTournamentUser = _tournamentsRepository
                        .GetTournamentUser(tournamentDetails.Id, matchup.SecondUser.UserId);

                    if (matchup.FirstUserScore > matchup.SecondUserScore)
                    {
                        _tournamentsRepository.UpdateTournamentUserStats(firstTournamentUser,
                            firstTournamentUser.Wins + 1, firstTournamentUser.Losses, firstTournamentUser.Points);
                        _tournamentsRepository.UpdateTournamentUserStats(secondTournamentUser,
                            secondTournamentUser.Wins, firstTournamentUser.Losses + 1, firstTournamentUser.Points);
                    }
                    else
                    {
                        _tournamentsRepository.UpdateTournamentUserStats(firstTournamentUser, firstTournamentUser.Wins,
                            firstTournamentUser.Losses + 1, firstTournamentUser.Points);
                        _tournamentsRepository.UpdateTournamentUserStats(secondTournamentUser,
                            secondTournamentUser.Wins + 1, firstTournamentUser.Losses, firstTournamentUser.Points);
                    }
                }
            }
        }

        public void SendCancelledTournamentNotifications(Tournament tournament)
        {
            List<string> tournamentUsersIds = _tournamentsRepository.GetTournamentUsersIds(tournament.Id);
            foreach (var userId in tournamentUsersIds)
            {
                PushNotificationViewModel notification = new PushNotificationViewModel("Fantasy Hoops Notification",
                    $"Tournament {tournament.Title} was cancelled!");
                _pushService.Send(userId, notification);
            }
        }
    }
}