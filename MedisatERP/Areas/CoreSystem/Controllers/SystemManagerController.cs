﻿using Microsoft.AspNetCore.Mvc;
using MedisatERP.Data;  
using Microsoft.EntityFrameworkCore;
using MedisatERP.Library;


namespace MedisatERP.Areas.SystemManager.Controllers
{
    [Area("CoreSystem")]
    [Route("CoreSystem/[controller]/[action]/{userId?}")]
    public class SystemManagerController : Controller
    {
        private readonly MedisatErpDbContext _dbContext;

        // Constructor to inject DbContext
        public SystemManagerController(MedisatErpDbContext dbContext)
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
