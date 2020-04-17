using fantasy_hoops.Models.ViewModels;
using fantasy_hoops.Repositories.Interfaces;
using fantasy_hoops.Services.Interfaces;

namespace fantasy_hoops.Services
{
    public class LineupService : ILineupService
    {
        public static readonly int MAX_PRICE = 300;
        
        private readonly ILineupRepository _lineupRepository;

        public LineupService(ILineupRepository lineupRepository)
        {
            _lineupRepository = lineupRepository;
        }

        public void SubmitLineup(SubmitLineupViewModel model)
        {
            if (!_lineupRepository.IsUpdating(model.UserID))
                _lineupRepository.AddLineup(model);
            else
                _lineupRepository.UpdateLineup(model);
        }

    }
}
