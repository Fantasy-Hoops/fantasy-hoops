using System;
using System.Collections.Generic;
using System.Linq;
using fantasy_hoops.Database;
using fantasy_hoops.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace fantasy_hoops.Controllers
{
    [Route("api/[controller]")]
    public class InjuriesController : Controller
    {

        private readonly IInjuryRepository _repository;

        public InjuriesController(IInjuryRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public IEnumerable<Object> Get()
        {
            return _repository.GetInjuries().ToList();
        }

    }
}