using Microsoft.AspNetCore.Mvc;
using MedisatERP.Data;  
using MedisatERP.Services;  
using Microsoft.EntityFrameworkCore;

namespace MedisatERP.Areas.AdministratorSystem.Controllers
{
    [Area("AdministratorSystem")]
    [Route("AdministratorSystem/[controller]/[action]")]
    public class CompanyClientsController : Controller
    {
        private readonly UserService _userService;
        private readonly ExceptionHandlerService _exceptionHandlerService;
        private readonly ILogger<CompanyClientsController> _logger;
        private readonly ValidateSessionService _validateSessionService;
        public CompanyClientsController(UserService userService, ExceptionHandlerService exceptionHandlerService, ILogger<CompanyClientsController> logger, ValidateSessionService validateSessionService)
        {
            _userService = userService;
            _exceptionHandlerService = exceptionHandlerService;
            _logger = logger;
            _validateSessionService = validateSessionService;
        }

        public async Task<IActionResult> Index()
        {
            // Validate session and check if user ID exists
            var redirectResult = _validateSessionService.ValidateUserSession();
            if (redirectResult != null)
            {
                return redirectResult;  // If user ID is missing, redirect to Home
            }

            // Retrieve user ID from session (if valid session)
            string userId = HttpContext.Session.GetString("UserId");

            try
            {
                // Retrieve user details using the UserService
                var user = await _userService.GetUserAsync(userId);

                // Return the view with the user model if everything is successful
                return View(user);
            }
            catch (Exception ex)
            {
                // Delegate to ExceptionHandlerService for proper exception handling
                return _exceptionHandlerService.HandleException(ex, this);
            }
        }
    }

}
