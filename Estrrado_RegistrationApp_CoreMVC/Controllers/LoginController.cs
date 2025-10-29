using Microsoft.AspNetCore.Mvc;
using Estrrado_RegistrationApp_CoreMVC.Data;
using System.Linq;

namespace Estrrado_RegistrationApp_CoreMVC.Controllers
{
    public class LoginController : Controller
    {
        private readonly AppDbContext _context;

        public LoginController(AppDbContext context)
        {
            _context = context;
        }

        // Handle GET 
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // Handle POST 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Message = "Enter username and password";
                return View();
            }

            var user = _context.Students
                .FirstOrDefault(u => u.Username == username && u.Password == password);

            if (user != null)
            {
                return RedirectToAction("Index", "Registration");
            }

            ViewBag.Message = "Invalid credentials!";
            return View();
        }
    }
}
