using Microsoft.AspNetCore.Mvc;

namespace MedisatERP.Controllers
{
    public class EmailConfirmationController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
