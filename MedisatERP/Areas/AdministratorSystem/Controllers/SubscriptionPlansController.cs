﻿using MedisatERP.Data;
using MedisatERP.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedisatERP.Areas.AdministratorSystem.Controllers
{
    [Area("AdministratorSystem")]
    [Route("AdministratorSystem/[controller]/[action]")]
    public class SubscriptionPlansController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public SubscriptionPlansController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET: Payments associated with a specific user
        public async Task<IActionResult> Index(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("User ID is required.");
            }

            try
            {
                // Decode the userId from the URL
                var decodedUserId = TranscodingService.DecodeString(userId);

                // Retrieve the user using the decodedUserId from the db
                var user = await _dbContext.AspNetUsers.Where(c => c.Id == decodedUserId)
                    .FirstOrDefaultAsync();

                if (user == null)
                {
                    return NotFound(); // Return a 404 if the user is not found
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
