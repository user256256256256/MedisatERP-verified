﻿using MedisatERP.Data;
using MedisatERP.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedisatERP.Controllers
{
	[Route("[controller]/[action]/{userId?}")]
	public class TwoFAController : Controller
	{
		private readonly AdministratorSystemDbContext _dbContext;
		// Constructor to inject DbContext
		public TwoFAController(AdministratorSystemDbContext dbContext)
		{
			_dbContext = dbContext;
		}
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
