using Microsoft.AspNetCore.Mvc;
using PROG6212POE.Data;
using PROG6212POE.Models;

namespace PROG6212POE.Controllers
{
    public class PCAMController : Controller
    {
        private readonly AppDbContext _context;

        // Constructor: injects the database context
        public PCAMController(AppDbContext context)
        {
            _context = context;
        }

        // GET: PCAM/ClaimList
        // Displays all claims for review by Programme Coordinators or Academic Managers
        public IActionResult PCClaimList(UserModel user)
        {
            // Retrieve all claims from the database
            var claims = _context.ClaimModel.ToList();

            // Store current user role from session to handle role-based view logic
            ViewBag.UserRole = HttpContext.Session.GetString("UserRole");

            // Pass claims to the view
            return View(claims);
        }

        public IActionResult AMClaimList(UserModel user)
        {
            // Retrieve all claims from the database
            var claims = _context.ClaimModel.ToList();

            // Store current user role from session to handle role-based view logic
            ViewBag.UserRole = HttpContext.Session.GetString("UserRole");

            // Pass claims to the view
            return View(claims);
        }

        // POST: PCAM/VerifyClaim
        // Used by Programme Coordinator to mark a claim as "Verified"
        [HttpPost]
        public IActionResult VerifyClaim(int id)
        {
            // Find the claim by its ID
            var claim = _context.ClaimModel.FirstOrDefault(c => c.ClaimId == id);

            if (claim != null)
            {
                // Update claim status
                claim.ClaimStatus = "Verified";

                // Save changes to the database
                _context.SaveChanges();

                // Optional feedback message for the user
                TempData["Success"] = $"Claim #{id} has been verified!";
            }

            // Redirect back to the claim list view
            return RedirectToAction("ClaimList");
        }

        // POST: PCAM/ApproveClaim
        // Used by Academic Manager to approve a claim after verification
        [HttpPost]
        public IActionResult ApproveClaim(int id)
        {
            var claim = _context.ClaimModel.FirstOrDefault(c => c.ClaimId == id);

            if (claim != null)
            {
                // Update claim status to Approved
                claim.ClaimStatus = "Approved";

                _context.SaveChanges();

                // Feedback message
                TempData["Success"] = $"Claim #{id} has been approved!";
            }

            return RedirectToAction("ClaimList");
        }

        // POST: PCAM/RejectClaim
        // Used by both Programme Coordinator and Academic Manager to reject a claim
        [HttpPost]
        public IActionResult RejectClaim(int id)
        {
            var claim = _context.ClaimModel.FirstOrDefault(c => c.ClaimId == id);

            if (claim != null)
            {
                // Update claim status to Rejected
                claim.ClaimStatus = "Rejected";

                _context.SaveChanges();

                // Feedback message
                TempData["Error"] = $"Claim #{id} has been rejected.";
            }

            return RedirectToAction("ClaimList");
        }
    }
}
