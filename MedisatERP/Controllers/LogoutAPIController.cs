using MedisatERP.Library;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
[ApiController]
[Route("api/[controller]/[action]")]
public class LogoutAPIController : ControllerBase
{
    private readonly SignInManager<IdentityUser> _signInManager;

    public LogoutAPIController(SignInManager<IdentityUser> signInManager)
    {
        _signInManager = signInManager;
    }

    [HttpPost]
    public IActionResult LogoutCheck([FromBody] string userId)
    {
        try
        {
            // Check if userId is null or empty
            if (string.IsNullOrEmpty(userId))
            {
                Console.WriteLine("userId is null or empty");
                return BadRequest(new { success = false, message = "Invalid user ID." });
            }

            // Optionally decode the userId if needed
            var decodedUserId = HashingHelper.DecodeString(userId);
            if (decodedUserId == null)
            {
                Console.WriteLine("Decoded userId is null");
                return BadRequest(new { success = false, message = "Invalid user ID after decoding." });
            }

            Console.WriteLine("The decoded user id is :" + decodedUserId);

            // Log out successful
            return Ok(new { success = true, message = "Logout successful.", redirectUrl = "/" });
        }
        catch (Exception ex)
        {
            // Log the full exception for better debugging
            Console.WriteLine($"Logout failed: {ex.ToString()}");
            return BadRequest(new { success = false, message = "Logout failed. Please try again." });
        }
    }
}

