﻿using Microsoft.AspNetCore.Mvc;
using MedisatERP.Data;  // Assuming you have MedisatErpDbContext
using MedisatERP.Library;  // Assuming you have HashingHelper class
using Microsoft.EntityFrameworkCore;

namespace MedisatERP.Areas.CoreSystem.Controllers
{
    [Area("CoreSystem")]
    [Route("CoreSystem/[controller]/[action]/{userId?}")]
    public class UserProfileController : Controller
    {
        private readonly MedisatErpDbContext _dbContext;

        // Constructor to inject DbContext
        public UserProfileController(MedisatErpDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET: CoreSystem/UserProfile/Index/{userId}
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

                // Retrieve the user and include the roles (eager loading)
                var user = await _dbContext.AspNetUsers
                                           .Where(c => c.Id == decodedUserId)
                                           .Include(u => u.Roles)  // Include roles
                                           .FirstOrDefaultAsync();

                if (user == null)
                {
                    return NotFound(); // Return a 404 if the user is not found
                }

                // Convert roles to a comma-separated string
                var rolesString = string.Join(", ", user.Roles.Select(r => r.Name));

                // Pass the user and the roles string directly to the view
                ViewData["Roles"] = rolesString;

                return View(user);  // Passing the user model to the view
            }
            catch (FormatException)
            {
                // Handle invalid Base64 string
                return BadRequest("Invalid User ID format.");
            }
        }

    }
}
