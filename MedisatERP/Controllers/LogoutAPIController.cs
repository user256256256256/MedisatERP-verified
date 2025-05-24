using MedisatERP.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MedisatERP.Controllers
{

    [ApiController]
    [Route("api/[controller]/[action]")]
    public class LogoutAPIController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IErrorCodeService _errorCodeService;
        private readonly ILogger<LogoutAPIController> _logger;

        public LogoutAPIController(
            SignInManager<IdentityUser> signInManager,
            IErrorCodeService errorCodeService,
            ILogger<LogoutAPIController> logger)
        {
            _signInManager = signInManager;
            _errorCodeService = errorCodeService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> LogoutCheck([FromBody] string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return HandleError("INVALID_USER_ID", "userId is null or empty");
            }

            var decodedUserId = DecodeUserId(userId);
            if (decodedUserId == null)
            {
                return HandleError("INVALID_USER_ID", "Decoded userId is null");
            }

            _logger.LogInformation("Decoded userId: {UserId} successfully retrieved", decodedUserId);

            try
            {
                await _signInManager.SignOutAsync();
                _logger.LogInformation("Logout successful for userId: {UserId}", decodedUserId);
                return Json(new { success = true, message = "Logout successful.", redirectUrl = "/" });
            }
            catch (Exception ex)
            {
                return HandleError("LOGOUT_FAILED", "Logout failed due to an exception", ex);
            }
        }

        private string DecodeUserId(string userId)
        {
            try
            {
                // Optionally decode the userId if required
                return TranscodingService.DecodeString(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during userId decoding");
                return null;
            }
        }

        private IActionResult HandleError(string errorCode, string logMessage, Exception exception = null)
        {
            var errorDetails = _errorCodeService.GetErrorDetails(errorCode);
            _logger.LogWarning(exception, logMessage);
            return Json(new { success = false, message = errorDetails.ErrorMessage });
        }
    }


}
