using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using PROG6212POE.Data;
using PROG6212POE.Models;
using static PROG6212POE.Models.UserModel;
using System;
using System.Linq;

namespace PROG6212POE.Controllers
{
    public class PCAMController : Controller
    {
        private readonly AppDbContext _context;

        // Constructor: inject DB context
        public PCAMController(AppDbContext context)
        {
            _context = context;
        }

        // ----------------------------------------------------------------------
        // GET: Programme Coordinator Claim List
        // ----------------------------------------------------------------------
        [HttpGet]
        public IActionResult PCClaimList()
        {
            try
            {
                var role = HttpContext.Session.GetString("UserRole");

                if (role != Role.ProgCoord.ToString())
                    return RedirectToAction("AccessDenied", "Login");

                var claims = _context.ClaimModel.ToList();

                ViewBag.UserRole = role;
                return View(claims);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading PC claim list: {ex.Message}");
                TempData["Error"] = "Unable to load claims. Please try again later.";
                return View(new List<ClaimModel>());
            }
        }

        // ----------------------------------------------------------------------
        // GET: Academic Manager Claim List
        // ----------------------------------------------------------------------
        [HttpGet]
        public IActionResult AMClaimList()
        {
            try
            {
                var role = HttpContext.Session.GetString("UserRole");

                if (role != Role.AcadMan.ToString())
                    return RedirectToAction("AccessDenied", "Login");

                var claims = _context.ClaimModel.ToList();
                ViewBag.UserRole = role;

                return View(claims);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading AM claim list: {ex.Message}");
                TempData["Error"] = "Unable to load claims. Please try again later.";
                return View(new List<ClaimModel>());
            }
        }

        // ----------------------------------------------------------------------
        // POST: Verify Claim (Programme Coordinator)
        // ----------------------------------------------------------------------
        [HttpPost]
        public IActionResult VerifyClaim(int id)
        {
            try
            {
                var claim = _context.ClaimModel.FirstOrDefault(c => c.ClaimId == id);

                if (claim == null)
                {
                    TempData["Error"] = "Claim not found.";
                    return RedirectToAction("PCClaimList");
                }

                claim.ClaimStatus = "Verified";
                _context.SaveChanges();

                TempData["Success"] = $"Claim #{id} has been verified!";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error verifying claim: {ex.Message}");
                TempData["Error"] = "An error occurred while verifying the claim.";
            }

            return RedirectToAction("PCClaimList");
        }

        // ----------------------------------------------------------------------
        // POST: Approve Claim (Academic Manager)
        // ----------------------------------------------------------------------
        [HttpPost]
        public IActionResult ApproveClaim(int id)
        {
            try
            {
                var claim = _context.ClaimModel.FirstOrDefault(c => c.ClaimId == id);

                if (claim == null)
                {
                    TempData["Error"] = "Claim not found.";
                    return RedirectToAction("AMClaimList");
                }

                claim.ClaimStatus = "Approved";
                _context.SaveChanges();

                TempData["Success"] = $"Claim #{id} has been approved!";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error approving claim: {ex.Message}");
                TempData["Error"] = "An error occurred while approving the claim.";
            }

            return RedirectToAction("AMClaimList");
        }

        // ----------------------------------------------------------------------
        // POST: Reject Claim (Programme Coordinator)
        // ----------------------------------------------------------------------
        [HttpPost]
        public IActionResult PCRejectClaim(int id)
        {
            try
            {
                var claim = _context.ClaimModel.FirstOrDefault(c => c.ClaimId == id);

                if (claim == null)
                {
                    TempData["Error"] = "Claim not found.";
                    return RedirectToAction("PCClaimList");
                }

                claim.ClaimStatus = "Rejected";
                _context.SaveChanges();

                TempData["Error"] = $"Claim #{id} has been rejected.";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error rejecting claim: {ex.Message}");
                TempData["Error"] = "An error occurred while rejecting the claim.";
            }

            return RedirectToAction("PCClaimList");
        }

        // ----------------------------------------------------------------------
        // POST: Reject Claim (Academic Manager)
        // ----------------------------------------------------------------------
        [HttpPost]
        public IActionResult AMRejectClaim(int id)
        {
            try
            {
                var claim = _context.ClaimModel.FirstOrDefault(c => c.ClaimId == id);

                if (claim == null)
                {
                    TempData["Error"] = "Claim not found.";
                    return RedirectToAction("AMClaimList");
                }

                claim.ClaimStatus = "Rejected";
                _context.SaveChanges();

                TempData["Error"] = $"Claim #{id} has been rejected.";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error rejecting claim: {ex.Message}");
                TempData["Error"] = "An error occurred while rejecting the claim.";
            }

            return RedirectToAction("AMClaimList");
        }
    }
}