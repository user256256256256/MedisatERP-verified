using MedisatERP.Services;
using Microsoft.AspNetCore.Mvc;

namespace MedisatERP.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ExceptionHandlerService _exceptionHandlerService;

        public HomeController(ILogger<HomeController> logger, ExceptionHandlerService exceptionHandlerService)
        {
            _logger = logger;
            _exceptionHandlerService = exceptionHandlerService;
        }

        public IActionResult Index()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                return _exceptionHandlerService.HandleException(ex, this);
            }
        }

        public IActionResult PrivacyPolicies()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                return _exceptionHandlerService.HandleException(ex, this);
            }
        }

        public IActionResult Error(string message)
        {
            try
            {
                // Log the error message
                _logger.LogError($"Error occurred: {message}");

                // Set the error message for display in the view
                ViewData["ErrorMessage"] = message;

                // Return the Error view
                return View();
            }
            catch (Exception ex)
            {
                // In case of failure in error handling itself, log this secondary error
                _logger.LogError(ex, "An error occurred while displaying the error page.");
                return View("Error"); // A generic fallback view
            }
        }
    }

}
