using Microsoft.AspNetCore.Mvc;
using MedisatERP.Data;  
using Microsoft.EntityFrameworkCore;
using MedisatERP.Services;


namespace MedisatERP.Areas.SystemManager.Controllers
{
    [Area("AdministratorSystem")]
    [Route("AdministratorSystem/[controller]/[action]/{userId?}")]
    public class SystemManagerController : Controller
    {
        private readonly AdministratorSystemDbContext _dbContext;

        // Constructor to inject DbContext
        public SystemManagerController(AdministratorSystemDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET
        public async Task<IActionResult> Index(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("User ID is required.");
            }
            try
            {
                // Decode the userId fromt the URL
                var decodedUserId = HashingHelper.DecodeString(userId);

                // Retrieve the user using the decodedUserId from the db
                var user = await _dbContext.AspNetUsers.Where(c => c.Id == decodedUserId)
                    .FirstOrDefaultAsync();

                if (user == null)
                {
                    return NotFound(); // Return a 404 if the company is not found
                }

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
