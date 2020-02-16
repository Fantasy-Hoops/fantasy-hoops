using System.Threading.Tasks;
using fantasy_hoops.Jobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace fantasy_hoops.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobController : ControllerBase
    {
        [HttpGet("news")]
        public IActionResult StartJobs()
        {
            try
            {
                Task.Run(() => new NewsJob(NewsJob.NewsType.RECAPS).Execute());
                Task.Run(() => new NewsJob(NewsJob.NewsType.PREVIEWS).Execute());
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            
            return Ok("News job started successfully.");
        }
    }
}
