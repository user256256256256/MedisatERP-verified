using MedisatERP.Data;
using MedisatERP.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedisatERP.Areas.AdministratorSystem.Controllers
{
    [Area("AdministratorSystem")]
    [Route("AdministratorSystem/[controller]/[action]")]
    public class SubscriptionsController : Controller
    {
        private readonly UserService _userService;
        private readonly ExceptionHandlerService _exceptionHandlerService;
        private readonly ILogger<SubscriptionsController> _logger;
        private readonly ValidateSessionService _validateSessionService;

        public SubscriptionsController(UserService userService, ExceptionHandlerService exceptionHandlerService, ILogger<SubscriptionsController> logger, ValidateSessionService validateSessionService)
        {
            _userService = userService;
            _exceptionHandlerService = exceptionHandlerService;
            _logger = logger;
            _validateSessionService = validateSessionService;
        }

        public Task<IActionResult> Subscriptions()
        {
            return GetSubscriptionViewAsync("Subscriptions");
        }

        public Task<IActionResult> Payments()
        {
            return GetSubscriptionViewAsync("Payments");
        }

        public Task<IActionResult> SubscriptionLogs()
        {
            return GetSubscriptionViewAsync("SubscriptionLogs");
        }

        public Task<IActionResult> Trials()
        {
            return GetSubscriptionViewAsync("Trials");
        }

        private async Task<IActionResult> GetSubscriptionViewAsync(string viewName)
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
                _logger.LogError(ex, "Error retrieving user details for subscription");
                return _exceptionHandlerService.HandleException(ex, this);
            }
        }
    }

}
