using System.Collections.Generic;
using System.Linq;
using fantasy_hoops.Database;
using fantasy_hoops.Models;
using fantasy_hoops.Models.PushNotifications;
using fantasy_hoops.Repositories.Interfaces;

namespace fantasy_hoops.Repositories
{
    public class PushNotificationRepository : IPushNotificationRepository
    {
        private readonly GameContext _context;

        public PushNotificationRepository(GameContext gameContext = null)
        {
            _context = gameContext ?? new GameContext();
        }

        public void AddSubscription(PushSubscription subscription)
        {
            GameContext gameContext = new GameContext();
            gameContext.PushSubscriptions.Add(subscription);
            gameContext.SaveChanges();
        }

        public IEnumerable<PushSubscription> GetAllSubscriptions()
        {
            return new GameContext().PushSubscriptions.ToList();
        }

        public PushSubscription GetByP256Dh(string p256dh)
        {
            return new GameContext().PushSubscriptions.Find(p256dh);
        }

        public IEnumerable<PushSubscription> GetUserSubscriptions(string userId)
        {
            return new GameContext().PushSubscriptions
                .Where(sub => sub.UserID.Equals(userId))
                .ToList();
        }

        public void RemoveSubscription(PushSubscription subscription)
        {
            GameContext context = new GameContext();
            context.PushSubscriptions.Remove(subscription);
            context.SaveChanges();
        }

        public bool SubscriptionExists(PushSubscription subscription)
        {
            return new GameContext().PushSubscriptions.Any(s => s.P256Dh == subscription.P256Dh);
        }
    }
}
