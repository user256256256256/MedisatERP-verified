using MedisatERP.Services;
using Microsoft.AspNetCore.Mvc;

namespace MedisatERP.Areas.AdministratorSystem.Controllers
{
    [Area("AdministratorSystem")]
    [Route("AdministratorSystem/[controller]/[action]")]
    public class DashboardsController : Controller
    {
        private readonly UserService _userService;
        private readonly ExceptionHandlerService _exceptionHandlerService;
        private readonly ILogger<DashboardsController> _logger;
        private readonly ValidateSessionService _validateSessionService;

        public DashboardsController(
            UserService userService,
            ExceptionHandlerService exceptionHandlerService,
            ILogger<DashboardsController> logger,
            ValidateSessionService validateSessionService)
        {
            _userService = userService;
            _exceptionHandlerService = exceptionHandlerService;
            _logger = logger;
            _validateSessionService = validateSessionService;
        }

        // Crm action using the helper method
        public Task<IActionResult> Crm()
        {
            return GetDashboardViewAsync("Crm");
        }

        // Finance action using the helper method
        public Task<IActionResult> Subscriptions()
        {
            return GetDashboardViewAsync("Subscriptions");
        }

        // Security action using the helper method
        public Task<IActionResult> Security()
        {
            return GetDashboardViewAsync("Security");
        }

        // Helper method to validate session and fetch user data
        private async Task<IActionResult> GetDashboardViewAsync(string viewName)
        {
            var redirectResult = _validateSessionService.ValidateUserSession();

            if (redirectResult != null)
            {
                return redirectResult; // If session is invalid, return the redirect result
            }

            string userId = HttpContext.Session.GetString("UserId");

            try
            {
                var user = await _userService.GetUserAsync(userId);
                return View(viewName, user); // Return the view with the user model
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user details for dashboard.");
                return _exceptionHandlerService.HandleException(ex, this); // Delegate exception handling
            }
        }
    }

}
