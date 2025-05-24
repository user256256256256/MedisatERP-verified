using Microsoft.AspNetCore.Mvc;
using MedisatERP.Data;  
using MedisatERP.Services;  
using Microsoft.EntityFrameworkCore;

namespace MedisatERP.Areas.NutritionCompanySystem.Controllers
{
    [Area("NutritionCompanySystem")]
    [Route("NutritionCompanySystem/[controller]/[action]")]
    public class SecurityController : Controller
    {
        private readonly UserService _userService;
        private readonly ExceptionHandlerService _exceptionHandlerService;
        private readonly ILogger<SecurityController> _logger;
        private readonly ValidateSessionService _validateSessionService;

        public SecurityController(UserService userService, ExceptionHandlerService exceptionHandlerService, ILogger<SecurityController> logger, ValidateSessionService validateSessionService)
        {
            _userService = userService;
            _exceptionHandlerService = exceptionHandlerService;
            _logger = logger;
            _validateSessionService = validateSessionService;
        }

        public async Task<IActionResult> UserAccounts()
        {
            return await GetViewAsync("UserAccounts");
        }

        public async Task<IActionResult> AuditLogs()
        {
            return await GetViewAsync("AuditLogs");
        }

        public async Task<IActionResult> Feedbacks()
        {
            return await GetViewAsync("Feedbacks");
        }

        public async Task<IActionResult> UserSettings()
        {
            return await GetViewAsync("UserSettings");
        }

        public async Task<IActionResult> UserNotifications()
        {
            return await GetViewAsync("UserNotifications");
        }

        public async Task<IActionResult> UserProfile()
        {
            return await GetViewAsync("UserProfile");
        }

        private async Task<IActionResult> GetViewAsync(string viewName)
        {
            var redirectResultUser = _validateSessionService.ValidateUserSession();
            var redirectResultCompany = _validateSessionService.ValidateCompanySession();

            if (redirectResultUser != null)
            {
                return redirectResultUser;
            }

            if (redirectResultCompany != null)
            {
                return redirectResultCompany;
            }

            string userId = HttpContext.Session.GetString("UserId");
            string companyId = HttpContext.Session.GetString("CompanyId");

            try
            {
                var user = await _userService.GetUserAsync(userId);


                ViewData["CompanyId"] = companyId;
                return View(viewName, user);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching user or company data for UserId: {userId}, CompanyId: {companyId}", userId, companyId);
                return _exceptionHandlerService.HandleException(ex, this);
            }
        }
    }

}
