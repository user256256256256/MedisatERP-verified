using Microsoft.AspNetCore.Mvc;
using MedisatERP.Data;  
using MedisatERP.Services;  
using Microsoft.EntityFrameworkCore;

namespace MedisatERP.Areas.AdministratorSystem.Controllers
{
    [Area("AdministratorSystem")]
    [Route("AdministratorSystem/[controller]/[action]")]
    public class FeedbacksController : Controller
    {
        private readonly UserService _userService;
        private readonly ExceptionHandlerService _exceptionHandlerService;
        private readonly ILogger<FeedbacksController> _logger;
        private readonly ValidateSessionService _validateSessionService;

        public FeedbacksController(UserService userService, ExceptionHandlerService exceptionHandlerService, ILogger<FeedbacksController> logger, ValidateSessionService validateSessionService)
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
