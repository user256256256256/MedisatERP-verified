using MedisatERP.Areas.CoreSystem.Models;
using MedisatERP.Data;
using MedisatERP.Library;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


[ApiController]
[Route("api/[controller]/[action]")]
public class LoginAPIController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly MedisatErpDbContext _dbContext;

    public LoginAPIController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, MedisatErpDbContext dbContext)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<ActionResult> LoginCheck(string email, string password)
    {
        // Log the start of the login attempt
        Console.WriteLine($"Login attempt started for email: {email}");

        // Find user by email
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            // Log when the user is not found
            Console.WriteLine($"User not found for email: {email}");
            return BadRequest(new { success = false, mresponse = "User not found" });
        }

        // Log when the user is found
        Console.WriteLine($"User found for email: {email}");

        // Attempt login
        var result = await _signInManager.PasswordSignInAsync(user, password, isPersistent: false, lockoutOnFailure: false);

        if (result.Succeeded)
        {
            // Log successful login
            Console.WriteLine("Login successful.");

            // Get the roles of the user
            var roles = await _userManager.GetRolesAsync(user);

            // Retrieve the custom AspNetUser
            var aspNetUser = await _dbContext.Set<AspNetUser>().FirstOrDefaultAsync(u => u.Email == email);     

            if (roles.Contains("System Administrator"))
            {
                if (aspNetUser == null)
                {
                    // Log when the user is not found in the custom table
                    Console.WriteLine($"Custom user not found for email: {email}");
                    return BadRequest(new { success = false, mresponse = "User not found." });
                }

                // Log the found AspNetUser
                Console.WriteLine($"Custom user found with CompanyId: {aspNetUser.Id}");

                var userId = aspNetUser.Id;

                if(userId != null)
                {
                    // Encode the User Id using HashingHelper
                    var encodedUserId = HashingHelper.EncodeString(userId);

                    // Log the encoded CompanyId
                    Console.WriteLine($"Encoded UserId: {encodedUserId}");

                    // Construct the redirect URL
                    var redirectUrl = $"/CoreSystem/SystemManager/Index/{encodedUserId}";

                    // Return the response with the redirect URL
                    return Ok(new { success = true, mresponse = "Login successful", redirectUrl });
                }

              
            }
            else if (roles.Contains("Company Administrator"))
            {
                

                if (aspNetUser == null)
                {
                    // Log when the user is not found in the custom table
                    Console.WriteLine($"Custom user not found for email: {email}");
                    return BadRequest(new { success = false, mresponse = "User not found." });
                }

                // Log the found AspNetUser
                Console.WriteLine($"Custom user found with CompanyId: {aspNetUser.CompanyId}");

                var companyId = aspNetUser.CompanyId;

                if (companyId != null)
                {
                    // Encode the CompanyId using HashingHelper
                    var encodedCompanyId = HashingHelper.EncodeGuidID(companyId.Value);

                    // Log the encoded CompanyId
                    Console.WriteLine($"Encoded CompanyId: {encodedCompanyId}");

                    // Construct the redirect URL
                    var redirectUrl = $"/NutritionCompany/NutritionSystem/Index/{encodedCompanyId}";

                    // Return the response with the redirect URL
                    return Ok(new { success = true, mresponse = "Login successful", redirectUrl });
                }
            }
            else
            {
                // Log when the user is neither a System Administrator nor a Company Administrator
                Console.WriteLine("User is neither a System Administrator nor a Company Administrator.");
                return Ok(new { success = true, mresponse = "Login successful" });
            }
        }

        // Log for failed login attempt
        Console.WriteLine("Invalid login attempt for email: " + email);
        return BadRequest(new { success = false, mresponse = "Invalid login attempt" });
    }

}
