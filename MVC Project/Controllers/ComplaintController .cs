using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MVC_Project.Data;
using MVC_Project.Models;

namespace MVC_Project.Controllers
{
    public class ComplaintsController : Controller
    {
        private readonly AppDbContext _db;

        public ComplaintsController(AppDbContext db)
        {
            _db = db;
        }


        [Authorize]
        public IActionResult Index()
        {
            if (User.IsInRole("Admin"))  
            {
                var allComplaints = _db.Complaints.ToList();
                return View(allComplaints);
            }
            
            var userName = User.Identity.Name;
            var myComplaints = _db.Complaints
                .Where(c => c.SubmittedBy == userName)
                .ToList();

            return View(myComplaints);
        }


        [Authorize] 
        public IActionResult Create()  
        {
            if (User.IsInRole("Admin"))
                return RedirectToAction("Index");
            else

            return View();
        }

   

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(Complaint complaint)
        {
            complaint.datetime = DateTime.Now;
            complaint.Status = "Pending";
            complaint.SubmittedBy = User.Identity.Name; 

            _db.Complaints.Add(complaint);
            await _db.SaveChangesAsync();

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
            var complaint = _db.Complaints.FirstOrDefault(c => c.Id == id);
            if (complaint == null)
                return NotFound();
            else
                return View(complaint);
        }


        [Authorize(Roles = "Admin")]   
        public IActionResult Edit(int id)
        {
            var complaint = _db.Complaints.FirstOrDefault(c => c.Id == id);
            if (complaint == null)
                return NotFound();
            return View(complaint);
        }


        [Authorize(Roles = "Admin")]   
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Complaint complaint)
        {
            ModelState.Remove("Title");
            ModelState.Remove("Description");
            ModelState.Remove("Category");
            ModelState.Remove("SubmittedBy");

            

            var existing = _db.Complaints.FirstOrDefault(c => c.Id == id);
            if (existing == null)
                return NotFound();

            existing.Status = complaint.Status;
            existing.UpdatedOn = DateTime.Now;

            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        
        [Authorize(Roles = "Admin")]   
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var complaint = _db.Complaints.FirstOrDefault(c => c.Id == id);

            if (complaint == null)
                return NotFound();

            _db.Complaints.Remove(complaint);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        
        [Authorize]  
        public IActionResult CitizenEdit(int id)
        {
            
            var complaint = _db.Complaints
                .FirstOrDefault(c => c.Id == id );

            if (complaint == null)
                return Forbid();

            return View(complaint);
        }


        [Authorize]
        [HttpPost]                                                                 
        public async Task<IActionResult> CitizenEdit(int id, Complaint complaint)
        {
            

            var existing = _db.Complaints
                .FirstOrDefault(c => c.Id == id );

            if (existing == null)
                return Forbid();  

            ModelState.Remove("Status");   
            ModelState.Remove("SubmittedBy");

            if (!ModelState.IsValid)
                return View(complaint);

            existing.Title = complaint.Title;
            existing.Description = complaint.Description;
            existing.Category = complaint.Category;
            existing.UpdatedOn = DateTime.Now;

            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}


