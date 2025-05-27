using Microsoft.AspNetCore.Mvc;
using MedisatERP.Data;  
using MedisatERP.Services;  // Assuming you have HashingHelper class
using Microsoft.EntityFrameworkCore;

namespace MedisatERP.Areas.NutritionCompanySystem.Controllers
{
    [Area("NutritionCompanySystem")]
    [Route("NutritionCompanySystem/[controller]/[action]")]
    public class UserManagementController : Controller
    {
        private readonly UserService _userService;
        private readonly ExceptionHandlerService _exceptionHandlerService;
        private readonly ILogger<UserManagementController> _logger;
        private readonly ValidateSessionService _validateSessionService;

        public UserManagementController(UserService userService, ExceptionHandlerService exceptionHandlerService, ILogger<UserManagementController> logger, ValidateSessionService validateSessionService)
        {
            _userService = userService;
            _exceptionHandlerService = exceptionHandlerService;
            _logger = logger;
            _validateSessionService = validateSessionService;  // Inject the helper
        }

        // UserProfile action using the helper method
        public Task<IActionResult> UserProfile()
        {
            return GetUserProfileViewAsync("UserProfile");
        }

        // UserSettings action using the helper method
        public Task<IActionResult> UserSettings()
        {
            return GetUserProfileViewAsync("UserSettings");
        }

        // Notifications action using the helper method
        public Task<IActionResult> Notifications()
        {
            return GetUserProfileViewAsync("Notifications");
        }

        // Help action using the helper method
        public Task<IActionResult> Help()
        {
            return GetUserProfileViewAsync("Help");
        }

        // Helper method to check user session and fetch user data
        private async Task<IActionResult> GetUserProfileViewAsync(string viewName)
        {
            var redirectResult = _validateSessionService.ValidateUserSession();

            if (redirectResult != null)
            {
                return redirectResult; // Redirect if session is invalid
            }

            string userId = HttpContext.Session.GetString("UserId");

            try
            {
                var user = await _userService.GetUserAsync(userId);
                return View(viewName, user); // Return the appropriate view with the user data
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching user data.");
                return _exceptionHandlerService.HandleException(ex, this); // Handle exception properly
            }
        }
    }

}
