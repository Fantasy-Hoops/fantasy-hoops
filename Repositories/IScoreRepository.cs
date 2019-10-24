using fantasy_hoops.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fantasy_hoops.Repositories
{
    public interface IScoreRepository
    {
        bool AnyPlayerStatsExists(Player player);
        double LastFiveAverage(Player player);
    }
}
