using System.Collections.Generic;
using fantasy_hoops.Dtos;
using fantasy_hoops.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace fantasy_hoops.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BestLineupsController : Controller
    {
        private readonly IBestLineupsRepository _bestLineupsRepository;
        
        public BestLineupsController(IBestLineupsRepository bestLineupsRepository)
        {
            _bestLineupsRepository = bestLineupsRepository;
        }

        [HttpGet]
        public List<BestLineupDto> GetBestLineups(string date, int from = 0, int limit = 10)
        {
            return _bestLineupsRepository.GetBestLineups(date, from, limit);
        }
    }
}