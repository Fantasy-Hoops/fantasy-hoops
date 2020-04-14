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
            SendInvitations(newTournament, newTournament.Invites);

            return new Pair<bool, string>(isCreated, inviteUrl);
        }

        private Tournament FromViewModel(CreateTournamentViewModel tournamentModel)
        {
            Tournament.TournamentType tournamentType = (Tournament.TournamentType) tournamentModel.TournamentType;
            DateTime endDate = GetEndDate(tournamentModel.Contests);
            return new Tournament
            {
                Id = Guid.NewGuid().ToString(),
                CreatorID = tournamentModel.CreatorId,
                Type = tournamentType.GetId(),
                StartDate = tournamentModel.StartDate,
                EndDate = endDate,
                Title = tournamentModel.TournamentTitle,
                Description = tournamentModel.TournamentDescription,
                Entrants = tournamentModel.Entrants,
                DroppedContests = tournamentModel.DroppedContests,
                ImageURL = ParseIconPath(tournamentModel.TournamentIcon),
                Contests = GenerateContests(tournamentModel, endDate),
                Invites = GenerateTournamentInvites(tournamentModel.UserFriends)
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

        private DateTime GetEndDate(int numberOfContests)
        {
            return _tournamentsRepository.GetUpcomingStartDates()[numberOfContests - 1];
        }

        private List<Contest> GenerateContests(CreateTournamentViewModel model, DateTime endDate)
        {
            List<Contest> contests = new List<Contest>();
            List<DateTime> contestStartDates = _tournamentsRepository.GetUpcomingStartDates()
                .Where(date => date >= model.StartDate && date <= endDate).ToList();
            for (int i = 0; i < contestStartDates.Count && contests.Count < model.Contests; i++)
            {
                contests.Add(new Contest
                {
                    ContestStart = contestStartDates[i],
                    ContestEnd = i + 1 < contestStartDates.Count
                        ? contestStartDates[i + 1]
                        : GetLastContestEndDate(contestStartDates[i]),
                    Matchups = new List<MatchupPair>()
                });
            }

            return contests;
        }

        private DateTime GetLastContestEndDate(DateTime lastContestStartDate)
        {
            int daysToAdd = lastContestStartDate.DayOfWeek == DayOfWeek.Monday
                ? 7
                : (int) lastContestStartDate.DayOfWeek;

            return lastContestStartDate.AddDays(daysToAdd).Date;
        }

        private List<TournamentInvite> GenerateTournamentInvites(List<string> invitedUsersIds)
        {
            return invitedUsersIds.Select(userId => new TournamentInvite
            {
                Status = RequestStatus.PENDING,
                InvitedUserID = userId
            }).ToList();
        }

        private void SendInvitations(Tournament tournament, List<TournamentInvite> invites)
        {
            invites.ForEach(invite =>
            {
                _notificationRepository
                    .AddTournamentRequestNotification(tournament, invite.InvitedUserID, tournament.CreatorID);
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
    }
}