using Microsoft.AspNetCore.Mvc;

namespace MedisatERP.Areas.NutritionCompanySystem.Controllers
{
    [Area("NutritionCompanySystem")]
    public class MealLogsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
