using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MVC_Project.Hubs;
using MVC_Project.Models;
using MVC_Project.Services;

namespace MVC_Project.Controllers
{
    public class ComplaintsController : Controller
    {
        private readonly IComplaintService _complaintService;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly INotificationService _notificationService;
        private readonly IEmailService _emailService;

        public ComplaintsController(
            IComplaintService complaintService,
            IHubContext<NotificationHub> hubContext,
            INotificationService notificationService,
            IEmailService emailService)
        {
            _complaintService = complaintService;
            _hubContext = hubContext;
            _notificationService = notificationService;
            _emailService = emailService;
        }

        [Authorize]
        public IActionResult Index()
        {
            if (User.IsInRole("Admin"))
                return View(_complaintService.GetAllComplaints());

            var userName = User.Identity.Name;
            return View(_complaintService.GetComplaintsByUser(userName));
        }

        [Authorize]
        public IActionResult Create()
        {
            if (User.IsInRole("Admin"))
                return RedirectToAction("Index");

            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(Complaint complaint)
        {
            ModelState.Remove("Status");
            ModelState.Remove("SubmittedBy");
            ModelState.Remove("datetime");

            if (!ModelState.IsValid)
                return View(complaint);

            // Save complaint
            _complaintService.AddComplaint(complaint, User.Identity.Name);

            // Save notification
            string message = $"New complaint submitted by {complaint.SubmittedBy}: {complaint.Title}";
            _notificationService.AddNotification(message);

            // SignalR notification
            await _hubContext.Clients.All.SendAsync("ReceiveNotification", message, new
            {
                id = complaint.Id,
                title = complaint.Title,
                category = complaint.Category,
                submittedBy = complaint.SubmittedBy,
                datetime = complaint.datetime.ToString("dd MMM yyyy"),
                status = complaint.Status
            });

            // EMAIL (UPDATED ONLY HERE)
            string adminEmail = "civicadmin3@gmail.com"; // change to real email

            string subject = "New Complaint Submitted";

            //string complaintLink = $"https://localhost:7051/Complaints/Details/{complaint.Id}";

            //string body =
            //    "A new complaint has been submitted:\n\n" +

            //    $"Title: {complaint.Title}\n" +
            //    $"Category: {complaint.Category}\n" +
            //    $"Description: {complaint.Description}\n" +
            //    $"Submitted By: {complaint.SubmittedBy}\n" +
            //    $"Date: {complaint.datetime}\n\n" +

            //    $"View Complaint: {complaintLink}";

            string complaintLink = $"https://localhost:7051/Complaints/Details/{complaint.Id}";

            string body = $@"
                                <h3>New Complaint Submitted</h3>

                                <p><b>Title:</b> {complaint.Title}</p>
                                <p><b>Category:</b> {complaint.Category}</p>
                                <p><b>Description:</b> {complaint.Description}</p>
                                <p><b>Submitted By:</b> {complaint.SubmittedBy}</p>
                                <p><b>Date:</b> {complaint.datetime}</p>

                            <p>
                               <b>View Complaint:</b> 
                               <a href='{complaintLink}'>Click Here</a>
                            </p>";



            await _emailService.SendEmailAsync(adminEmail, subject, body);

            return RedirectToAction("RedirectToPortal", new
            {
                category = complaint.Category,
                id = complaint.Id
            });
        }

        [Authorize]
        public IActionResult RedirectToPortal(string category, int id)
        {
            string portalUrl;

            if (category == "Roads" || category == "Water" || category == "Sanitation")
                portalUrl = "https://cmwssb.tn.gov.in/complaints-grievance";
            else if (category == "Electricity")
                portalUrl = "https://www.tnebltd.gov.in/cgrfonline/";
            else
                portalUrl = "https://maduraicorporation.co.in/";

            ViewBag.PortalUrl = portalUrl;
            ViewBag.ComplaintId = id;
            ViewBag.Category = category;

            return View();
        }

        [Authorize]
        public IActionResult Details(int id)
        {
            var complaint = _complaintService.GetComplaintById(id);
            if (complaint == null)
                return NotFound();

            return View(complaint);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int id)
        {
            var complaint = _complaintService.GetComplaintById(id);
            if (complaint == null)
                return NotFound();

            return View(complaint);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Edit(int id, Complaint complaint)
        {
            ModelState.Remove("Title");
            ModelState.Remove("Description");
            ModelState.Remove("Category");
            ModelState.Remove("SubmittedBy");

            _complaintService.UpdateStatus(id, complaint.Status, complaint.AdminNote);

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Delete(int id)
        {
            _complaintService.DeleteComplaint(id);
            return RedirectToAction("Index");
        }

        [Authorize]
        public IActionResult CitizenEdit(int id)
        {
            var complaint = _complaintService.GetComplaintById(id);
            if (complaint == null)
                return Forbid();

            if (complaint.Status != "Pending")
                return RedirectToAction("Index");

            return View(complaint);
        }

        [Authorize]
        [HttpPost]
        public IActionResult CitizenEdit(int id, Complaint complaint)
        {
            var existing = _complaintService.GetComplaintById(id);
            if (existing == null)
                return Forbid();

            if (existing.Status != "Pending")
                return RedirectToAction("Index");

            ModelState.Remove("Status");
            ModelState.Remove("SubmittedBy");

            if (!ModelState.IsValid)
                return View(complaint);

            _complaintService.UpdateComplaintDetails(id, complaint.Title, complaint.Description, complaint.Category);

            return RedirectToAction("Index");
        }
    }
}