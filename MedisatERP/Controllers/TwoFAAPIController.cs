using MedisatERP.Areas.CoreSystem.Models;
using MedisatERP.Data;
using MedisatERP.Library;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace MedisatERP.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class TwoFAAPIController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<TwoFAAPIController> _logger;
        private readonly MedisatErpDbContext _dbContext;
        private readonly RoleRedirectService _roleRedirectService;

        public TwoFAAPIController(UserManager<IdentityUser> userManager,
                        SignInManager<IdentityUser> signInManager,
                        IEmailSender emailSender,
                        ILogger<TwoFAAPIController> logger, MedisatErpDbContext dbContext, RoleRedirectService roleRedirectService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _dbContext = dbContext;
            _logger = logger;
            _roleRedirectService = roleRedirectService;
        }

        // Action to send 2FA code via GET request
        [HttpGet]
        public async Task<ActionResult> SendCode(string userId, string method)
        {
            Console.WriteLine("Entered SendCode method");
            Console.WriteLine($"User ID: {userId}, Method: {method}");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                Console.WriteLine("User not found");
                return BadRequest(new { success = false, mresponse = "User not found" });
            }

            var twoFactorEnabled = await _userManager.GetTwoFactorEnabledAsync(user);
            if (!twoFactorEnabled)
            {
                Console.WriteLine("Two-factor authentication is not enabled for this user.");
                return BadRequest(new { success = false, mresponse = "Two-factor authentication is not enabled for this user." });
            }

            var providers = await _userManager.GetValidTwoFactorProvidersAsync(user);
            if (!providers.Contains("Email"))
            {
                Console.WriteLine("No valid 2FA providers available");
                return BadRequest(new { success = false, mresponse = "No valid 2FA providers available" });
            }

            string code = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");
            Console.WriteLine($"Generated 2FA code: {code}");

            if (string.IsNullOrEmpty(user.Email) || !IsValidEmail(user.Email))
            {
                Console.WriteLine($"Invalid email address: {user.Email}");
                return BadRequest(new { success = false, mresponse = "Invalid email address" });
            }

            await _emailSender.SendEmailAsync(user.Email, "Your Security Code", $"Your security code is: {code}");
            Console.WriteLine($"Email sent to: {user.Email}");

            return Ok(new { success = true, mresponse = "Code sent successfully to your email. Expiring in 5 minutes." });
        }


        // Helper method to validate email address format
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        [HttpGet]
        public async Task<ActionResult> VerifyCode(string userId, string provider, string code, bool rememberMe)
        {
            // If remember me is checked, remember browser is set to true
            var rememberBrowser = rememberMe;

            Console.WriteLine("Entered VerifyCode method");
            Console.WriteLine($"UserId: {userId}, Provider: {provider}, Code: {code}");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                Console.WriteLine("User not found");
                return BadRequest(new { success = false, mresponse = "User not found" });
            }

            var providers = await _userManager.GetValidTwoFactorProvidersAsync(user);
            Console.WriteLine($"Valid 2FA providers: {string.Join(", ", providers)}");

            string normalizedProvider = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(provider.ToLower());
            Console.WriteLine($"Normalized Provider: {normalizedProvider}");

            if (!providers.Contains(normalizedProvider))
            {
                Console.WriteLine($"Invalid 2FA provider. Valid providers: {string.Join(", ", providers)}");
                return BadRequest(new { success = false, mresponse = "Invalid 2FA provider" });
            }

            var result = await _signInManager.TwoFactorSignInAsync(normalizedProvider, code, rememberMe, rememberBrowser);

            if (result.Succeeded)
            {
                // Use RoleRedirectService to handle role-based redirection
                var roleRedirectResult = await _roleRedirectService.HandleRoleRedirectAsync(user.Email);
                return roleRedirectResult;  // Return the redirect result
            }
            else if (result.IsLockedOut)
            {
                Console.WriteLine("User is locked out");
                return BadRequest(new { success = false, mresponse = "User is locked out" });
            }
            else if (result.IsNotAllowed)
            {
                Console.WriteLine("Sign-in not allowed");
                return BadRequest(new { success = false, mresponse = "Sign-in not allowed" });
            }
            else
            {
                Console.WriteLine("Authentication failed due to other reasons.");
                return BadRequest(new { success = false, mresponse = "Authentication failed" });
            }
        }
    }
}



