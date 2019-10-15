using fantasy_hoops.Database;
using fantasy_hoops.Repositories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace fantasy_hoops.Controllers
{
    [Route("api/[controller]")]
    public class NewsController
    {

        private readonly INewsRepository _repository;

        public NewsController(INewsRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public IEnumerable<Object> Get(int start = 0, int count = 6)
        {
            return _repository.GetNews(start, count).ToList();
        }
    }
}
