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

        
        public IActionResult GetUnreadCount()
        {
            int count = _notificationService.GetUnreadCount();
            return Json(count);
        }

       
        public IActionResult GetAll()
        {
            var notifications = _notificationService.GetAllNotifications();
            return Json(notifications);
        }

        
        [HttpPost]
        public IActionResult MarkAllRead()
        {
            _notificationService.MarkAllAsRead();
            return Json(new { success = true });
        }

        
        [HttpPost]
        public IActionResult Delete(int id)
        {
            _notificationService.DeleteNotification(id);
            return Json(new { success = true });
        }
    }
}