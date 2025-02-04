using Microsoft.AspNetCore.Mvc;
using MedisatERP.Data;  // Assuming you have MedisatErpDbContext
using MedisatERP.Services;  // Assuming you have HashingHelper class
using Microsoft.EntityFrameworkCore;

namespace MedisatERP.Areas.NutritionCompanySystem.Controllers
{
    [Area("NutritionCompanySystem")]
    [Route("NutritionCompanySystem/[controller]/[action]/{userId?}/{companyId?}")]
    public class UserProfileController : Controller
    {
        private readonly AdministratorSystemDbContext _dbContext;

        // Constructor to inject DbContext
        public UserProfileController(AdministratorSystemDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET: AdministratorSystem/UserProfile/Index/{userId}
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

                // Retrieve the user and include the roles (eager loading)
                var user = await _dbContext.AspNetUsers
                                           .Where(c => c.Id == decodedUserId)
                                           .Include(u => u.AspNetUserRoles)  // Include user roles
                                               .ThenInclude(ur => ur.Role)    // Include role details
                                           .FirstOrDefaultAsync();

                if (user == null)
                {
                    return NotFound(); // Return a 404 if the user is not found
                }

                // Convert roles to a comma-separated string
                var rolesString = string.Join(", ", user.AspNetUserRoles.Select(ur => ur.Role.Name));

                // Pass the user and the roles string directly to the view
                ViewData["Roles"] = rolesString;

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
