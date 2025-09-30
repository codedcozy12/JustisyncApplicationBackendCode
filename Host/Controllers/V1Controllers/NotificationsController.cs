using Application.Dto;
using Application.Interfaces.Service;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Host.Controllers.V1Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly ILogger<NotificationsController> _logger;
        public NotificationsController(ILogger<NotificationsController> logger, INotificationService notificationService)
        {
            _notificationService = notificationService;
            _logger = logger;
        }
        [HttpPost]
        [SwaggerOperation(Summary = "Create a new notification", Description = "Creates a new notification in the system.")]
        public async Task<IActionResult> CreateNotification([FromBody] NotificationCreateRequestDto request)
        {
            var result = await _notificationService.CreateNotificationAsync(request);
            if (result == null)
            {
                return BadRequest("Notification creation failed.");
            }
            return Ok(result);
        }
        [HttpGet("{notificationId:guid}")]
        [SwaggerOperation(Summary = "Get notification by Id", Description = "Retrieves details of a notification by its unique Id.")]
        public async Task<IActionResult> GetNotificationById(Guid notificationId)
        {
            var result = await _notificationService.GetNotificationByIdAsync(notificationId);
            return Ok(result);
        }
        [HttpDelete("{notificationId:guid}")]
        [SwaggerOperation(Summary = "Delete notification", Description = "Deletes a notification by its unique Id.")]
        public async Task<IActionResult> DeleteNotification(Guid notificationId)
        {
            var result = await _notificationService.DeleteNotificationAsync(notificationId);
            if (!result)
            {
                return NotFound("Notification not found or already deleted.");
            }
            return Ok("Notification deleted successfully.");
        }
        [HttpGet("user/{userId:guid}")]
        [SwaggerOperation(Summary = "Get notifications for a user", Description = "Retrieves all notifications for a specific user by their unique Id.")]
        public async Task<IActionResult> GetNotificationsByUserIdAsync(Guid userId)
        {
            var result = await _notificationService.GetNotificationsByUserIdAsync(userId);
            return Ok(result);
        }
        [HttpPut("markAllAsRead/{userId:guid}")]
        [SwaggerOperation(Summary = "Mark all notifications as read for a user", Description = "Marks all notifications as read for a specific user by their unique Id.")]
        public async Task<IActionResult> MarkAllNotificationsAsReadAsync(Guid userId)
        {
            var result = await _notificationService.MarkAllAsReadAsync(userId);
            return Ok(result);
        }
        [HttpGet("unreadnotifications/{userId:guid}")]
        [SwaggerOperation(Summary = "Get unread notifications for a user", Description = "Retrieves unread notifications for a specific user by their unique Id.")]
        public async Task<IActionResult> GetUnreadNotificationsAsync(Guid userId)
        {
            var result = await _notificationService.GetUnreadNotificationsAsync(userId);
            return Ok(result);
        }
        [HttpPut("updatenotifications/{notificationId:guid}")]
        [SwaggerOperation(Summary = "Update a notification", Description = "Updates the details of an existing notification.")]
        public async Task<IActionResult> UpdateNotificationAsync(Guid notificationId, [FromBody] NotificationUpdateRequestDto request)
        {
            var result = await _notificationService.UpdateNotificationAsync(notificationId, request);
            if (result == null)
            {
                return BadRequest("Notification update failed.");
            }
            return Ok(result);
        }
    }
}