using Microsoft.AspNetCore.Mvc;
using PROG6212POE.Models;

public class LoginController : Controller
{
    // GET: Login
    // Returns the login page view
    [HttpGet]
    public IActionResult Login()
    {
        return View(); // Simply renders the login form
    }

    // POST: Login
    // Handles login form submission
    [HttpPost]
    public IActionResult Login(string Username, string Password, string Role)
    {
        // Validate all fields: role, username, password
        // Here, the password is hardcoded for testing purposes ("Password123!")
        if (string.IsNullOrEmpty(Role) ||
            string.IsNullOrEmpty(Username) ||
            string.IsNullOrEmpty(Password) || Password != "Password123!")
        {
            // Add a general validation error
            ModelState.AddModelError("", "Please Fill in All Details");
            return View(); // Return the login view with the error displayed
        }

        // Check if the user is a Lecturer
        if (Role == "Lecturer" && Username == "JayDoe")
        {
            // Set the user role in session for authorization checks later
            HttpContext.Session.SetString("UserRole", "Lecturer");

            // Redirect to Lecturer's dashboard (Overview page)
            return RedirectToAction("Overview", "Lecturer");
        }
        // Check if the user is a Programme Coordinator
        else if (Role == "ProgrammeCoordinator" && Username == "JaneDoe")
        {
            HttpContext.Session.SetString("UserRole", "ProgrammeCoordinator");

            // Redirect to PCAM's Claim List page
            return RedirectToAction("ClaimList", "PCAM");
        }
        // Check if the user is an Academic Manager
        else if (Role == "AcademicManager" && Username == "JohnDoe")
        {
            HttpContext.Session.SetString("UserRole", "AcademicManager");

            // Redirect to PCAM's Claim List page
            return RedirectToAction("ClaimList", "PCAM");
        }

        // If none of the credentials match, reload login page
        return RedirectToAction("Login");
    }
}
