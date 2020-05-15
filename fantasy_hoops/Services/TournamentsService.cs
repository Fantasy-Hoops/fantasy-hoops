using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Core;
using Castle.Core.Internal;
using fantasy_hoops.Dtos;
using fantasy_hoops.Helpers;
using fantasy_hoops.Models;
using fantasy_hoops.Models.Enums;
using fantasy_hoops.Models.Tournaments;
using fantasy_hoops.Models.ViewModels;
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

        public TournamentsService(ITournamentsRepository tournamentsRepository,
            INotificationRepository notificationRepository, ILeaderboardRepository leaderboardRepository,
            IUserRepository userRepository, IPushService pushService)
        {
            _tournamentsRepository = tournamentsRepository;
            _notificationRepository = notificationRepository;
            _leaderboardRepository = leaderboardRepository;
            _userRepository = userRepository;
            _pushService = pushService;
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

        public Tournament FromViewModel(CreateTournamentViewModel tournamentModel)
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

            return $"https://{CommonFunctions.Instance.DOMAIN}/tournaments/invitations/{tournamentId}";
        }

        public Pair<List<Contest>, DateTime> GenerateContests(CreateTournamentViewModel model)
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
                    ContestEnd = CommonFunctions.Instance.LastDayOfWeek(contestStartDates[i]).AddDays(1),
                    Matchups = new List<MatchupPair>()
                });
            }

            DateTime endDate = contests
                .Select(contest => contest.ContestEnd)
                .OrderByDescending(date => date)
                .FirstOrDefault();

            return new Pair<List<Contest>, DateTime>(contests, endDate);
        }

        public ContestDto GetContestDto(Contest contest)
        {
            return new ContestDto
            {
                Id = contest.Id,
                TournamentId = contest.TournamentId,
                ContestNumber = contest.ContestNumber,
                ContestStart = contest.ContestStart,
                ContestEnd = contest.ContestEnd,
                IsFinished = contest.IsFinished,
                Matchups = _tournamentsRepository.GetContestMatchups(contest.Id)
                .Select(contestPair =>
                {
                    User firstUser = _userRepository.GetUserById(contestPair.FirstUserID);
                    User secondUser = _userRepository.GetUserById(contestPair.SecondUserID);
                    return new MatchupPairDto
                    {
                        FirstUser = new TournamentUserDto
                        {
                            UserId = firstUser.Id,
                            Username = firstUser.UserName,
                            AvatarUrl = firstUser.AvatarURL
                        },
                        FirstUserScore = contestPair.FirstUserScore,
                        SecondUser = new TournamentUserDto
                        {
                            UserId = secondUser.Id,
                            Username = secondUser.UserName,
                            AvatarUrl = secondUser.AvatarURL
                        },
                        SecondUserScore = contestPair.SecondUserScore
                    };
                }).ToList()
            };
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

        public User GetTournamentWinner(TournamentDetailsDto tournamentDetails)
        {
            TournamentUserDto tournamentWinner = tournamentDetails.Standings
                .OrderByDescending(user =>
                    (Tournament.TournamentType) tournamentDetails.Type == Tournament.TournamentType.MATCHUPS
                        ? user.W - user.L
                        : user.Points)
                .FirstOrDefault();
            return _userRepository.GetUserById(tournamentWinner?.UserId);
        }

        public User GetContestWinner(TournamentDetailsDto tournamentDetails, ContestDto contest)
        {
            UserLeaderboardRecordDto contestWinner = _leaderboardRepository.GetUserLeaderboard(0,
                    Int32.MaxValue, LeaderboardType.WEEKLY, null,
                    CommonFunctions.Instance.GetIso8601WeekOfYear(contest.ContestStart),
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
                tournamentDetails.Contests
                    .Where(contestDto => contestDto.ContestStart > contest.ContestStart)
                    .ToList()
                    .ForEach(contestDto => _tournamentsRepository.RemoveUserMatchup(eliminatedUser.UserId, contestDto.Id));
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
                        .GetTournamentUser(tournamentDetails.Id, orderedContestUsers[orderedContestUsers.Count - 1 - i].FirstUser.UserId);
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

        public List<Tuple<string, string>>[] GetMatchupsPermutations(List<string> userIds)
        {
            if (userIds.IsNullOrEmpty() || userIds.Count % 2 == 1)
            {
                return null;
            }
            
            int tournamentUsersCount = userIds.Count;
            int permutationsCount = tournamentUsersCount * (tournamentUsersCount - 1);
            int distinctContestsCount = permutationsCount / (tournamentUsersCount / 2);
            List<Tuple<string, string>>[] variations = new List<Tuple<string, string>>[distinctContestsCount];

            for (int i = 0; i < tournamentUsersCount - 1; i++)
            {
                int swappedPairIndex = tournamentUsersCount + i - 1;
                variations[i] = new List<Tuple<string, string>>();
                variations[swappedPairIndex] = new List<Tuple<string, string>>();

                string firstUser = userIds[0];
                string secondUser = userIds[tournamentUsersCount - 1 - i];
                variations[i].Add(new Tuple<string, string>(firstUser, secondUser));
                variations[swappedPairIndex].Add(new Tuple<string, string>(secondUser, firstUser));

                for (int j = 1; j < tournamentUsersCount / 2; j++)
                {
                    firstUser =
                        userIds[1 + (tournamentUsersCount - i + j - 2) % (tournamentUsersCount - 1)];
                    secondUser =
                        userIds[1 + (2 * tournamentUsersCount - i - j - 3) % (tournamentUsersCount - 1)];
                    variations[i].Add(new Tuple<string, string>(firstUser, secondUser));
                    variations[swappedPairIndex].Add(new Tuple<string, string>(secondUser, firstUser));
                }
            }

            return variations;
        }
    }
}