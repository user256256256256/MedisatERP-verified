using MedisatERP.Data;
using MedisatERP.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MedisatERP.Controllers
{

    public class EmailConfirmationAPIController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ExceptionHandlerService _exceptionHandlerService;
        private readonly ILogger<EmailConfirmationAPIController> _logger;  // Injecting ILogger

        public EmailConfirmationAPIController(UserManager<IdentityUser> userManager,
                                               SignInManager<IdentityUser> signInManager,
                                               ExceptionHandlerService exceptionHandlerService,
                                               ILogger<EmailConfirmationAPIController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _exceptionHandlerService = exceptionHandlerService;  // Injecting the ExceptionHandlerService
            _logger = logger;
        }

        // Action to handle email confirmation
        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Error", "Home", new { message = "Invalid email confirmation request." });
            }

            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    return RedirectToAction("Error", "Home", new { message = "User not found." });
                }

                var result = await _userManager.ConfirmEmailAsync(user, token);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "EmailConfirmation");
                }

                return RedirectToAction("Error", "Home", new { message = "Email confirmation failed." });
            }
            catch (Exception ex)
            {
                // Log the exception using ILogger and delegate handling to the ExceptionHandler service
                _logger.LogError($"An error occurred: {ex.Message}");
                return _exceptionHandlerService.HandleException(ex, this);
            }
        }
    }


}
