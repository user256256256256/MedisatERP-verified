using Microsoft.AspNetCore.Mvc;

namespace MedisatERP.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Error(string message) 
        {
            ViewData["ErrorMessage"] = message; 
            return View(); 
        }
    }
}
