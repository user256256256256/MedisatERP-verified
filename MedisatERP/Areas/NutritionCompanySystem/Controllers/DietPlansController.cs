using Microsoft.AspNetCore.Mvc;

namespace MedisatERP.Areas.NutritionCompanySystem.Controllers
{
    [Area("NutritionCompanySystem")]
    public class DietPlansController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
