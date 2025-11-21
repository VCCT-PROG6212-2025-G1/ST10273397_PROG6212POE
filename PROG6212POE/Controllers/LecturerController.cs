// Cleaned, refactored, and fully commented LecturerController with error handling
using Microsoft.AspNetCore.Mvc;
using PROG6212POE.Data;
using PROG6212POE.Models;
using static PROG6212POE.Models.UserModel;

namespace PROG6212POE.Controllers
{
    public class LecturerController : Controller
    {
        private readonly AppDbContext _context;

        public LecturerController(AppDbContext context)
        {
            _context = context;
        }

        // ------------------------------
        // Submit Claim (GET)
        // ------------------------------
        [HttpGet]
        public IActionResult SubmitClaim()
        {
            try
            {
                // Validate lecturer role
                var role = HttpContext.Session.GetString("UserRole");
                if (string.IsNullOrEmpty(role) || role != Role.Lecturer.ToString())
                    return RedirectToAction("AccessDenied", "Login");

                // Validate logged-in user
                int? userId = HttpContext.Session.GetInt32("UserId");
                if (userId == null)
                    return RedirectToAction("Login", "Login");

                var user = _context.UserModel.Find(userId.Value);
                if (user == null)
                    return RedirectToAction("Login", "Login");

                // Send user data to the view for pre-filled fields
                ViewBag.User = user;

                return View(new ClaimModel());
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error loading claim submission page: {ex.Message}");
            }
        }

        // ------------------------------
        // Submit Claim (POST)
        // ------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitClaim(ClaimModel claim, IFormFile SuppDocName)
        {
            try
            {
                // Remove validation for view-controlled fields
                ModelState.Remove("UserName");
                ModelState.Remove("HourlyRate");

                if (!ModelState.IsValid)
                {
                    ReloadUserIntoViewBag();
                    return View(claim);
                }

                // ------------------------------
                // File Upload Handling
                // ------------------------------
                if (SuppDocName != null && SuppDocName.Length > 0)
                {
                    // Only accept PDFs
                    if (SuppDocName.ContentType != "application/pdf")
                    {
                        ModelState.AddModelError("SuppDocName", "Only PDF files are allowed.");
                        ReloadUserIntoViewBag();
                        return View(claim);
                    }

                    // Max 10 MB
                    if (SuppDocName.Length > 10 * 1024 * 1024)
                    {
                        ModelState.AddModelError("SuppDocName", "File size must be under 10 MB.");
                        ReloadUserIntoViewBag();
                        return View(claim);
                    }

                    // Ensure uploads directory exists
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    // Save file with unique name
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
                    claim.SuppDocPath = string.Empty;
                    claim.SuppDocName = string.Empty;
                }

                // ------------------------------
                // Logical Validations
                // ------------------------------
                if (claim.HoursWorked > 180)
                {
                    ModelState.AddModelError("HoursWorked", "Cannot work more than 180 hours in a month.");
                    ReloadUserIntoViewBag();
                    return View(claim);
                }

                // Assign Claim ID
                claim.ClaimId = _context.ClaimModel.Count() + 1;
                claim.ClaimStatus = "Pending";

                // Save to DB
                _context.ClaimModel.Add(claim);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Your claim has been submitted successfully!";
                return RedirectToAction("Overview");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error submitting claim: {ex.Message}");
            }
        }

        // ------------------------------
        // Lecturer Claim Overview
        // ------------------------------
        public IActionResult Overview()
        {
            try
            {
                var role = HttpContext.Session.GetString("UserRole");
                if (string.IsNullOrEmpty(role) || role != Role.Lecturer.ToString())
                    return RedirectToAction("AccessDenied", "Login");

                // Get current user's claims only
                int? userId = HttpContext.Session.GetInt32("UserId");
                if (userId == null)
                    return RedirectToAction("Login", "Login");

                var claims = _context.ClaimModel
                    .Where(c => c.UserId == userId.Value)
                    .ToList();

                return View(claims);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error loading claims overview: {ex.Message}");
            }
        }

        // ------------------------------
        // Helper Method: Reload current user for ViewBag
        // ------------------------------
        private void ReloadUserIntoViewBag()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId != null)
                ViewBag.User = _context.UserModel.Find(userId.Value);
        }
    }
}
