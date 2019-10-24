using fantasy_hoops.Database;
using fantasy_hoops.Services;
using FluentScheduler;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fantasy_hoops
{
    public class ApplicationRegistry : Registry
    {
        public ApplicationRegistry(GameContext context, IScoreService scoreService, IPushService pushService)
        {
            if (context.Teams.Count() < 30)
                Schedule(new Seed(pushService))
                    .WithName("seed")
                    .ToRunNow();

            Task.Run(() => Seed.UpdateTeamColors(new GameContext()));

            Schedule(new NextGame(scoreService, pushService, false))
                .WithName("nextGame")
                .ToRunNow();

            Schedule(new InjuriesSeed(pushService))
                .WithName("injuriesSeed")
                .ToRunNow()
                .AndEvery(10)
                .Minutes();

            Schedule(new PhotosSeed())
                .WithName("photoSeed")
                .ToRunEvery(1)
                .Days()
                .At(00, 04);
        }
    }
}
