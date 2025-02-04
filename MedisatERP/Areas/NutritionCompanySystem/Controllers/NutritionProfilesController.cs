using Microsoft.AspNetCore.Mvc;

namespace MedisatERP.Areas.NutritionCompanySystem.Controllers
{
    [Area("NutritionCompanySystem")]
    public class NutritionProfilesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
