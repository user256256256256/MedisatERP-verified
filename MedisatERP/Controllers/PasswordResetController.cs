using MedisatERP.Services;
using Microsoft.AspNetCore.Mvc;
using NuGet.Common;

namespace MedisatERP.Controllers
{
    public class PasswordResetController : Controller
    {
        private readonly IErrorCodeService _errorCodeService;
        private readonly ExceptionHandlerService _exceptionHandlerService;
        private readonly ILogger<PasswordResetController> _logger;  // Injecting ILogger

        public PasswordResetController(
            IErrorCodeService errorCodeService,
            ExceptionHandlerService exceptionHandlerService,
            ILogger<PasswordResetController> logger)
        {
            _errorCodeService = errorCodeService;
            _exceptionHandlerService = exceptionHandlerService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ResetPassword()
        {
            try
            {
                // Read from TempData
                var token = TempData["Token"] as string;
                var email = TempData["Email"] as string;

                // Check if TempData has the expected values
                if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
                {
                    var invalidRequestDetails = _errorCodeService.GetErrorDetails("INVALID_REQUEST");
                    // Redirect to error page if the request is invalid
                    return RedirectToAction("Error", "Home", new { message = invalidRequestDetails.ErrorMessage });
                }

                // Pass the token and email to the view via ViewData
                ViewData["Token"] = token;
                ViewData["Email"] = email;

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred: {ex.Message}");
                return _exceptionHandlerService.HandleException(ex, this);
            }
        }
    }

}
