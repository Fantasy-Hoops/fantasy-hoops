using fantasy_hoops.Database;
using FluentScheduler;
using System.Linq;
using System.Threading.Tasks;
using fantasy_hoops.Jobs;
using fantasy_hoops.Services.Interfaces;

namespace fantasy_hoops
{
    public class ApplicationRegistry : Registry
    {
        public ApplicationRegistry(GameContext context, IScoreService scoreService, IPushService pushService)
        {
            if (context.Teams.Count() < 30)
                Schedule(new RostersJob(pushService))
                    .WithName("seed")
                    .ToRunNow();

            Task.Run(() => RostersJob.UpdateTeamColors(new GameContext()));

            Schedule(new NextGameJob(scoreService, pushService, false))
                .WithName("nextGame")
                .ToRunNow();

            Schedule(new InjuriesJob(pushService))
                .WithName("injuriesSeed")
                .ToRunNow()
                .AndEvery(10)
                .Minutes();

            Schedule(new PhotosJob())
                .WithName("photoSeed")
                .ToRunEvery(1)
                .Days()
                .At(00, 04);
        }
    }
}
