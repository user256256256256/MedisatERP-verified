using MedisatERP.Areas.CoreSystem.Models;
using MedisatERP.Data;
using MedisatERP.Library;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


[ApiController]
[Route("api/[controller]/[action]")]
public class LoginAPIController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly ApplicationDbContext _dbContext;
    private readonly RoleRedirectService _roleRedirectService;

    public LoginAPIController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, ApplicationDbContext dbContext, RoleRedirectService roleRedirectService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _dbContext = dbContext;
        _roleRedirectService = roleRedirectService;

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

        Console.WriteLine($"Login attempt started for email: {email}");

        try
        {
            // Try to find the user by email
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                Console.WriteLine($"User not found for email: {email}");
                return BadRequest(new { success = false, mresponse = "Invalid login attempt" });
            }

            Console.WriteLine($"User found for email: {email}");

            // Attempt to sign in with the provided password
            var result = await _signInManager.PasswordSignInAsync(user, password, false, lockoutOnFailure: true);

            // Log the result of the login attempt
            Console.WriteLine($"PasswordSignInAsync result: RequiresTwoFactor = {result.RequiresTwoFactor}, Succeeded = {result.Succeeded}, IsLockedOut = {result.IsLockedOut}");

            string encodedUserId = null;  // Initialize encodedUserId as string
            string redirectUrl = null;    // Initialize redirectUrl as string

            // Handle users who require two-factor authentication
            if (result.RequiresTwoFactor)
            {
                Console.WriteLine("Two-factor authentication is required.");

                // Check if the `TwoFactorRememberBrowser` cookie is present
                if (await _signInManager.IsTwoFactorClientRememberedAsync(user))
                {
                    // Use RoleRedirectService to handle role-based redirection
                    var roleRedirectResult = await _roleRedirectService.HandleRoleRedirectAsync(email);
                    return roleRedirectResult;  // Return the redirect result
                }

                // Redirect to 2FA if the cookie is not present
                encodedUserId = HashingHelper.EncodeString(user.Id);
                redirectUrl = $"/TwoFA/Index/{encodedUserId}";
                return Ok(new { success = true, mresponse = "Login successful", redirectUrl });
            }

            // Handle users who don't require 2FA (persistent cookie exists)
            else if (result.Succeeded)
            {
                // Use RoleRedirectService to handle role-based redirection
                var roleRedirectResult = await _roleRedirectService.HandleRoleRedirectAsync(email);
                return roleRedirectResult;  // Return the redirect result
            }

            // Handle locked-out accounts
            else if (result.IsLockedOut)
            {
                // Log the lockout event for debugging
                Console.WriteLine($"Account is locked out for email: {email} at {DateTime.Now}");

                // Get the lockout end date if available
                var lockoutEnd = await _userManager.GetLockoutEndDateAsync(user);
                var lockoutMessage = lockoutEnd.HasValue
                    ? $"Account is locked out until {lockoutEnd.Value.LocalDateTime}"
                    : "Account is locked out";

                // Log the lockout end date for debugging
                Console.WriteLine(lockoutMessage);

                return BadRequest(new { success = false, mresponse = lockoutMessage });
            }
            else
            {
                return BadRequest(new { success = false, mresponse = "Invalid login attempt" });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return StatusCode(500, new { success = false, mresponse = "An error occurred" });
        }
    }


}

