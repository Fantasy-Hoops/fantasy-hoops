using fantasy_hoops.Models;

namespace fantasy_hoops.Repositories.Interfaces
{
    public interface IScoreRepository
    {
        bool AnyPlayerStatsExists(Player player);
        double LastFiveAverage(Player player);
    }
}
