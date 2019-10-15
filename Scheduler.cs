using fantasy_hoops.Database;
using fantasy_hoops.Services;
using FluentScheduler;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace fantasy_hoops
{
	public class Scheduler
	{

        private readonly GameContext _context;
        private readonly IScoreService _scoreService;
        private static readonly Lazy<Scheduler> INSTANCE = new Lazy<Scheduler>();

        private Scheduler(GameContext context, IScoreService scoreService)
        {
            _context = context;
            _scoreService = scoreService;
        }

        public static Lazy<Scheduler> Instance
        {
            get { return INSTANCE; }
        }

        public async Task Run()
		{
			if (_context.Teams.Count() < 30)
				await Task.Run(() => Seed.Initialize(_context));

			await Seed.UpdateTeamColors(_context);
			var registry = new Registry();
			JobManager.Initialize(registry);
			JobManager.UseUtcTime();

            NextGame nextGame = new NextGame(_scoreService);

			JobManager.AddJob(() => nextGame.Initialize(_context, false),
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
