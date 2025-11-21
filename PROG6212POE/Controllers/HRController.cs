// Cleaned and improved HRController with comments and error handling
using IronPdf;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using PROG6212POE.Data;
using PROG6212POE.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static PROG6212POE.Models.UserModel;

namespace PROG6212POE.Controllers
{
    public class HRController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ChromePdfRenderer _pdfRenderer;
        private readonly ICompositeViewEngine _viewEngine;

        public HRController(AppDbContext context, ChromePdfRenderer pdfRenderer, ICompositeViewEngine viewEngine)
        {
            _context = context;
            _pdfRenderer = pdfRenderer;
            _viewEngine = viewEngine;
        }

        // ------------------------------
        // HR Dashboard
        // ------------------------------
        [HttpGet]
        public IActionResult Index()
        {
            try
            {
                var role = HttpContext.Session.GetString("UserRole");

                if (string.IsNullOrEmpty(role) || role != Role.HR.ToString())
                    return RedirectToAction("AccessDenied", "Login");

                var users = _context.UserModel.ToList();
                var claims = _context.ClaimModel.ToList();

                ViewBag.Claims = claims;
                return View(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error loading HR dashboard: {ex.Message}");
            }
        }

        // ------------------------------
        // Edit User (GET)
        // ------------------------------
        [HttpGet]
        public IActionResult Edit(int userId)
        {
            try
            {
                var role = HttpContext.Session.GetString("UserRole");
                if (role != Role.HR.ToString())
                    return RedirectToAction("AccessDenied", "Login");

                var user = _context.UserModel.Find(userId);
                if (user == null)
                    return NotFound();

                return View(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error loading Edit page: {ex.Message}");
            }
        }

        // ------------------------------
        // Edit User (POST)
        // ------------------------------
        [HttpPost]
        public async Task<IActionResult> Edit(UserModel user)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(user);

                var existingUser = _context.UserModel.Find(user.UserId);
                if (existingUser == null)
                    return NotFound();

                // Update user fields
                existingUser.FirstName = user.FirstName;
                existingUser.LastName = user.LastName;
                existingUser.Email = user.Email;
                existingUser.UserRole = user.UserRole;
                existingUser.HourlyRate = user.HourlyRate;

                // Update password only if changed
                if (!string.IsNullOrWhiteSpace(user.Password))
                    existingUser.Password = user.Password;

                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error updating user: {ex.Message}");
            }
        }

        // ------------------------------
        // User Details
        // ------------------------------
        [HttpGet]
        public IActionResult Details(int userId)
        {
            try
            {
                var role = HttpContext.Session.GetString("UserRole");
                if (role != Role.HR.ToString())
                    return RedirectToAction("AccessDenied", "Login");

                var user = _context.UserModel.Find(userId);
                if (user == null)
                    return NotFound();

                var userClaims = _context.ClaimModel.Where(c => c.UserId == userId).ToList();
                ViewBag.Claims = userClaims;

                return View(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error loading user details: {ex.Message}");
            }
        }

        // ------------------------------
        // Create User (GET)
        // ------------------------------
        [HttpGet]
        public IActionResult CreateUser()
        {
            try
            {
                var role = HttpContext.Session.GetString("UserRole");
                if (role != Role.HR.ToString())
                    return RedirectToAction("AccessDenied", "Login");

                return View(new UserModel());
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error loading Create User page: {ex.Message}");
            }
        }

        // ------------------------------
        // Create User (POST)
        // ------------------------------
        [HttpPost]
        public async Task<IActionResult> CreateUser(UserModel user)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(user);

                user.UserId = _context.UserModel.ToList().Count + 1;
                _context.UserModel.Add(user);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error creating user: {ex.Message}");
            }
        }

        // ------------------------------
        // Generate PDF Report
        // ------------------------------
        [HttpGet]
        public async Task<IActionResult> GenerateReport(int userId)
        {

            var userRole = HttpContext.Session.GetString("UserRole");
            var currentUserId = HttpContext.Session.GetInt32("UserId");

            if (userRole != Role.HR.ToString())
            {
                return RedirectToAction("AccessDenied");
            }


            try
            {
                var user = _context.UserModel.Find(userId);
                if (user == null)
                    return NotFound("User not found.");

                var claims = _context.ClaimModel.Where(c => c.UserId == userId).ToList();
                ViewBag.Claims = claims;

                // Generate HTML
                var html = await RenderViewToStringAsync("ReportTemplate", user);
                var pdf = _pdfRenderer.RenderHtmlAsPdf(html);

                return File(
                    pdf.BinaryData,
                    "application/pdf",
                    $"UserReport-{user.FirstName}{user.LastName}-{DateTime.Now:ddMMyyyy}.pdf"
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error generating report: {ex.Message}");
            }
        }

        // ------------------------------
        // Convert Razor View to HTML
        // ------------------------------
        private async Task<string> RenderViewToStringAsync(string viewName, object model)
        {
            try
            {
                ViewData.Model = model;

                using var writer = new StringWriter();
                var viewResult = _viewEngine.FindView(ControllerContext, viewName, false);

                if (viewResult.View == null)
                    throw new ArgumentNullException($"View '{viewName}' not found.");

                var viewContext = new ViewContext(
                    ControllerContext,
                    viewResult.View,
                    ViewData,
                    TempData,
                    writer,
                    new HtmlHelperOptions()
                );

                await viewResult.View.RenderAsync(viewContext);
                return writer.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error rendering view '{viewName}': {ex.Message}");
            }
        }
    }
}
