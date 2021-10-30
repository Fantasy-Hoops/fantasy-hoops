using System;
using fantasy_hoops.Database;
using FluentScheduler;
using System.Linq;
using System.Threading.Tasks;
using fantasy_hoops.Helpers;
using fantasy_hoops.Jobs;
using fantasy_hoops.Services.Interfaces;
using Hangfire;

namespace fantasy_hoops
{
    public class ApplicationRegistry : Registry
    {
        public ApplicationRegistry(IScoreService scoreService, IPushService pushService,
            IBackgroundJobClient backgroundJobClient)
        {
            Schedule(new NextGameJob(scoreService, pushService, backgroundJobClient, false))
                .WithName("nextGameJob")
                .ToRunNow();
            RuntimeUtils.NEXT_GAME_CLIENT = RuntimeUtils.NEXT_GAME;
            RuntimeUtils.PLAYER_POOL_DATE = RuntimeUtils.NEXT_GAME;
        }
    }
}