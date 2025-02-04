using Microsoft.AspNetCore.Mvc;

namespace MedisatERP.Areas.NutritionCompanySystem.Controllers
{
    [Area("NutritionCompanySystem")]
    public class FoodDatabaseController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
