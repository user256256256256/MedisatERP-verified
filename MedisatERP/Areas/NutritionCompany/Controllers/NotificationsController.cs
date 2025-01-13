using MedisatERP.Data;
using MedisatERP.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedisatERP.Areas.NutritionCompany.Controllers
{
    [Area("NutritionCompany")]
    [Route("NutritionCompany/[controller]/[action]/{userId?}/{companyId?}")]
    public class NotificationsController : Controller
    {
        private readonly MedisatErpDbContext _dbContext;

        // Constructor to inject DbContext
        public NotificationsController(MedisatErpDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public async Task<IActionResult> Index(string userId, string companyId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(companyId))
            {
                return BadRequest("User ID and valid Company ID are required.");
            }

            try
            {
                // Decode the userId from the URL
                var decodedUserId = HashingHelper.DecodeString(userId);

                var decodedCompanyId = HashingHelper.DecodeGuidID(companyId);

                // Retrieve the user using the decodedUserId from the db
                var user = await _dbContext.AspNetUsers
                                           .Where(c => c.Id == decodedUserId)
                                           .FirstOrDefaultAsync();

                if (user == null)
                {
                    return NotFound(); // Return a 404 if the user is not found
                }

                // Pass the user model and companyId to the view, which will be available in the layout
                ViewData["CompanyId"] = decodedCompanyId;
                // Pass the user model to the view, which will be available in the layout
                return View(user);
            }
            catch (FormatException)
            {
                // Handle invalid Base64 string
                return BadRequest("Invalid User ID format.");
            }
        }
    }
}
