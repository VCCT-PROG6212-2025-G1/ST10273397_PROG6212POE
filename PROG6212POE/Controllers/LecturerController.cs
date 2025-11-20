using Microsoft.AspNetCore.Mvc;
using PROG6212POE.Data;
using PROG6212POE.Models;

namespace PROG6212POE.Controllers
{
    public class LecturerController : Controller
    {
        private readonly ApplicationDbContext _context;

        // Constructor injects the database context
        public LecturerController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Lecturer/SubmitClaim
        // Returns the form view for submitting a new claim
        [HttpGet]
        public IActionResult SubmitClaim()
        {
            // Provide an empty ClaimModel to the view to bind form inputs
            return View(new ClaimModel());
        }

        // POST: Lecturer/SubmitClaim
        // Handles the form submission for a new claim
        [HttpPost]
        [ValidateAntiForgeryToken] // Protects against CSRF attacks
        public async Task<IActionResult> SubmitClaim(ClaimModel claim, IFormFile SuppDocName)
        {
            // Check if the form inputs are valid based on model validation attributes
            if (!ModelState.IsValid)
            {
                return View(claim); // Return the form with validation errors
            }

            // Handle optional file upload if a file is provided
            if (SuppDocName != null && SuppDocName.Length > 0)
            {
                // Only allow PDF files
                if (SuppDocName.ContentType != "application/pdf")
                {
                    ModelState.AddModelError("SuppDoc", "Only PDF files are allowed.");
                    return View(claim);
                }

                // Check file size limit (10 MB)
                if (SuppDocName.Length > 10 * 1024 * 1024)
                {
                    ModelState.AddModelError("SuppDoc", "File size must be under 10 MB.");
                    return View(claim);
                }

                // Ensure the uploads folder exists in wwwroot
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                // Generate a unique file name to avoid conflicts
                var uniqueFileName = Guid.NewGuid() + Path.GetExtension(SuppDocName.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Save the uploaded file to the server
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await SuppDocName.CopyToAsync(stream);
                }

                // Save both the server path and the original file name
                claim.SuppDocPath = uniqueFileName;     // Used for downloading later
                claim.SuppDocName = SuppDocName.FileName;   // Displayed in the UI
            }
            else
            {
                // If no file is uploaded, set empty strings to avoid null values
                claim.SuppDocPath = "";
                claim.SuppDocName = "";
            }

            // Set the default status for a new claim
            claim.ClaimStatus = "Pending";

            // Save the claim to the database
            _context.ClaimModel.Add(claim);
            await _context.SaveChangesAsync();

            // Set a temporary success message to display on the redirected view
            TempData["Success"] = "Your claim has been submitted successfully!";
            return RedirectToAction("Overview"); // Redirect to the claims overview page
        }

        // GET: Lecturer/Overview
        // Displays all claims submitted by lecturers
        public IActionResult Overview()
        {
            // Retrieve all claims from the database
            var claims = _context.ClaimModel.ToList();
            return View(claims); // Pass claims to the view for display
        }
    }
}
