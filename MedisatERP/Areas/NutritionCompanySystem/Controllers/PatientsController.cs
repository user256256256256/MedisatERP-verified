using Microsoft.AspNetCore.Mvc;
using MedisatERP.Data;  // Assuming this namespace contains the DbContext
using Microsoft.EntityFrameworkCore;
using MedisatERP.Services; // Assuming this has any helper methods for decoding or processing IDs, if needed

namespace MedisatERP.Areas.NutritionCompanySystem.Controllers
{
    [Area("NutritionCompanySystem")]
    [Route("NutritionCompanySystem/[controller]/[action]/{userId?}/{companyId?}")]
    public class PatientsController : Controller
    {
        private readonly AdministratorSystemDbContext _dbContext;

        // Constructor to inject the DbContext
        public PatientsController(AdministratorSystemDbContext dbContext)
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

                var company = await _dbContext.Companies
                                      .Where(c => c.CompanyId == decodedCompanyId)
                                      .FirstOrDefaultAsync();

                if (user == null)
                {
                    return NotFound(); // Return a 404 if the user is not found
                }

                // Pass the user model and companyId to the view, which will be available in the layout
                ViewData["CompanyId"] = decodedCompanyId;
                ViewData["CompanyLogoFilePath"] = company.CompanyLogoFilePath;
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
