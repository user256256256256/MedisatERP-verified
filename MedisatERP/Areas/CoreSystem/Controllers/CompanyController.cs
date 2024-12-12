using Microsoft.AspNetCore.Mvc;
using MedisatERP.Data;  // Assuming you have the MedisatErpDbContext
using MedisatERP.Library;  // Assuming you have the HashingHelper class
using Microsoft.EntityFrameworkCore;

namespace MedisatERP.Areas.CoreSystem.Controllers
{
    [Area("CoreSystem")]
    [Route("CoreSystem/[controller]/[action]/{userId?}")]
    public class CompanyController : Controller
    {
        private readonly MedisatErpDbContext _dbContext;

        // Constructor to inject DbContext
        public CompanyController(MedisatErpDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET: CoreSystem/Company/Index/{userId}
        public async Task<IActionResult> Index(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("User ID is required.");
            }

            try
            {
                // Decode the userId from the URL
                var decodedUserId = HashingHelper.DecodeString(userId);

                // Retrieve the user using the decodedUserId from the db
                var user = await _dbContext.AspNetUsers
                                           .Where(c => c.Id == decodedUserId)
                                           .FirstOrDefaultAsync();

                if (user == null)
                {
                    return NotFound(); // Return a 404 if the user is not found
                }

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
