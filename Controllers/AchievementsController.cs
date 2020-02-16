using System.Collections.Generic;
using fantasy_hoops.Dtos;
using fantasy_hoops.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace fantasy_hoops.Controllers
{
    [Route("api/[controller]")]
    public class AchievementsController : Controller
    {
        private IAchievementsRepository _achievementsRepository;
        
        public AchievementsController(IAchievementsRepository achievementsRepository)
        {
            _achievementsRepository = achievementsRepository;
        }

        [HttpGet]
        public List<AchievementDto> GetExistingAchievements()
        {
            return _achievementsRepository.GetExistingAchievements();
        }
    }
}