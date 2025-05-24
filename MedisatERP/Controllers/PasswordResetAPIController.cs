using MedisatERP.Data;
using MedisatERP.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace MedisatERP.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class PasswordResetAPIController : Controller
    {
        private readonly IValidationService _validationService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IErrorCodeService _errorCodeService;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<PasswordResetAPIController> _logger;
        private readonly ExceptionHandlerService _exceptionHandlerService;

        public PasswordResetAPIController(
            IValidationService validationService,
            UserManager<IdentityUser> userManager,
            IErrorCodeService errorCodeService,
            IEmailSender emailSender,
            ILogger<PasswordResetAPIController> logger,
            ExceptionHandlerService exceptionHandlerService)
        {
            _validationService = validationService;
            _userManager = userManager;
            _errorCodeService = errorCodeService;
            _emailSender = emailSender;
            _logger = logger;
            _exceptionHandlerService = exceptionHandlerService;
        }

        [HttpGet]
        public async Task<IActionResult> SendRecoveryToken(string email)
        {
            try
            {
                // Validate the email format first
                if (!ValidateEmailFormatAsync(email))
                {
                    return HandleError("INVALID_EMAIL", email);
                }

                // Check if the user exists
                var user = await GetUserByEmailAsync(email);
                if (user == null)
                {
                    return HandleError("USER_NOT_FOUND", email);
                }

                // Generate password reset token
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = GenerateRecoveryCallbackUrl(user.Email, token);
                Console.WriteLine(callbackUrl);

                // Send recovery email
                if (!await SendRecoveryEmailAsync(email, callbackUrl))
                {
                    // Return a specific error
                    return new JsonResult(new { success = false, mresponse = "Failed to send the recovery token. Check your network connection." });
                }

                return Json(new { success = true, mresponse = "Token sent successfully." });
            }
            catch (Exception ex)
            {
                return HandleException(ex, "Error sending recovery token.");
            }
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmPasswordRecoveryToken(string token, string email)
        {
            try
            {
                // Validate the input parameters
                if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
                {
                    return HandleError("INVALID_REQUEST", email);
                }

                // Retrieve the user by email
                var user = await GetUserByEmailAsync(email);
                if (user == null)
                {
                    return HandleError("USER_NOT_FOUND", email);
                }

                // Store the token and email for the next step
                TempData["Token"] = token;
                TempData["Email"] = email;

                // Redirect to password reset page
                return RedirectToAction("ResetPassword", "PasswordReset");
            }
            catch (Exception ex)
            {
                return HandleException(ex, "Error confirming recovery token.");
            }
        }

        [HttpGet]
        public async Task<IActionResult> ResetPassword(string token, string email, string newPassword)
        {
            try
            {
                // Validate the input parameters
                if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
                {
                    return HandleError("INVALID_REQUEST", email);
                }

                // Validate strong password
                if (!ValidatePasswordStrength(newPassword))
                {
                    return HandleError("WEAK_PASSWORD", newPassword);
                }

                // Retrieve the user by email
                var user = await GetUserByEmailAsync(email);
                if (user == null)
                {
                    return HandleError("USER_NOT_FOUND", email);
                }

                // Attempt to reset the password
                var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

                if (result.Succeeded)
                {
                    return Json(new { success = true, mresponse = "Password updated successfully." });
                }

                return HandleError("PASSWORD_RESET_FAILED", email);
            }
            catch (Exception ex)
            {
                return HandleException(ex, "Error resetting password.");
            }
        }

        // Methods 
        private bool ValidateEmailFormatAsync(string email)
        {
            var validationResult = _validationService.IsValidEmailFormat(email);

            if (!validationResult.IsValid)
            {
                _logger.LogWarning($"Invalid email format: {email}. Error Code: {validationResult.ErrorCode}");
                return false;
            }

            return true;
        }

        private bool ValidatePasswordStrength(string password)
        {
            var validationResult = _validationService.IsStrongPassword(password);

            if (!validationResult.IsValid)
            {
                _logger.LogWarning($"Weak password: {password}. Error Code: {validationResult.ErrorCode}");
                return false;
            }

            return true;
        }

        private async Task<IdentityUser> GetUserByEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                _logger.LogWarning($"User not found: {email}");
            }
            return user;
        }

        private string GenerateRecoveryCallbackUrl(string email, string token)
        {
            return Url.Action("ConfirmPasswordRecoveryToken", "PasswordResetAPI", new { token, email }, protocol: Request.Scheme);
        }

        private async Task<bool> SendRecoveryEmailAsync(string email, string callbackUrl)
        {
            try
            {
                await _emailSender.SendEmailAsync(email, "Reset Password", $"Please reset your password by <a href='{callbackUrl}'>clicking here</a>.");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending email to {email}: {ex.Message}");
                return false;
            }
        }

        private IActionResult HandleError(string errorCode, string identifier)
        {
            var errorDetails = _errorCodeService.GetErrorDetails(errorCode);
            return Json(new { success = false, mresponse = errorDetails.ErrorMessage });
        }

        private IActionResult HandleException(Exception ex, string message)
        {
            _logger.LogError($"{message}: {ex.Message}");
            return _exceptionHandlerService.HandleException(ex, this);
        }

    }

}
