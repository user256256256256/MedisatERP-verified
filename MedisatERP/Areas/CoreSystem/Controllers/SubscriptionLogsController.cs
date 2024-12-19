using MedisatERP.Data;
using MedisatERP.Library;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedisatERP.Areas.CoreSystem.Controllers
{
    [Area("CoreSystem")]
    [Route("CoreSystem/[controller]/[action]/{userId?}")]
    public class SubscriptionLogsController : Controller
    {
        private readonly MedisatErpDbContext _dbContext;

        public SubscriptionLogsController(MedisatErpDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET: Subscription logs associated with a specific user
        public async Task<IActionResult> Index(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("User ID is required.");
            }

            try
            {
                var decodedUserId = HashingHelper.DecodeString(userId);

                var user = await _dbContext.AspNetUsers.Where(c => c.Id == decodedUserId)
                    .FirstOrDefaultAsync();

                if (user == null)
                {
                    return NotFound();
                }          

                return View(user);
            }
            catch (FormatException)
            {
                return BadRequest("Invalid User ID format.");
            }
        }
    }
}
