using Microsoft.AspNetCore.Mvc;

namespace MedisatERP.Areas.NutritionCompanySystem.Controllers
{
    [Area("NutritionCompanySystem")]
    public class MealPlansController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
