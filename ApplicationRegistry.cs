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
        public ApplicationRegistry(GameContext context, IScoreService scoreService)
        {
            if (context.Teams.Count() < 30)
                Schedule(new Seed(new GameContext()))
                    .WithName("seed")
                    .ToRunNow();

            Task.Run(() => Seed.UpdateTeamColors(new GameContext()));

            Schedule(new NextGame(new GameContext(), scoreService, false))
                .WithName("nextGame")
                .ToRunNow();

            Schedule(new InjuriesSeed(new GameContext()))
                .WithName("injuries")
                .ToRunNow()
                .AndEvery(10)
                .Minutes();

            Schedule(new PhotosSeed(new GameContext()))
                .WithName("photoSeed")
                .ToRunEvery(1)
                .Days()
                .At(00, 04);
        }
    }
}
