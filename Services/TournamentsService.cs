using System;
using Castle.Core;
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
            string tournamentId = newTournament.Id;
            
            return new Pair<bool, string>(isCreated, tournamentId);
        }

        private Tournament FromViewModel(CreateTournamentViewModel tournamentModel)
        {
            return new Tournament
            {
                Id = Guid.NewGuid().ToString(),
                TypeID = tournamentModel.TournamentType,
                Type = _tournamentsRepository.GetTournamentTypeById(tournamentModel.TournamentType),
                StartDate = tournamentModel.StartDate,
                EndDate = DateTime.Now,
                Name = tournamentModel.TournamentTitle,
                Description = tournamentModel.TournamentDescription,
                Entrants = tournamentModel.Entrants,
                Contests = tournamentModel.Contests,
                DroppedContests = tournamentModel.DroppedContests,
                ImageURL = tournamentModel.TournamentIcon,
            };
        }
    }
}