using fantasy_hoops.Helpers;
using fantasy_hoops.Models;
using FluentScheduler;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace fantasy_hoops.Database
{
    public class UserScoreSeed
    {
        public static void Initialize(GameContext context)
        {
            if (JobManager.RunningSchedules.Any(s => !s.Name.Equals("userScore")))
            {
                JobManager.AddJob(() => Initialize(context),
                s => s.WithName("userScore")
                .ToRunOnceIn(30)
                .Seconds());
                return;
            }
            Update(context);
        }

        private static void Update(GameContext context)
        {
            var allPlayers = context.Lineups.Where(x => x.Date == CommonFunctions.UTCToEastern(NextGame.PREVIOUS_GAME) && !x.Calculated)
                .Include(x => x.Player).ThenInclude(x => x.Stats)
                .ToList();

            if (allPlayers.Count == 0)
                return;

            foreach (var player in allPlayers)
            {
                player.FP = player.Player.Stats
                    .Where(s => s.Date >= CommonFunctions.UTCToEastern(NextGame.PREVIOUS_GAME) && s.Date <= CommonFunctions.UTCToEastern(NextGame.PREVIOUS_LAST_GAME))
                    .Select(x => x.FP).FirstOrDefault();
                player.Calculated = true;
            }

            context.Lineups
                .Where(x => x.Date == NextGame.PREVIOUS_GAME)
                .Select(x => x.UserID)
                .Distinct()
                .ToList()
                .ForEach(userID =>
                {
                    var userScore = Math.Round(allPlayers
                        .Where(x => x.Date == CommonFunctions.UTCToEastern(NextGame.PREVIOUS_GAME)
                                && x.UserID.Equals(userID))
                        .Select(x => x.FP).Sum(), 1);

                    var gs = new GameScoreNotification
                    {
                        UserID = userID,
                        ReadStatus = false,
                        DateCreated = DateTime.UtcNow,
                        Score = userScore
                    };

                    context.GameScoreNotifications.Add(gs);
                });
            context.SaveChanges();
        }
    }
}
