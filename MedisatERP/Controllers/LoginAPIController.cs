using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]/[action]")]
public class LoginAPIController : ControllerBase
{
	private readonly UserManager<IdentityUser> _userManager;
	private readonly SignInManager<IdentityUser> _signInManager;

	public LoginAPIController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
	{
		_userManager = userManager;
		_signInManager = signInManager;
	}

	[HttpGet]
	public async Task<ActionResult> LoginCheck(string email, string password)
	{
		var user = await _userManager.FindByEmailAsync(email);
		if (user == null)
		{
			return BadRequest(new { success = false, mresponse = "User not found" });
		}

		var result = await _signInManager.PasswordSignInAsync(user, password, isPersistent: false, lockoutOnFailure: false);

		if (result.Succeeded)
		{
			return Ok(new { success = true, mresponse = "Login successful" }); // Redirect URL can be added here.
		}

		return BadRequest(new { success = false, mresponse = "Invalid login attempt" });
	}
}
