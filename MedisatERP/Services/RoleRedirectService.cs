using MedisatERP.Areas.CoreSystem.Models;
using MedisatERP.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedisatERP.Services
{
    // RoleRedirectService.cs
    public class RoleRedirectService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly MedisatErpDbContext _dbContext;

        public RoleRedirectService(UserManager<IdentityUser> userManager, MedisatErpDbContext dbContext)
        {
            _userManager = userManager;
            _dbContext = dbContext;
        }

        public async Task<ActionResult> HandleRoleRedirectAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            // Retrieve user roles
            var roles = await _userManager.GetRolesAsync(user);
            Console.WriteLine($"Roles for user {email}: {string.Join(", ", roles)}");

            // Retrieve the custom AspNetUser data
            var aspNetUser = await _dbContext.Set<AspNetUser>().FirstOrDefaultAsync(u => u.Email == email);
            if (aspNetUser == null)
            {
                Console.WriteLine($"No custom AspNetUser found for email: {email}");
                return new BadRequestObjectResult(new { success = false, mresponse = "Invalid user data" });
            }

            var userId = aspNetUser.Id;
            Console.WriteLine($"AspNetUser found, UserId: {userId}");

            string redirectUrl = null;

            // Role-based redirection logic
            if (roles.Contains("System Administrator"))
            {
                Console.WriteLine("User is a System Administrator.");
                if (userId != null)
                {
                    var encodedUserId = HashingHelper.EncodeString(userId);
                    Console.WriteLine($"Encoded UserId: {encodedUserId}");
                    redirectUrl = $"/CoreSystem/SystemManager/Index/{encodedUserId}";
                }
            }
            else if (roles.Contains("Company Administrator"))
            {
                Console.WriteLine("User is a Company Administrator.");
                var companyId = aspNetUser.CompanyId;
                Console.WriteLine($"Custom user found with CompanyId: {companyId}");

                if (companyId != null)
                {
                    var encodedCompanyId = HashingHelper.EncodeGuidID(companyId.Value);
                    Console.WriteLine($"Encoded CompanyId: {encodedCompanyId}");
                    redirectUrl = $"/NutritionCompany/NutritionSystem/Index/{encodedCompanyId}";
                }
            }
            else
            {
                Console.WriteLine("User is neither a System Administrator nor a Company Administrator.");
                return new OkObjectResult(new { success = true, mresponse = "Login successful" });
            }

            // Return the redirect URL if we constructed one
            if (redirectUrl != null)
            {
                return new OkObjectResult(new { success = true, mresponse = "Login successful", redirectUrl });
            }

            // If no redirect URL was found, fallback to successful login
            return new OkObjectResult(new { success = true, mresponse = "Login successful" });
        }
    }

}
