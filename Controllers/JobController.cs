using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fantasy_hoops.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace fantasy_hoops.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobController : ControllerBase
    {
        // GET: api/Job
        [HttpGet("news")]
        public IActionResult StartJobs()
        {
            try
            {
                Task.Run(() => new NewsSeed(NewsSeed.NewsType.RECAPS).Execute());
                Task.Run(() => new NewsSeed(NewsSeed.NewsType.PREVIEWS).Execute());
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            return Ok("News job started successfully.");
        }

        // GET: api/Job/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Job
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/Job/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
