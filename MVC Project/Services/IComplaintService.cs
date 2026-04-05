using MVC_Project.Models;

namespace MVC_Project.Services
{
    public interface IComplaintService
    {
        List<Complaint> GetAllComplaints();
        List<Complaint> GetComplaintsByUser(string userName);
        Complaint? GetComplaintById(int id);
        void AddComplaint(Complaint complaint, string userName);
        

        void UpdateStatus(int id, string newStatus, string? adminNote);
        void UpdateComplaintDetails(int id, string title, string description, string category);
        void DeleteComplaint(int id);
    }
}