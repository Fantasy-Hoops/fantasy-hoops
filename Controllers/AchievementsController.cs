using System;
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
        
        [HttpGet("user")]
        public Dictionary<String, List<UserAchievementDto>> GetAllUserAchievements()
        {
            return _achievementsRepository.GetAllUserAchievements();
        }
        
        [HttpGet("user/{userId}")]
        public List<UserAchievementDto> GetUserAchievements(String userId)
        {
            return _achievementsRepository.GetUserAchievements(userId);
        }
    }
}