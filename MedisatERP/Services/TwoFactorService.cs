using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace MedisatERP.Services
{

    public class TwoFactorService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly IValidationService _validationService;
        private readonly IErrorCodeService _errorCodeService;
        private readonly HandelRoleRedirectService _roleRedirectService;
        private readonly ILogger<TwoFactorService> _logger;

        public TwoFactorService(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IEmailSender emailSender,
            IValidationService validationService,
            IErrorCodeService errorCodeService,
            HandelRoleRedirectService roleRedirectService,
            ILogger<TwoFactorService> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _validationService = validationService;
            _errorCodeService = errorCodeService;
            _roleRedirectService = roleRedirectService;
            _logger = logger;
        }

        public async Task<ActionResult> SendCodeAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return HandleError("USER_NOT_FOUND", userId);

            var twoFactorEnabled = await _userManager.GetTwoFactorEnabledAsync(user);
            if (!twoFactorEnabled)
                return HandleError("TWO_FACTOR_NOT_ENABLED", userId);

            var providers = await _userManager.GetValidTwoFactorProvidersAsync(user);
            if (!providers.Contains("Email"))
                return HandleError("NO_VALID_2FA_PROVIDERS", userId);

            string code = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");

            if (string.IsNullOrEmpty(user.Email) || !_validationService.IsValidEmailFormat(user.Email).IsValid)
                return HandleError("INVALID_EMAIL_ADDRESS", userId);

            try
            {
                await _emailSender.SendEmailAsync(user.Email, "Your Security Code", $"Your security code is: {code}");
                Console.WriteLine("Code is " + code);

                return new JsonResult(new { success = true, mresponse = "Code sent successfully to your email. Expiring in 5 minutes." });
            }
            catch (Exception ex)
            {
                // Log the failure and inform the user
                _logger.LogError(ex, "Error sending 2FA email");

                // Return a specific error
                return new JsonResult(new { success = false, mresponse = "Failed to send the 2FA code. Check your network connection." });
            }
        }


        public async Task<ActionResult> VerifyCodeAsync(string userId, string provider, string code, bool rememberMe)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return HandleError("USER_NOT_FOUND", userId);

            var providers = await _userManager.GetValidTwoFactorProvidersAsync(user);
            string normalizedProvider = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(provider.ToLower());

            if (!providers.Contains(normalizedProvider))
            {
                _logger.LogWarning($"Invalid 2FA provider. Valid providers: {string.Join(", ", providers)}");
                return HandleError("INVALID_2FA_PROVIDER", userId);
            }

            var result = await _signInManager.TwoFactorSignInAsync(normalizedProvider, code, rememberMe, rememberMe);

            if (result.Succeeded)
            {
                var roleRedirectResult = await _roleRedirectService.HandleRoleRedirectAsync(user.Email);
                return roleRedirectResult;
            }
            else if (result.IsLockedOut)
            {
                return HandleError("USER_LOCKED_OUT", userId);
            }
            else if (result.IsNotAllowed)
            {
                return HandleError("SIGN_IN_NOT_ALLOWED", userId);
            }
            else
            {
                return HandleError("AUTHENTICATION_FAILED", userId);
            }
        }

        private ActionResult HandleError(string errorCode, string userId)
        {
            var errorDetails = _errorCodeService.GetErrorDetails(errorCode);
            _logger.LogWarning($"Error for UserId: {userId}, Error: {errorDetails.ErrorMessage}");
            return new JsonResult(new { success = false, mresponse = errorDetails.ErrorMessage });
        }
    }

}
