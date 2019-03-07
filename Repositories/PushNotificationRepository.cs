using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fantasy_hoops.Database;
using fantasy_hoops.Models;

namespace fantasy_hoops.Repositories
{
    public class PushNotificationRepository : IPushNotificationRepository
    {
        private readonly GameContext _context;

        public PushNotificationRepository(GameContext context)
        {
            _context = context;
        }
        public IEnumerable<PushSubscription> GetAllSubscribtions()
        {
            return _context.PushSubscriptions.ToList();
        }

        public IEnumerable<PushSubscription> GetUserSubscribtions(string userId)
        {
            return _context.PushSubscriptions
                .Where(sub => sub.UserID.Equals(userId))
                .ToList();
        }
    }
}
