﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fantasy_hoops.Database;
using fantasy_hoops.Models;
using Microsoft.EntityFrameworkCore;

namespace fantasy_hoops.Repositories
{
    public class PushNotificationRepository : IPushNotificationRepository
    {
        private readonly GameContext _context;

        public PushNotificationRepository(GameContext context)
        {
            _context = context;
        }

        public void AddSubscription(PushSubscription subscription)
        {
            _context.PushSubscriptions.Add(subscription);
            _context.SaveChanges();
        }

        public IEnumerable<PushSubscription> GetAllSubscriptions()
        {
            return _context.PushSubscriptions.ToList();
        }

        public PushSubscription GetByP256Dh(string p256dh)
        {
            return _context.PushSubscriptions.Find(p256dh);
        }

        public IEnumerable<PushSubscription> GetUserSubscriptions(string userId)
        {
            return _context.PushSubscriptions
                .Where(sub => sub.UserID.Equals(userId))
                .ToList();
        }

        public void RemoveSubscription(PushSubscription subscription)
        {
            _context.PushSubscriptions.Remove(subscription);
            _context.SaveChanges();
        }

        public bool SubscriptionExists(PushSubscription subscription)
        {
            return _context.PushSubscriptions.Any(s => s.P256Dh == subscription.P256Dh);
        }
    }
}
