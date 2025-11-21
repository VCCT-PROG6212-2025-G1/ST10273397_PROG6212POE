using Microsoft.AspNetCore.Mvc;
using PROG6212POE.Data;
using PROG6212POE.Models;
using System.Linq;
using System.Threading.Tasks;

namespace PROG6212POE.Controllers
{
    public class HRController : Controller
    {
        private readonly AppDbContext _context;

        public HRController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var users = _context.UserModel.ToList();
            var claims = _context.ClaimModel.ToList();
            ViewBag.Claims = claims;
            return View(users);
        }

        [HttpGet]
        public IActionResult Edit(int userId)
        {
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
            var user = _context.UserModel.Find(userId);

            if (user == null)
            {
                return NotFound();
            }

            // Get all claims for this specific user
            var userClaims = _context.ClaimModel
                .Where(c => c.UserId == userId)
                .ToList();

            ViewBag.Claims = userClaims;

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
                return View(user);
            }

            user.UserId = _context.UserModel.ToList().Count + 1;
            _context.UserModel.Add(user);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}