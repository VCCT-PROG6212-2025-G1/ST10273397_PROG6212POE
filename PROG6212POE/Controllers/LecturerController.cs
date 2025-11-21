using Microsoft.AspNetCore.Mvc;
using PROG6212POE.Data;
using PROG6212POE.Models;

namespace PROG6212POE.Controllers
{
    public class LecturerController : Controller
    {
        private readonly AppDbContext _context;

        public LecturerController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult SubmitClaim()
        {
            // Get the logged-in user's ID from session
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Login");
            }

            var user = _context.UserModel.Find(userId.Value);

            if (user == null)
            {
                return RedirectToAction("Login", "Login");
            }

            // Pass user to ViewBag so the form can pre-fill read-only fields
            ViewBag.User = user;

            // Return empty ClaimModel for form binding
            return View(new ClaimModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitClaim(ClaimModel claim, IFormFile SuppDocName)
        {
            // Remove validation for fields that are set from ViewBag/hidden fields
            ModelState.Remove("UserName");
            ModelState.Remove("HourlyRate");

            if (!ModelState.IsValid)
            {
                // Re-populate ViewBag.User if returning to view with errors
                int? userId = HttpContext.Session.GetInt32("UserId");
                if (userId != null)
                {
                    ViewBag.User = _context.UserModel.Find(userId.Value);
                }
                return View(claim);
            }

            // Handle file upload
            if (SuppDocName != null && SuppDocName.Length > 0)
            {
                if (SuppDocName.ContentType != "application/pdf")
                {
                    ModelState.AddModelError("SuppDocName", "Only PDF files are allowed.");

                    // Re-populate ViewBag.User
                    int? userId = HttpContext.Session.GetInt32("UserId");
                    if (userId != null)
                    {
                        ViewBag.User = _context.UserModel.Find(userId.Value);
                    }
                    return View(claim);
                }

                if (SuppDocName.Length > 10 * 1024 * 1024)
                {
                    ModelState.AddModelError("SuppDocName", "File size must be under 10 MB.");

                    // Re-populate ViewBag.User
                    int? userId = HttpContext.Session.GetInt32("UserId");
                    if (userId != null)
                    {
                        ViewBag.User = _context.UserModel.Find(userId.Value);
                    }
                    return View(claim);
                }

                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = Guid.NewGuid() + Path.GetExtension(SuppDocName.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await SuppDocName.CopyToAsync(stream);
                }

                claim.SuppDocPath = uniqueFileName;
                claim.SuppDocName = SuppDocName.FileName;
            }
            else
            {
                claim.SuppDocPath = "";
                claim.SuppDocName = "";
            }

            if (claim.HoursWorked > 180)
            {
                ModelState.AddModelError("HoursWorked", "Cannot work more than 180 hours in a month");

                // Re-populate ViewBag.User
                int? userId = HttpContext.Session.GetInt32("UserId");
                if (userId != null)
                {
                    ViewBag.User = _context.UserModel.Find(userId.Value);
                }
                return View(claim);
            }

            claim.ClaimId = _context.ClaimModel.ToList().Count + 1;

            // Set default status
            claim.ClaimStatus = "Pending";

            // Save claim to database
            _context.ClaimModel.Add(claim);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Your claim has been submitted successfully!";
            return RedirectToAction("Overview");
        }

        public IActionResult Overview()
        {
            // Get current user's ID from session
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Login");
            }

            // Only show claims for the current user
            var claims = _context.ClaimModel
                .Where(c => c.UserId == userId.Value)
                .ToList();

            return View(claims);
        }
    }
}