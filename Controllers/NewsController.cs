using fantasy_hoops.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using fantasy_hoops.Dtos;

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
        public List<NewsDto> Get(int start = 0, int count = 6)
        {
            return _repository.GetNews(start, count);
        }
    }
}
