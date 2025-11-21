using Microsoft.AspNetCore.Mvc;
using PROG6212POE.Data;
using PROG6212POE.Models;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace PROG6212POE.Controllers
{
    public class HRController : Controller
    {

        private readonly AppDbContext _context;

        //Constructor injects the database context
        public HRController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var users = _context.UserModel.ToList(); // Your user data
            var claims = _context.ClaimModel.ToList(); // Your claims data
            ViewBag.Claims = claims;
            return View(users);
        }

        [HttpGet]
        public IActionResult Edit(int userId)
        {
            var user = _context.UserModel.Find(userId);
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UserModel user)
        {

            if (!ModelState.IsValid)
            {
                return View();
            }

            if (user == null)
            {
                // Add a general validation error
                ModelState.AddModelError("", "Please Fill in All Details");
                return View(); // Return the login view with the error displayed
            }


            _context.UserModel.Update(user);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Details(int userId)
        {
            var user = _context.UserModel.Find(userId);
            return View(user);
        }

        [HttpGet]
        public IActionResult CreateUser()
        {
            return View(new UserModel());
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(UserModel user)
        {

            if (!ModelState.IsValid)
            {
                return View();
            }

            if (user == null)
            {
                // Add a general validation error
                ModelState.AddModelError("", "Please Fill in All Details");
                return View(); // Return the login view with the error displayed
            }

            _context.UserModel.Add(user);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
