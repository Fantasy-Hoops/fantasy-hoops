using System;
using System.Collections.Generic;
using fantasy_hoops.Dtos;
using fantasy_hoops.Models;

namespace fantasy_hoops.Repositories.Interfaces
{
    public interface IBestLineupsRepository
    {
        public List<BestLineupDto> GetBestLineups(string date, int from, int limit);
    }
}