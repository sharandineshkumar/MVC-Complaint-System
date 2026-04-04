using MVC_Project.Models;

namespace MVC_Project.Services
{
    public interface INotificationService
    {
        void AddNotification(string message);
        List<Notification> GetAllNotifications();
        int GetUnreadCount();
        void MarkAllAsRead();
    }
}