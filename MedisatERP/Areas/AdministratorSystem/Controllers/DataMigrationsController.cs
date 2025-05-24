using Microsoft.AspNetCore.Mvc;
using MedisatERP.Data;  // Assuming you have MedisatErpDbContext
using MedisatERP.Services;  // Assuming you have HashingHelper class
using Microsoft.EntityFrameworkCore;

namespace MedisatERP.Areas.AdministratorSystem.Controllers
{
    [Area("AdministratorSystem")]
    [Route("AdministratorSystem/[controller]/[action]")]
    public class DataMigrationsController : Controller
    {
        private readonly UserService _userService;
        private readonly ExceptionHandlerService _exceptionHandlerService;
        private readonly ILogger<DataMigrationsController> _logger;
        private readonly ValidateSessionService _validateSessionService;

        public DataMigrationsController(UserService userService, ExceptionHandlerService exceptionHandlerService, ILogger<DataMigrationsController> logger, ValidateSessionService validateSessionService)
        {
            _userService = userService;
            _exceptionHandlerService = exceptionHandlerService;
            _logger = logger;
            _validateSessionService = validateSessionService;
        }

        public async Task<IActionResult> Index()
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

                return View(user);
            }
            catch (Exception ex)
            {
                return _exceptionHandlerService.HandleException(ex, this);
            }
        }
    }

}
