using Microsoft.AspNetCore.Mvc;

namespace MedisatERP.Areas.CoreSystem.Controllers
{
    [Area("CoreSystem")]
    public class DataMigrationsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
