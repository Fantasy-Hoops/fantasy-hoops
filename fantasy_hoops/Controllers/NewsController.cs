using System;
using fantasy_hoops.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using fantasy_hoops.Dtos;
using fantasy_hoops.Repositories.Interfaces;

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
        public Dictionary<String, List<NewsDto>> Get(int start = 0, int count = 6)
        {
            return _repository.GetNews(start, count);
        }
        
        

        [HttpGet("preview")]
        public List<NewsDto> GetPreviews(int start = 0, int count = 6)
        {
            return _repository.GetPreviews(start, count);
        }
        
        

        [HttpGet("recap")]
        public List<NewsDto> GetRecaps(int start = 0, int count = 6)
        {
            return _repository.GetRecaps(start, count);
        }
    }
}
