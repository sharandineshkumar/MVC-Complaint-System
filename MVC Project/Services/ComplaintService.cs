using MVC_Project.Data;
using MVC_Project.Models;

namespace MVC_Project.Services
{
    public class ComplaintService : IComplaintService
    {
        private readonly AppDbContext _db;

        public ComplaintService(AppDbContext db)
        {
            _db = db;
        }

        public List<Complaint> GetAllComplaints()
        {
            return _db.Complaints.ToList();
        }

        public List<Complaint> GetComplaintsByUser(string userName)
        {
            return _db.Complaints
                .Where(c => c.SubmittedBy == userName)
                .ToList();
        }

        public Complaint? GetComplaintById(int id)
        {
            return _db.Complaints.FirstOrDefault(c => c.Id == id);
        }

        public void AddComplaint(Complaint complaint, string userName)
        {
            complaint.SubmittedBy = userName;
            complaint.datetime = DateTime.Now;
            complaint.Status = "Pending";

            _db.Complaints.Add(complaint);
            _db.SaveChanges();
        }

        public void UpdateStatus(int id, string newStatus, string? adminNote)
        {
            var complaint = _db.Complaints.FirstOrDefault(c => c.Id == id);
            if (complaint == null) return;

            complaint.Status = newStatus;
            complaint.AdminNote = adminNote;
            complaint.UpdatedOn = DateTime.Now;

            _db.SaveChanges();
        }

        public void UpdateComplaintDetails(int id, string title, string description, string category)
        {
            var complaint = _db.Complaints.FirstOrDefault(c => c.Id == id);
            if (complaint == null) return;

            complaint.Title = title;
            complaint.Description = description;
            complaint.Category = category;
            complaint.UpdatedOn = DateTime.Now;

            _db.SaveChanges();
        }

        public void DeleteComplaint(int id)
        {
            var complaint = _db.Complaints.FirstOrDefault(c => c.Id == id);
            if (complaint == null) return;

            _db.Complaints.Remove(complaint);
            _db.SaveChanges();
        }
    }
}