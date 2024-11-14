using Microsoft.AspNetCore.Mvc;

namespace MedisatERP.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
