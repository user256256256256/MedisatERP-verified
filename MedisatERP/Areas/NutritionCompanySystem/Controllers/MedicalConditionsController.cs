using Microsoft.AspNetCore.Mvc;

namespace MedisatERP.Areas.NutritionCompanySystem.Controllers
{
    [Area("NutritionCompanySystem")]
    public class MedicalConditionsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
