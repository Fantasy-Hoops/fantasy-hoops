using System.Threading;
using System.Threading.Tasks;
using fantasy_hoops.Jobs;
using fantasy_hoops.Models;
using fantasy_hoops.Services.Interfaces;
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
        private readonly IPushService _pushService;

        public JobController(IPushService pushService)
        {
            _pushService = pushService;
        }

        [HttpGet("news")]
        public IActionResult StartJobs()
        {
            bool previewStarted = false, recapStarted = false;
            try
            {
                if (JobManager.GetSchedule(NewsType.PREVIEW.GetDisplayName()) == null)
                {
                    previewStarted = true;
                    JobManager.AddJob(new NewsJob(NewsType.PREVIEW),
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

        [HttpGet("best-lineup")]
        public IActionResult StartBestLineup()
        {
            new Thread(async () => await new BestLineupsJob(_pushService).Execute()).Start();
            
            return Ok("Best lineups started.");
        }
    }
}