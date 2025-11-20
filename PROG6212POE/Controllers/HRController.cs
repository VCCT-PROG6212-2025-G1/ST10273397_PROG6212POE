using Microsoft.AspNetCore.Mvc;
using PROG6212POE.Models;

namespace PROG6212POE.Controllers
{
    public class HRController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Edit()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Details()
        {
            return View();
        }

        [HttpGet]
        public IActionResult CreateUser()
        {
            return View();
        }

    }
}
