using MedisatERP.Data;
using MedisatERP.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedisatERP.Areas.NutritionCompanySystem.Controllers
{
    [Area("NutritionCompanySystem")]
    [Route("NutritionCompanySystem/[controller]/[action]")]
    public class CrmController : Controller
    {
        private readonly UserService _userService;
        private readonly ExceptionHandlerService _exceptionHandlerService;
        private readonly ILogger<CrmController> _logger;
        private readonly ValidateSessionService _validateSessionService;

        public CrmController(UserService userService, ExceptionHandlerService exceptionHandlerService, ILogger<CrmController> logger, ValidateSessionService validateSessionService)
        {
            _userService = userService;
            _exceptionHandlerService = exceptionHandlerService;
            _logger = logger;
            _validateSessionService = validateSessionService;
        }

        public async Task<IActionResult> Patients()
        {
            return await GetDataAsync("Patients");
        }

        public async Task<IActionResult> Appointments()
        {
            return await GetDataAsync("Appointments");
        }

        public async Task<IActionResult> OnlineApplicants()
        {
            return await GetDataAsync("OnlineApplicants");
        }

        public async Task<IActionResult> Communications()
        {
            return await GetDataAsync("Communications");
        }

        public async Task<IActionResult> NutritionCalendar()
        {
            return await GetDataAsync("NutritionCalendar");
        }

        private async Task<IActionResult> GetDataAsync(string viewName)
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
            string sessionCompanyId = HttpContext.Session.GetString("CompanyId");

            Guid? companyId = null;

            try
            {
                var user = await _userService.GetUserAsync(userId);

                if (!string.IsNullOrEmpty(sessionCompanyId) && Guid.TryParse(sessionCompanyId, out Guid parsedCompanyId))
                {
                    companyId = parsedCompanyId;
                }

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
