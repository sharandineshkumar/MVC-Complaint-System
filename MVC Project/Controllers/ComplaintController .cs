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

        public ComplaintsController(IComplaintService complaintService,
                                    IHubContext<NotificationHub> hubContext,
                                    INotificationService notificationService)
        {
            _complaintService = complaintService;
            _hubContext = hubContext;
            _notificationService = notificationService;
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
            _complaintService.AddComplaint(complaint, User.Identity.Name);

            // Save notification to database
            string message = $"New complaint submitted by {User.Identity.Name}: {complaint.Title}";
            _notificationService.AddNotification(message);

            // Also push real-time via SignalR
            await _hubContext.Clients.All.SendAsync("ReceiveNotification", message);

            return RedirectToAction("RedirectToPortal", new { category = complaint.Category, id = complaint.Id });
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

            _complaintService.UpdateStatus(id, complaint.Status);

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

            return View(complaint);
        }

        [Authorize]
        [HttpPost]
        public IActionResult CitizenEdit(int id, Complaint complaint)
        {
            ModelState.Remove("Status");
            ModelState.Remove("SubmittedBy");

            if (!ModelState.IsValid)
                return View(complaint);

            _complaintService.UpdateComplaintDetails(id, complaint.Title, complaint.Description, complaint.Category);

            return RedirectToAction("Index");
        }
    }
}