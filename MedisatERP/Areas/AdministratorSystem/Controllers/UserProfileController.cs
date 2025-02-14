﻿using Microsoft.AspNetCore.Mvc;
using MedisatERP.Data;  // Assuming you have MedisatErpDbContext
using MedisatERP.Services;  // Assuming you have HashingHelper class
using Microsoft.EntityFrameworkCore;

namespace MedisatERP.Areas.AdministratorSystem.Controllers
{
    [Area("AdministratorSystem")]
    [Route("AdministratorSystem/[controller]/[action]/{userId?}")]
    public class UserProfileController : Controller
    {
        private readonly AdministratorSystemDbContext _dbContext;

        // Constructor to inject DbContext
        public UserProfileController(AdministratorSystemDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET: AdministratorSystem/UserProfile/Index/{userId}
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
