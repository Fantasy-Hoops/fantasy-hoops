using System.Linq;
using fantasy_hoops.Models.PushNotifications;
using fantasy_hoops.Repositories;
using fantasy_hoops.Repositories.Interfaces;
using fantasy_hoops.Tests.Mocks;
using NUnit.Framework;

namespace fantasy_hoops.Tests.Repositories
{
    public class PushNotificationRepositoryTests
    {
        private readonly ContextMock.Builder _contextBuilder = new ContextMock.Builder();

        [Test]
        public void TestGetAllSubscriptions()
        {
            var context = _contextBuilder
                .SetPushSubscriptions()
                .Build();

            IPushNotificationRepository pushNotificationRepository = new PushNotificationRepository(context);
            var allSubscriptions = pushNotificationRepository.GetAllSubscriptions();
            
            Assert.NotNull(allSubscriptions);
            Assert.AreEqual(2, allSubscriptions.Count());
        }

        [Test]
        public void TestGetUserSubscriptions()
        {
            var context = _contextBuilder
                .SetPushSubscriptions()
                .Build();

            IPushNotificationRepository pushNotificationRepository = new PushNotificationRepository(context);
            var userSubscriptions = pushNotificationRepository.GetUserSubscriptions("xxx");
            
            Assert.NotNull(userSubscriptions);
            Assert.AreEqual(1, userSubscriptions.Count());
        }

        [Test]
        public void TestSubscriptionExistsTrue()
        {
            var context = _contextBuilder
                .SetPushSubscriptions()
                .Build();

            IPushNotificationRepository pushNotificationRepository = new PushNotificationRepository(context);
            bool subscriptionExists = pushNotificationRepository.SubscriptionExists(new PushSubscription
            {
                P256Dh = "iii"
            });

            Assert.True(subscriptionExists);
        }

        [Test]
        public void TestSubscriptionExistsFalse()
        {
            var context = _contextBuilder
                .SetPushSubscriptions()
                .Build();

            IPushNotificationRepository pushNotificationRepository = new PushNotificationRepository(context);
            bool subscriptionExists = pushNotificationRepository.SubscriptionExists(new PushSubscription
            {
                P256Dh = "uiu"
            });

            Assert.False(subscriptionExists);
        }

        [Test]
        public void TestGetByP256Dh()
        {
            var context = _contextBuilder
                .SetUsers()
                .SetPushSubscriptions()
                .Build();

            IPushNotificationRepository pushNotificationRepository = new PushNotificationRepository(context);
            PushSubscription subscription = pushNotificationRepository.GetByP256Dh("iii");

            Assert.NotNull(subscription);
            Assert.AreEqual("123", subscription.Auth);
            Assert.AreEqual("xxx", subscription.UserID);
        }

        [Test]
        public void TestAddSubscription()
        {
            var context = _contextBuilder
                .SetUsers()
                .SetPushSubscriptions()
                .Build();

            IPushNotificationRepository pushNotificationRepository = new PushNotificationRepository(context);
            pushNotificationRepository.AddSubscription(new PushSubscription
            {
                Auth = "123",
                Endpoint = "yyy",
                P256Dh = "ooo",
                UserID = "xxx"
            });

            Assert.AreEqual(3, context.PushSubscriptions.Count());
        }

        [Test]
        public void TestRemoveSubscription()
        {
            var context = _contextBuilder
                .SetPushSubscriptions()
                .Build();

            IPushNotificationRepository pushNotificationRepository = new PushNotificationRepository(context);
            PushSubscription pushSubscriptionToRemove = context.PushSubscriptions.FirstOrDefault();
            pushNotificationRepository.RemoveSubscription(pushSubscriptionToRemove);
            
            Assert.AreEqual(1, context.PushSubscriptions.Count());
        }
    }
}