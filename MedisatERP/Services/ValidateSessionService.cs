using Microsoft.AspNetCore.Mvc;

namespace MedisatERP.Services
{

    public class ValidateSessionService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ValidateSessionService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public IActionResult ValidateUserSession()
        {
            // Retrieve user ID from session
            string userId = _httpContextAccessor.HttpContext.Session.GetString("UserId");

            // Check if user ID is null or empty, and handle accordingly
            if (string.IsNullOrEmpty(userId))
            {
                // Redirect to Home page if user ID is not found in session
                return new RedirectToActionResult("Index", "Home", null);
            }

            // Return null if everything is fine (i.e., userId exists)
            return null;
        }

        public IActionResult ValidateCompanySession()
        {
            string companyId = _httpContextAccessor.HttpContext.Session.GetString("CompanyId");
            
            if (string.IsNullOrEmpty(companyId)) 
            {
                return new RedirectToActionResult("Index", "Home", null);
            }

            return null;
        }

    }

}
