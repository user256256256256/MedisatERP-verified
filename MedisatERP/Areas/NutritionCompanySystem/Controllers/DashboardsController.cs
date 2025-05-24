using Microsoft.AspNetCore.Mvc;
using MedisatERP.Data;  
using Microsoft.EntityFrameworkCore;
using MedisatERP.Services;

namespace MedisatERP.Areas.NutritionCompanySystem.Controllers
{
    [Area("NutritionCompanySystem")]
    [Route("NutritionCompanySystem/[controller]/[action]")]

    public class DashboardsController : Controller
    {
        private readonly UserService _userService;
        private readonly ExceptionHandlerService _exceptionHandlerService;
        private readonly ILogger<DashboardsController> _logger;
        private readonly ValidateSessionService _validateSessionService;

        public DashboardsController(UserService userService, ExceptionHandlerService exceptionHandlerService, ILogger<DashboardsController> logger, ValidateSessionService validateSessionService)
        {
            _userService = userService;
            _exceptionHandlerService = exceptionHandlerService;
            _logger = logger;
            _validateSessionService = validateSessionService;
        }

        public async Task<IActionResult> Crm()
        {
            return await GetDashboardViewAsync("Crm");
        }

        public async Task<IActionResult> Security()
        {
            return await GetDashboardViewAsync("Security");
        }

        private async Task<IActionResult> GetDashboardViewAsync(string viewName)
        {
            _logger.LogInformation("GetDashboardViewAsync started for viewName: {viewName}", viewName);

            var redirectResultUser = _validateSessionService.ValidateUserSession();
            if (redirectResultUser != null)
            {
                return redirectResultUser;
            }

            var redirectResultCompany = _validateSessionService.ValidateCompanySession();
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



