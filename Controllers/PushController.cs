using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using fantasy_hoops.Database;
using fantasy_hoops.Models;
using fantasy_hoops.Models.ViewModels;
using fantasy_hoops.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace fantasy_hoops.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class PushController : ControllerBase
    {
        private readonly GameContext _context;
        private readonly IHostingEnvironment _env;
        private readonly IPushService _pushService;

        public PushController(IHostingEnvironment hostingEnvironment, IPushService pushService)
        {
            _context = new GameContext();
            _env = hostingEnvironment;
            _pushService = pushService;
        }

        [HttpGet, Route("vapidpublickey")]
        public ActionResult<string> GetVapidPublicKey()
        {
            return Ok(_pushService.GetVapidPublicKey());
        }

        [HttpPost("subscribe/{userId}")]
        public async Task<ActionResult<PushSubscription>> Subscribe([FromRoute] string userId, [FromBody] PushSubscriptionViewModel model)
        {
            var subscription = new PushSubscription
            {
                UserID = userId,
                Endpoint = model.Subscription.Endpoint,
                ExpirationTime = model.Subscription.ExpirationTime,
                Auth = model.Subscription.Keys.Auth,
                P256Dh = model.Subscription.Keys.P256Dh
            };

            return await _pushService.Subscribe(subscription);
        }

        [HttpPost("unsubscribe")]
        public async Task<ActionResult<PushSubscription>> Unsubscribe([FromBody] PushSubscriptionViewModel model)
        {
            var subscription = new PushSubscription
            {
                Endpoint = model.Subscription.Endpoint,
                ExpirationTime = model.Subscription.ExpirationTime,
                Auth = model.Subscription.Keys.Auth,
                P256Dh = model.Subscription.Keys.P256Dh
            };

            await _pushService.Unsubscribe(subscription);

            return subscription;
        }

        [HttpPost("send/{userId}")]
        public async Task<IActionResult> Send([FromRoute] string userId, [FromBody] PushNotificationViewModel notification, [FromQuery] int? delay)
        {
            // if (!_env.IsDevelopment()) return Forbid();

            if (delay != null) Thread.Sleep((int)delay);

            await _pushService.Send(userId, notification);

            return Ok("Notification sent");
        }
    }

    public class PushSubscriptionViewModel
    {
        public Subscription Subscription { get; set; }
        public string DeviceId { get; set; }
    }

    public class Subscription
    {
        public string Endpoint { get; set; }
        public double? ExpirationTime { get; set; }
        public Keys Keys { get; set; }
        public WebPush.PushSubscription ToWebPushSubscription() => new WebPush.PushSubscription(Endpoint, Keys.P256Dh, Keys.Auth);
    }

    public class Keys
    {
        public string P256Dh { get; set; }
        public string Auth { get; set; }
    }
}
