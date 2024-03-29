﻿using System.Collections.Generic;
using System.Threading.Tasks;
using fantasy_hoops.Jobs;
using fantasy_hoops.Models;
using fantasy_hoops.Repositories;
using fantasy_hoops.Services;
using fantasy_hoops.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace fantasy_hoops.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobController : ControllerBase
    {
        private readonly IPushService _pushService;

        public JobController(IPushService pushService)
        {
            _pushService = pushService;
        }

        [HttpGet("players")]
        public IActionResult StartPlayersJob()
        {
            Task.Run(() => new PlayersJob(new ScoreService(new ScoreRepository()), false).Execute());

            return Ok("Players job started.");
        }

        [HttpGet("best-lineup")]
        public IActionResult StartBestLineup()
        {
            Task.Run(() => new BestLineupsJob().Execute());

            return Ok("Best lineups started.");
        }

        [HttpGet("achievements")]
        public IActionResult StartAchievements()
        {
            Task.Run(() => new AchievementsJob(_pushService).Execute());

            return Ok("Achievements job started.");
        }
        
        [HttpGet("league-schedule")]
        public IActionResult StartLeagueSchedule()
        {
            Task.Run(() => new ScheduleJob().Execute());

            return Ok("League schedule job started.");
        }

        [HttpGet("calculate-tournaments")]
        public IActionResult CalculateTournaments()
        {
            Task.Run(() => new UserScoreJob(null).UpdateActiveTournamentsScores(new List<UserLineup>()));
            
            return Ok("Calculating tournaments");
        }
    }
}