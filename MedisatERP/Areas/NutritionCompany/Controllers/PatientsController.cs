using Microsoft.AspNetCore.Mvc;

namespace MedisatERP.Areas.NutritionCompany.Controllers
{
    [Area("NutritionCompany")]
    public class PatientsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
