using System.Collections.Generic;
using fantasy_hoops.Dtos;

namespace fantasy_hoops.Repositories.Interfaces
{
    public interface IAchievementsRepository
    {
        public List<AchievementDto> GetExistingAchievements();
    }
}