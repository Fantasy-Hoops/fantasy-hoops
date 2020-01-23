using fantasy_hoops.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;

namespace fantasy_hoops.Repositories
{
    public interface IStatsRepository
    {

        List<StatsDto> GetStats();
        List<StatsDto> GetStats(int id, int start, int count);

    }
}
