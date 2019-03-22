using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fantasy_hoops.Database;
using fantasy_hoops.Models.ViewModels;
using fantasy_hoops.Repositories;

namespace fantasy_hoops.Services
{
    public class LineupService : ILineupService
    {

        private readonly GameContext _context;
        private readonly LineupRepository _repository;

        public LineupService(GameContext context)
        {
            _context = context;
            _repository = new LineupRepository(_context);
        }

        public async void SubmitLineup(SubmitLineupViewModel model)
        {
            if (!_repository.IsUpdating(model.UserID))
                _repository.AddLineup(model);
            else
                _repository.UpdateLineup(model);

            await _context.SaveChangesAsync();
        }

    }
}
