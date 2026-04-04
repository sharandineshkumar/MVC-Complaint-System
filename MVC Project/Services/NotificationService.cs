using MVC_Project.Data;
using MVC_Project.Models;

namespace MVC_Project.Services
{
    public class NotificationService : INotificationService
    {
        private readonly AppDbContext _db;

        public NotificationService(AppDbContext db)
        {
            _db = db;
        }

        public void AddNotification(string message)
        {
            var notification = new Notification
            {
                Message = message,
                IsRead = false,
                CreatedOn = DateTime.Now
            };

            _db.Notifications.Add(notification);
            _db.SaveChanges();
        }

        public List<Notification> GetAllNotifications()
        {
            return _db.Notifications
                .OrderByDescending(n => n.CreatedOn)
                .ToList();
        }

        public int GetUnreadCount()
        {
            return _db.Notifications.Count(n => n.IsRead == false);
        }

        public void MarkAllAsRead()
        {
            var unread = _db.Notifications.Where(n => n.IsRead == false).ToList();

            foreach (var n in unread)
            {
                n.IsRead = true;
            }

            _db.SaveChanges();
        }
    }
}