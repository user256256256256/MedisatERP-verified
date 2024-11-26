using Microsoft.AspNetCore.Mvc;

namespace MedisatERP.Areas.NutritionCompany.Controllers
{
    [Area("NutritionCompany")]
    public class NutritionSystemController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
