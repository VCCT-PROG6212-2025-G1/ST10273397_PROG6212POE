using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PROG6212POE.Data;
using PROG6212POE.Models;
using static PROG6212POE.Models.UserModel;
using System;
using System.Linq;

namespace PROG6212POE.Controllers
{
    public class LoginController : Controller
    {
        private readonly AppDbContext _context;

        public LoginController(AppDbContext context)
        {
            _context = context;
        }

        // ----------------------------
        // GET: Login Page
        // ----------------------------
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // ----------------------------
        // POST: Handle Login Attempt
        // ----------------------------
        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            try
            {
                // Validate input fields
                if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                {
                    ModelState.AddModelError("", "Please fill in all details");
                    return View();
                }

                // Attempt to fetch matching user
                var user = _context.UserModel.FirstOrDefault(u => u.Email == email);

                if (user == null || user.Password != password)
                {
                    ModelState.AddModelError("", "Invalid email or password");
                    return View();
                }

                // Save user data to session
                HttpContext.Session.SetString("UserRole", user.UserRole.ToString());
                HttpContext.Session.SetInt32("UserId", user.UserId);
                HttpContext.Session.SetString("UserEmail", user.Email);
                HttpContext.Session.SetString("UserName", $"{user.FirstName} {user.LastName}");

                // Redirect based on role
                return user.UserRole switch
                {
                    Role.Lecturer => RedirectToAction("Overview", "Lecturer"),
                    Role.AcadMan => RedirectToAction("AMClaimList", "PCAM"),
                    Role.ProgCoord => RedirectToAction("PCClaimList", "PCAM"),
                    Role.HR => RedirectToAction("Index", "HR"),
                    _ => HandleUnknownRole()
                };
            }
            catch (Exception ex)
            {
                // Log error (actual logging recommended)
                Console.WriteLine($"Login error: {ex.Message}");

                ModelState.AddModelError("", "An unexpected error occurred. Please try again later.");
                return View();
            }
        }

        // ----------------------------
        // Helper: Handle unknown / invalid role
        // ----------------------------
        private IActionResult HandleUnknownRole()
        {
            ModelState.AddModelError("", "User role not recognized");
            return View("Login");
        }

        // ----------------------------
        // GET: Logout and clear session
        // ----------------------------
        [HttpGet]
        public IActionResult Logout()
        {
            try
            {
                HttpContext.Session.Clear();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Logout error: {ex.Message}");
            }

            return RedirectToAction("Login");
        }
    }
}
