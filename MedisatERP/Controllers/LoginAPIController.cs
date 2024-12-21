using MedisatERP.Areas.CoreSystem.Models;
using MedisatERP.Data;
using MedisatERP.Library;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


[ApiController]
[Route("api/[controller]/[action]")]
public class LoginAPIController : ControllerBase
{
	private readonly UserManager<IdentityUser> _userManager;
	private readonly SignInManager<IdentityUser> _signInManager;
	private readonly MedisatErpDbContext _dbContext;

	public LoginAPIController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, MedisatErpDbContext dbContext)
	{
		_userManager = userManager;
		_signInManager = signInManager;
		_dbContext = dbContext;
	}

	[HttpGet]
	public async Task<ActionResult> LoginCheck(string email, string password)
	{
		// Validate the input parameters
		if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
		{
			Console.WriteLine("Login attempt failed: email or password is empty");
			return BadRequest(new { success = false, mresponse = "Email and password are required" });
		}

		// Log the start of the login attempt
		Console.WriteLine($"Login attempt started for email: {email}");

		try
		{
			// Retrieve the user based on the email
			var user = await _userManager.FindByEmailAsync(email);
			if (user == null)
			{
				Console.WriteLine($"User not found for email: {email}");
				return BadRequest(new { success = false, mresponse = "Invalid login attempt" });
			}

			// Log when the user is found
			Console.WriteLine($"User found for email: {email}");

			// Attempt to sign in the user with the provided password
			var result = await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: false);

			// Handle different sign-in results
			if (result.Succeeded)
			{
				// Log successful sign-in attempt
				Console.WriteLine($"Login successful for email: {email}");

				// Retrieve the custom AspNetUser (if exists) based on the email
				var aspNetUser = await _dbContext.Set<AspNetUser>().FirstOrDefaultAsync(u => u.Email == email);

				// If no custom user data is found
				if (aspNetUser == null)
				{
					Console.WriteLine($"Custom user not found for email: {email}");
					return BadRequest(new { success = false, mresponse = "User not found" });
				}

				// Log the custom user data
				Console.WriteLine($"Custom user data found for email: {email}, UserId: {aspNetUser.Id}, TwoFactorEnabled: {aspNetUser.TwoFactorEnabled}");

				// Check if the user has TwoFactorAuthentication enabled
				if (aspNetUser.TwoFactorEnabled)
				{
					// If 2FA is enabled, log the redirection logic
					Console.WriteLine($"TwoFactorAuthentication is enabled for email: {email}. Redirecting to 2FA.");

					// Encode the UserId and prepare the redirect URL
					var encodedUserId = HashingHelper.EncodeString(aspNetUser.Id);
					var redirectUrl = $"/TwoFA/Index/{encodedUserId}";

					// Log the redirect URL
					Console.WriteLine($"Redirecting to: {redirectUrl}");

					return Ok(new { success = true, mresponse = "Login successful", redirectUrl });
				}

				// Log for users who do not need 2FA
				Console.WriteLine($"TwoFactorAuthentication is not enabled for email: {email}. Proceeding with login.");

				// Default case for other users that do not need 2FA
				return Ok(new { success = true, mresponse = "Login successful" });
			}
			else if (result.IsLockedOut)
			{
				Console.WriteLine($"Login failed for email: {email}, account is locked out");
				return BadRequest(new { success = false, mresponse = "Account is locked out" });
			}
			else
			{
				Console.WriteLine($"Login failed for email: {email}, invalid password");
				return BadRequest(new { success = false, mresponse = "Invalid login attempt" });
			}
		}
		catch (Exception ex)
		{
			// Log the exception details for troubleshooting
			Console.WriteLine($"Exception occurred during login attempt for email: {email}. Error: {ex.Message}");
			return StatusCode(500, new { success = false, mresponse = "An error occurred during login. Please try again later." });
		}
	}






}
