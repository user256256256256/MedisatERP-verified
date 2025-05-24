using MedisatERP.Services;
using Microsoft.AspNetCore.Mvc;

namespace MedisatERP.Controllers
{

    public class EmailConfirmationController : Controller
    {
        private readonly ExceptionHandlerService _exceptionHandlerService;
        private readonly ILogger<EmailConfirmationController> _logger;  // Injecting ILogger

        // Inject the ExceptionHandlerService to handle any errors
        public EmailConfirmationController(ExceptionHandlerService exceptionHandlerService, ILogger<EmailConfirmationController> logger)
        {
            _exceptionHandlerService = exceptionHandlerService;
            _logger = logger;
        }

        // Action to display the confirmation page (success or failure)
        public IActionResult Index()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                // Log the exception using ILogger and delegate handling to the ExceptionHandler service
                _logger.LogError($"An error occurred: {ex.Message}");
                return _exceptionHandlerService.HandleException(ex, this);
            }
        }
    }

}
