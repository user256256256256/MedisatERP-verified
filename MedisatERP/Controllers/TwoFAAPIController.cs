
using MedisatERP.Data;
using MedisatERP.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Microsoft.Data.SqlClient;

namespace MedisatERP.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class TwoFAAPIController : Controller
    {
        private readonly TwoFactorService _twoFAService;

        // Constructor
        public TwoFAAPIController(TwoFactorService twoFAService)
        {
            _twoFAService = twoFAService;
        }

        // Action to send 2FA code via GET request
        [HttpGet]
        public async Task<ActionResult> SendCode(string userId)
        {
            return await _twoFAService.SendCodeAsync(userId);
        }

        [HttpGet]
        public async Task<ActionResult> VerifyCode(string userId, string provider, string code, bool rememberMe)
        {
            return await _twoFAService.VerifyCodeAsync(userId, provider, code, rememberMe);
        }
    }
}



