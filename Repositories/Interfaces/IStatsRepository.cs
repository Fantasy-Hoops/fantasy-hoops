using System.Collections.Generic;
using fantasy_hoops.Dtos;

namespace fantasy_hoops.Repositories.Interfaces
{
    public interface IStatsRepository
    {

        List<StatsDto> GetStats();
        List<StatsDto> GetStats(int id, int start, int count);

    }
}
