using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Core;
using fantasy_hoops.Helpers;
using fantasy_hoops.Models.Tournaments;
using fantasy_hoops.Models.ViewModels;
using fantasy_hoops.Repositories.Interfaces;
using fantasy_hoops.Services.Interfaces;

namespace fantasy_hoops.Services
{
    public class TournamentsService : ITournamentsService
    {
        private readonly ITournamentsRepository _tournamentsRepository;

        public TournamentsService(ITournamentsRepository tournamentsRepository)
        {
            _tournamentsRepository = tournamentsRepository;
        }

        public Pair<bool, string> CreateTournament(CreateTournamentViewModel tournamentViewModel)
        {
            Tournament newTournament = FromViewModel(tournamentViewModel);

            bool isCreated = _tournamentsRepository.CreateTournament(newTournament);
            _tournamentsRepository.AddCreatorToTournament(newTournament);
            string inviteUrl = GenerateInviteUrl(newTournament.Id);

            return new Pair<bool, string>(isCreated, inviteUrl);
        }

        private Tournament FromViewModel(CreateTournamentViewModel tournamentModel)
        {
            Tournament.TournamentType tournamentType = (Tournament.TournamentType) tournamentModel.TournamentType;
            DateTime endDate = GetEndDate();
            return new Tournament
            {
                Id = Guid.NewGuid().ToString(),
                CreatorID = tournamentModel.CreatorId,
                Type = tournamentType.GetId(),
                StartDate = tournamentModel.StartDate,
                EndDate = endDate,
                Name = tournamentModel.TournamentTitle,
                Description = tournamentModel.TournamentDescription,
                Entrants = tournamentModel.Entrants,
                DroppedContests = tournamentModel.DroppedContests,
                ImageURL = ParseIconPath(tournamentModel.TournamentIcon),
                Contests = GenerateContests(tournamentModel.StartDate, endDate)
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

            return $"https://{CommonFunctions.DOMAIN}/tournaments/invitation/{tournamentId}";
        }

        private DateTime GetEndDate()
        {
            return _tournamentsRepository.GetLastEndDate();
        }

        private List<Contest> GenerateContests(DateTime startDate, DateTime endDate)
        {
            List<Contest> contests = _tournamentsRepository.GetUpcomingStartDates()
                .Where(date => date >= startDate && date <= endDate)
                .Select(date => new Contest
                {
                    ContestStart = date,
                    ContestPairs = new List<MatchupPair>()
                })
                .ToList();
            
            return contests;
        }
    }
}