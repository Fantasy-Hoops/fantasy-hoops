using fantasy_hoops.Database;
using FluentScheduler;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace fantasy_hoops
{
	public class Scheduler
	{
		public static async Task Run(GameContext _context)
		{
			if (_context.Teams.Count() < 30)
				await Task.Run(() => Seed.Initialize(_context));

			await Seed.UpdateTeamColors(_context);
			var registry = new Registry();
			JobManager.Initialize(registry);
			JobManager.UseUtcTime();

			JobManager.AddJob(() => NextGame.Initialize(_context, false),
					s => s.WithName("nextGame")
					.ToRunNow());

			if (bool.Parse(Environment.GetEnvironmentVariable("IS_PRODUCTION")))
				JobManager.AddJob(() => InjuriesSeed.Initialize(_context),
						s => s.WithName("injuries")
						.ToRunOnceAt(DateTime.UtcNow.AddSeconds(10))
						.AndEvery(10)
						.Minutes());

			if (bool.Parse(Environment.GetEnvironmentVariable("IS_PRODUCTION")))
				JobManager.AddJob(() => PhotosSeed.Initialize(_context),
						s => s.WithName("photos")
						.ToRunOnceAt(DateTime.UtcNow.AddMinutes(5))
						.AndEvery(1)
						.Days()
						.At(00, 04));   // 20p.m. Eastern Time
		}
	}
}
