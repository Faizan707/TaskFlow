using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Interfaces;

namespace TaskFlow.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [Authorize(Roles = "User,Manager,Admin")]
        [HttpGet]
        public IActionResult GetNotifications()
        {
            var userId = int.Parse(User.FindFirst("userId")!.Value);
            var notifications = _notificationService.GetUserNotifications(userId);
            return Ok(notifications);
        }

        [Authorize(Roles = "User,Manager,Admin")]
        [HttpGet("unread-count")]
        public IActionResult GetUnreadCount()
        {
            var userId = int.Parse(User.FindFirst("userId")!.Value);
            var count = _notificationService.GetUnreadCount(userId);
            return Ok(new { count });
        }

        [Authorize(Roles = "User,Manager,Admin")]
        [HttpPut("{id}/read")]
        public IActionResult MarkAsRead(int id)
        {
            var userId = int.Parse(User.FindFirst("userId")!.Value);
            var updated = _notificationService.MarkAsRead(id, userId);

            if (!updated)
                return NotFound(new { message = "Notification not found" });

            return Ok(new { message = "Notification marked as read" });
        }

        [Authorize(Roles = "User,Manager,Admin")]
        [HttpPut("read-all")]
        public IActionResult MarkAllAsRead()
        {
            var userId = int.Parse(User.FindFirst("userId")!.Value);
            var count = _notificationService.MarkAllAsRead(userId);
            return Ok(new { message = "All notifications marked as read", count });
        }
    }
}
