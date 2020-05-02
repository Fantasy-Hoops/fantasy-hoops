using System;
using System.Collections.Generic;
using fantasy_hoops.Dtos;
using fantasy_hoops.Helpers;
using fantasy_hoops.Models.ViewModels;
using fantasy_hoops.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace fantasy_hoops.Controllers
{
    [Route("api/[controller]")]
    public class NotificationController : Controller
    {

        private readonly INotificationRepository _repository;

        public NotificationController(INotificationRepository repository)
        {
            _repository = repository;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult GetAllNotifications()
        {
            return Ok(_repository.GetAllNotifications());
        }

        [Authorize]
        [HttpGet("{id}")]
        public IActionResult GetUserNotifications(string id, int start = 0, int count = 0)
        {
            if (!id.Equals(CommonFunctions.Instance.GetUserIdFromClaims(User)))
            {
                return StatusCode(StatusCodes.Status403Forbidden, "Unauthorized access to resource.");
            }
            
            List<NotificationDto> notifications = _repository.GetNotifications(id, start, count);

            if (notifications == null || notifications.Count == 0)
                return NotFound($"User with id {id} do not have any notifications!");
            return Ok(notifications);
        }

        [Authorize]
        [HttpPost("read")]
        public IActionResult ToggleNotification([FromBody]NotificationViewModel model)
        {
            if (!model.ReceiverID.Equals(CommonFunctions.Instance.GetUserIdFromClaims(User)))
            {
                return StatusCode(StatusCodes.Status403Forbidden, "Unauthorized access to resource.");
            }
            
            _repository.ReadNotification(model);
            return Ok();
        }

        [Authorize]
        [HttpPost("readall/{id}")]
        public IActionResult ReadAllNotifications(string id)
        {
            if (!id.Equals(CommonFunctions.Instance.GetUserIdFromClaims(User)))
            {
                return StatusCode(StatusCodes.Status403Forbidden, "Unauthorized access to resource.");
            }
            
            _repository.ReadAllNotifications(id);
            return Ok();
        }
    }
}