﻿using System;
using System.Collections.Generic;
using System.Linq;
using fantasy_hoops.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace fantasy_hoops.Controllers
{
    [Route("api/[controller]")]
    public class PlayerController : Controller
    {

        private readonly IPlayerRepository _repository;

        public PlayerController(IPlayerRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public IEnumerable<Object> GetActivePlayers()
        {
            return _repository.GetActivePlayers().ToList();
        }

        [HttpGet("{id}")]
        public IActionResult GetPlayer(int id)
        {
            var player = _repository.GetPlayer(id)
                .FirstOrDefault();
            if (player == null)
                return NotFound(String.Format("Player with id {0} has not been found!", id));
            return Ok(player);
        }
    }
}