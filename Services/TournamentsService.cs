using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Core;
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
        private readonly IPushService _pushService;
        private readonly INotificationRepository _notificationRepository;
        private readonly IUserRepository _userRepository;

        public TournamentsService(ITournamentsRepository tournamentsRepository)
        {
            _tournamentsRepository = tournamentsRepository;
            _pushService = new PushService();
            _notificationRepository = new NotificationRepository();
            _userRepository = new UserRepository();
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
            for (int i = 0; i < contestStartDates.Count; i++)
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
            bool statusChanged = _tournamentsRepository.ChangeInvitationStatus(tournamentId, userId, RequestStatus.ACCEPTED);

            return userAdded && statusChanged;
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
    }
}