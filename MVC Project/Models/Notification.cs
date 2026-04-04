namespace MVC_Project.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; } = false;
        public DateTime CreatedOn { get; set; } = DateTime.Now;
    }
}