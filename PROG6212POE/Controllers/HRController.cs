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

        [HttpGet]
        public IActionResult Index()
        {
            var role = HttpContext.Session.GetString("UserRole");

            if (string.IsNullOrEmpty(role) || role != Role.HR.ToString())
            {
                return RedirectToAction("AccessDenied", "Login");
            }

            var users = _context.UserModel.ToList();
            var claims = _context.ClaimModel.ToList();
            ViewBag.Claims = claims;
            return View(users);
        }

        [HttpGet]
        public IActionResult Edit(int userId)
        {
            var role = HttpContext.Session.GetString("UserRole");

            if (string.IsNullOrEmpty(role) || role != Role.HR.ToString())
            {
                return RedirectToAction("AccessDenied", "Login");
            }

            var user = _context.UserModel.Find(userId);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UserModel user)
        {
            if (!ModelState.IsValid)
            {
                return View(user);
            }

            var existingUser = _context.UserModel.Find(user.UserId);

            if (existingUser == null)
            {
                return NotFound();
            }

            existingUser.FirstName = user.FirstName;
            existingUser.LastName = user.LastName;
            existingUser.Email = user.Email;
            existingUser.UserRole = user.UserRole;
            existingUser.HourlyRate = user.HourlyRate;

            if (!string.IsNullOrWhiteSpace(user.Password))
            {
                existingUser.Password = user.Password;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Details(int userId)
        {

            var role = HttpContext.Session.GetString("UserRole");

            if (string.IsNullOrEmpty(role) || role != Role.HR.ToString())
            {
                return RedirectToAction("AccessDenied", "Login");
            }

            var user = _context.UserModel.Find(userId);

            if (user == null)
            {
                return NotFound();
            }

            var userClaims = _context.ClaimModel
                .Where(c => c.UserId == userId)
                .ToList();

            ViewBag.Claims = userClaims;

            return View(user);
        }

        [HttpGet]
        public IActionResult CreateUser()
        {

            var role = HttpContext.Session.GetString("UserRole");

            if (string.IsNullOrEmpty(role) || role != Role.HR.ToString())
            {
                return RedirectToAction("AccessDenied", "Login");
            }


            return View(new UserModel());
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(UserModel user)
        {
            if (!ModelState.IsValid)
            {
                return View(user);
            }

            _context.UserModel.Add(user);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> GenerateReport(int userId)
        {
            var user = _context.UserModel.Find(userId);

            if (user == null)
            {
                return RedirectToAction("Details");
            }

            var claims = _context.ClaimModel
                .Where(c => c.UserId == userId)
                .ToList();

            // Prepare ViewBag for the rendered view
            ViewBag.Claims = claims;

            // Render the view to HTML string
            var html = await RenderViewToStringAsync("ReportTemplate", user);

            // Generate PDF from HTML
            var pdf = _pdfRenderer.RenderHtmlAsPdf(html);

            // Return PDF file
            return File(
                pdf.BinaryData,
                "application/pdf",
                $"UserReport-{user.FirstName}{user.LastName}-{DateTime.Now:dd/MM/yyy}.pdf"
            );
        }

        private async Task<string> RenderViewToStringAsync(string viewName, object model)
        {
            ViewData.Model = model;

            using var writer = new StringWriter();

            // Find the view
            var viewResult = _viewEngine.FindView(ControllerContext, viewName, false);

            if (viewResult.View == null)
            {
                throw new ArgumentNullException($"View '{viewName}' not found");
            }

            // Create view context
            var viewContext = new ViewContext(
                ControllerContext,
                viewResult.View,
                ViewData,
                TempData,
                writer,
                new HtmlHelperOptions()
            );

            // Render the view
            await viewResult.View.RenderAsync(viewContext);

            return writer.GetStringBuilder().ToString();
        }
    }
}