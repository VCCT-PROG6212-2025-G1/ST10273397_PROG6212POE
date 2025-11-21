using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PROG6212POE.Data;
using PROG6212POE.Models;
using static PROG6212POE.Models.UserModel;
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

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string Password)
        {
            // Validate all fields are filled
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(Password))
            {
                ModelState.AddModelError("", "Please Fill in All Details");
                return View();
            }

            // Find user by email using LINQ
            var user = _context.UserModel.FirstOrDefault(u => u.Email == email);

            // Check if user exists
            if (user == null)
            {
                ModelState.AddModelError("", "Invalid Email or Password");
                return View();
            }

            // Check if password matches
            if (user.Password != Password)
            {
                ModelState.AddModelError("", "Invalid Email or Password");
                return View();
            }

            // Store user information in session
            HttpContext.Session.SetString("UserRole", user.UserRole.ToString());
            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetString("UserEmail", user.Email);
            HttpContext.Session.SetString("UserName", $"{user.FirstName} {user.LastName}");

            // Redirect based on role
            if (user.UserRole == Role.Lecturer)
            {
                return RedirectToAction("Overview", "Lecturer");
            }
            else if (user.UserRole == Role.AcadMan)
            {
                return RedirectToAction("AMClaimList", "PCAM");
            }
            else if (user.UserRole == Role.ProgCoord)
            {
                return RedirectToAction("PCClaimList", "PCAM");
            }
            else if (user.UserRole == Role.HR)
            {
                return RedirectToAction("Index", "HR");
            }

            // Fallback if no role matches
            ModelState.AddModelError("", "User role not recognized");
            return View();
        }

        // Logout action to clear session
        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}