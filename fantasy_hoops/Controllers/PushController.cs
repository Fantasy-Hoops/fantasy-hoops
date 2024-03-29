﻿using System.Threading;
using System.Threading.Tasks;
using fantasy_hoops.Models;
using fantasy_hoops.Models.PushNotifications;
using fantasy_hoops.Models.ViewModels;
using fantasy_hoops.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace fantasy_hoops.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class PushController : ControllerBase
    {
        private readonly IPushService _pushService;

        public PushController(IPushService pushService)
        {
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

        [Authorize(Roles = "Admin")]
        [HttpPost("send/all")]
        public async Task<IActionResult> SendAll([FromBody] PushNotificationViewModel notification, [FromQuery] int? delay)
        {
            if (delay != null) Thread.Sleep((int)delay);

            await _pushService.SendToAllUsers(notification);

            return Ok("Notifications sent");
        }

        [HttpPost("send/{userId}")]
        public async Task<IActionResult> Send([FromRoute] string userId, [FromBody] PushNotificationViewModel notification, [FromQuery] int? delay)
        {
            if (delay != null) Thread.Sleep((int)delay);

            await _pushService.Send(userId, notification);

            return Ok("Notification sent");
        }
    }
}
