using System;
using System.Collections.Generic;
using fantasy_hoops.Dtos;
using fantasy_hoops.Models.Achievements;
using fantasy_hoops.Repositories.Interfaces;
using fantasy_hoops.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace fantasy_hoops.Controllers
{
    [Route("api/[controller]")]
    public class AchievementsController : Controller
    {
        private readonly IAchievementsRepository _achievementsRepository;
        private readonly IAchievementsService _achievementsService;
        
        public AchievementsController(IAchievementsRepository achievementsRepository, IAchievementsService achievementsService)
        {
            _achievementsRepository = achievementsRepository;
            _achievementsService = achievementsService;
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

        [HttpPost]
        public IActionResult CreateAchievement([FromBody] Achievement achievement)
        {
            bool result = _achievementsService.CreateAchievement(achievement);

            if (result)
            {
                return StatusCode(500, "Server error.");
            }

            return Ok($"Achievement {achievement.Title} created.");
        }
    }
}