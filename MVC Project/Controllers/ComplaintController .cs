using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MVC_Project.Models;
using MVC_Project.Services;

namespace MVC_Project.Controllers
{
    public class ComplaintsController : Controller
    {
        private readonly IComplaintService _complaintService;

        public ComplaintsController(IComplaintService complaintService)
        {
            _complaintService = complaintService;
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
        public IActionResult Create(Complaint complaint)
        {
            _complaintService.AddComplaint(complaint, User.Identity.Name);
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