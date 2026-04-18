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
                CreatedOn = DateTime.UtcNow
            };

            _db.Notifications.Add(notification);
            _db.SaveChanges();
        }

        public List<Notification> GetAllNotifications()
        {
            return _db.Notifications
                .OrderByDescending(n => n.CreatedOn)// sort them so the newest ones come first
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

        public void DeleteNotification(int id)
        {
            var check = _db.Notifications.FirstOrDefault(n => n.Id == id);
            if (check == null) return;
            _db.Notifications.Remove(check);
            _db.SaveChanges();
        }
    }
}