using Microsoft.AspNetCore.Mvc;
using MedisatERP.Data;  // Assuming you have MedisatErpDbContext
using MedisatERP.Services;  // Assuming you have HashingHelper class
using Microsoft.EntityFrameworkCore;

namespace MedisatERP.Areas.AdministratorSystem.Controllers
{
    [Area("AdministratorSystem")]
    [Route("AdministratorSystem/[controller]/[action]")]
    public class UserAccountsController : Controller
    {
        private readonly UserService _userService;
        private readonly ExceptionHandlerService _exceptionHandlerService;
        private readonly ILogger<UserAccountsController> _logger;
        private readonly ValidateSessionService _validateSessionService;

        public UserAccountsController(UserService userService, ExceptionHandlerService exceptionHandlerService, ILogger<UserAccountsController> logger, ValidateSessionService validateSessionService)
        {
            _userService = userService;
            _exceptionHandlerService = exceptionHandlerService;
            _logger = logger;
            _validateSessionService = validateSessionService;
        }

        public Task<IActionResult> UserAccounts()
        {
            return GetUserAccountAsync("UserAccounts");
        }

        public Task<IActionResult> Rbac()
        {
            return GetUserAccountAsync("Rbac");
        }

        public Task<IActionResult> Claims()
        {
            return GetUserAccountAsync("Claims");
        }

        private async Task<IActionResult> GetUserAccountAsync(string viewName)
        {
            var redirectResult = _validateSessionService.ValidateUserSession();

            if (redirectResult != null)
            {
                return redirectResult; 
            }

            string userId = HttpContext.Session.GetString("UserId");

            try
            {
                var user = await _userService.GetUserAsync(userId);
                return View(viewName, user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching user data.");
                return _exceptionHandlerService.HandleException(ex, this);
            }
        }
    }

}
