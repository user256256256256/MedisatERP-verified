using MedisatERP.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MedisatERP.Services
{

    public class HandelRoleRedirectService : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IErrorCodeService _errorCodeService;
        private readonly ILogger<HandelRoleRedirectService> _logger;
        private readonly IUserSessionService _userSessionService;
        private readonly IRoleRedirectService _roleRedirectService;

        public HandelRoleRedirectService(
            UserManager<IdentityUser> userManager,
            IErrorCodeService errorCodeService,
            ILogger<HandelRoleRedirectService> logger,
            IUserSessionService userSessionService,
            IRoleRedirectService roleRedirectService)
        {
            _userManager = userManager;
            _errorCodeService = errorCodeService;
            _logger = logger;
            _userSessionService = userSessionService;
            _roleRedirectService = roleRedirectService;
        }

        public async Task<ActionResult> HandleRoleRedirectAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                _logger.LogWarning("User not found for email: {Email}", email);
                return BadRequest(new { success = false, mresponse = "Invalid user data" });
            }

            var roles = await _userManager.GetRolesAsync(user);
            _logger.LogInformation("Roles for user {Email}: {Roles}", email, string.Join(", ", roles));

            var aspNetUser = await _userSessionService.GetUserSessionAsync(email);
            if (aspNetUser == null)
            {
                _logger.LogWarning("No custom AspNetUser found for email: {Email}", email);
                return BadRequest(new { success = false, mresponse = "Invalid user data" });
            }

            // Store user ID and company ID in session
            _userSessionService.SetSessionData(aspNetUser.Id, aspNetUser.CompanyId);

            // Get the appropriate redirect URL
            var redirectUrl = _roleRedirectService.GenerateRedirectUrl(roles);

            if (!string.IsNullOrEmpty(redirectUrl))
            {
                return Ok(new { success = true, mresponse = "Login successful", redirectUrl });
            }

            return Ok(new { success = true, mresponse = "Login successful" });
        }

        public async Task<string> HandleAdminToCompSystemAsync(string email, Guid companyId)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    _logger.LogWarning("User not found for email: {Email}", email);
                    return null;
                }

                var roles = await _userManager.GetRolesAsync(user);
                _logger.LogInformation("Roles for user {Email}: {Roles}", email, string.Join(", ", roles));

                var aspNetUser = await _userSessionService.GetUserSessionAsync(email);
                if (aspNetUser == null)
                {
                    _logger.LogWarning("No custom AspNetUser found for email: {Email}", email);
                    return null;
                }

                _userSessionService.SetSessionData(aspNetUser.Id, aspNetUser.CompanyId);

                var redirectUrl = _roleRedirectService.GenerateAdminToCompRedirectUrl(roles);

                if (!string.IsNullOrEmpty(redirectUrl)) 
                {
                    return redirectUrl;
                }

                return null;

            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Error occurred while redirecting: {ex.Message}");
                return null;
            }
        }
    }


}
