using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MVC_Project.Services;

namespace MVC_Project.Controllers
{
    [Authorize(Roles = "Admin")]
    public class NotificationsController : Controller
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        // Returns unread count as JSON — called by navbar bell icon
        public IActionResult GetUnreadCount()
        {
            int count = _notificationService.GetUnreadCount();
            return Json(count);
        }

        // Returns all notifications as JSON — called when bell dropdown opens
        public IActionResult GetAll()
        {
            var notifications = _notificationService.GetAllNotifications();
            return Json(notifications);
        }

        // Marks all notifications as read
        [HttpPost]
        public IActionResult MarkAllRead()
        {
            _notificationService.MarkAllAsRead();
            return Json(new { success = true });
        }

        // Deletes a single notification — called by the X button
        [HttpPost]
        public IActionResult Delete(int id)
        {
            _notificationService.DeleteNotification(id);
            return Json(new { success = true });
        }
    }
}