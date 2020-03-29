using System;
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
            string inviteUrl = GenerateInviteUrl(newTournament.Id);
            
            return new Pair<bool, string>(isCreated, inviteUrl);
        }

        private Tournament FromViewModel(CreateTournamentViewModel tournamentModel)
        {
            return new Tournament
            {
                Id = Guid.NewGuid().ToString(),
                CreatorID = tournamentModel.CreatorId,
                TypeID = tournamentModel.TournamentType,
                Type = _tournamentsRepository.GetTournamentTypeById(tournamentModel.TournamentType),
                StartDate = tournamentModel.StartDate,
                EndDate = GetEndDate(),
                Name = tournamentModel.TournamentTitle,
                Description = tournamentModel.TournamentDescription,
                Entrants = tournamentModel.Entrants,
                Contests = tournamentModel.Contests,
                DroppedContests = tournamentModel.DroppedContests,
                ImageURL = ParseIconPath(tournamentModel.TournamentIcon)
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
    }
}