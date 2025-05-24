using MedisatERP.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;


namespace MedisatERP.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class LoginAPIController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<LoginAPIController> _logger;
        private readonly IValidationService _validationService;
        private readonly IErrorCodeService _errorCodeService;
        private readonly HandelRoleRedirectService _roleRedirectService;
        private readonly IUserSessionService _userSessionService;

        // Constructor to inject dependencies
        public LoginAPIController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IEmailSender emailSender,
            ILogger<LoginAPIController> logger,
            IValidationService validationService,
            IErrorCodeService errorCodeService,
            HandelRoleRedirectService roleRedirectService,
            IUserSessionService userSessionService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _logger = logger;
            _validationService = validationService;
            _errorCodeService = errorCodeService;
            _roleRedirectService = roleRedirectService;
            _userSessionService = userSessionService;
        }

        public async Task<ActionResult> LoginCheck(string identifier, string password)
        {
            if (string.IsNullOrEmpty(identifier) || string.IsNullOrEmpty(password))
            {
                return HandleError("MISSING_CREDENTIALS", identifier);
            }

            // First, try to find by username
            var user = await _userManager.FindByNameAsync(identifier);

            if (user == null)
            {
                // If no user found by username, try finding by email
                user = await _userManager.FindByEmailAsync(identifier);
            }

            if (user == null)
            {
                return HandleError("USER_NOT_FOUND", identifier);
            }

            // Check if the user's email is confirmed (if you're using email verification)
            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                await SendConfirmationEmail(user);
                return HandleError("EMAIL_NOT_CONFIRMED", identifier);
            }

            // Attempt sign-in with password
            var result = await _signInManager.PasswordSignInAsync(user, password, false, lockoutOnFailure: true);

            // Handle 2FA and login success
            if (result.RequiresTwoFactor)
            {
                return Handle2FARedirection(user);
            }

            if (result.Succeeded)
            {
                return await HandleRoleRedirect(user, identifier);
            }

            if (result.IsLockedOut)
            {
                return await HandleLockout(user, identifier);
            }

            return HandleError("INVALID_PASSWORD", identifier); // Failed login due to wrong credentials
        }

        private ActionResult HandleError(string errorCode, string email)
        {
            var errorDetails = _errorCodeService.GetErrorDetails(errorCode);
            _logger.LogWarning($"Login attempt failed: {errorDetails.ErrorMessage} for email: {email}");
            return Json(new { success = false, mresponse = errorDetails.ErrorMessage });
        }

        private async Task SendConfirmationEmail(IdentityUser user)
        {
            // Generate email confirmation token and the confirmation link
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = Url.Action("ConfirmEmail", "EmailConfirmationAPI", new { token, email = user.Email }, Request.Scheme);
            Console.WriteLine(confirmationLink);

            // Send email with a user-friendly message including "click here"
            string emailBody = $"Please confirm your email by <a href='{confirmationLink}'>clicking here</a>.";

            await _emailSender.SendEmailAsync(user.Email, "Confirm your email", emailBody);
            _logger.LogInformation($"Confirmation email sent to: {user.Email}");
        }

        // Handle Two Factor Authentication Redirection
        private ActionResult Handle2FARedirection(IdentityUser user)
        {
            _logger.LogInformation("Two-factor authentication required.");

            // Store user ID in session for 2FA
            _userSessionService.SetSessionData(user.Id, null);

            var redirectUrl = $"/TwoFA/Index";
            return Json(new { success = true, mresponse = "Login successful, redirecting to 2FA.", redirectUrl });
        }

        private async Task<ActionResult> HandleRoleRedirect(IdentityUser user, string email)
        {
            var roleRedirectResult = await _roleRedirectService.HandleRoleRedirectAsync(email);
            return roleRedirectResult;
        }

        private async Task<ActionResult> HandleLockout(IdentityUser user, string email)
        {
            var lockoutEnd = await _userManager.GetLockoutEndDateAsync(user);
            var lockoutMessage = lockoutEnd.HasValue ? $"Account locked out until {lockoutEnd.Value.LocalDateTime}" : "Account is locked out due to multiple login attempts. If this wasn't you ignore this message.";
            _logger.LogWarning($"Account is locked out for email: {email}. {lockoutMessage}.");
            return Json(new { success = false, mresponse = _errorCodeService.GetErrorDetails("ACCOUNT_LOCKED").ErrorMessage });
        }

        private ActionResult HandleDatabaseError(SqlException ex)
        {
            _logger.LogError(ex, "Database error during login attempt");
            var errorDetails = _errorCodeService.GetErrorDetails("DB_CONNECTION_FAILED");
            return RedirectToAction("Error", "Home", new { message = errorDetails.ErrorMessage });
        }

        private ActionResult HandleUnknownError(Exception ex)
        {
            _logger.LogError(ex, "An error occurred during login attempt");
            var errorDetails = _errorCodeService.GetErrorDetails("UNKNOWN_ERROR");
            return RedirectToAction("Error", "Home", new { message = errorDetails.ErrorMessage });
        }
    }
}

