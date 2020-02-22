using System.Threading.Tasks;
using fantasy_hoops.Jobs;
using fantasy_hoops.Models;
using FluentScheduler;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Extensions;

namespace fantasy_hoops.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobController : ControllerBase
    {
        [HttpGet("news")]
        public IActionResult StartJobs()
        {
            bool previewStarted = false, recapStarted = false;
            try
            {
                if (JobManager.GetSchedule(NewsType.PREVIEW.GetDisplayName()) == null)
                {
                    previewStarted = true;
                    JobManager.AddJob( new NewsJob(NewsType.PREVIEW),
                        s => s.WithName(NewsType.PREVIEW.GetDisplayName())
                            .ToRunNow());
                }
                if (JobManager.GetSchedule(NewsType.RECAP.GetDisplayName()) == null)
                {
                    recapStarted = true;
                    JobManager.AddJob(new NewsJob(NewsType.RECAP),
                        s => s.WithName(NewsType.RECAP.GetDisplayName())
                            .ToRunNow());
                }
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            
            return Ok(new
            {
                Previews = previewStarted ? "Started." : "Failed.",
                Recaps = recapStarted ? "Started." : "Failed."
            });
        }
    }
}
