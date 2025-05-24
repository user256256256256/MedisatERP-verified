using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using MedisatERP.Data;
using MedisatERP.Models;
using MedisatERP.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http.HttpResults;

namespace MedisatERP.Controllers
{
    [Route("api/[controller]/[action]")]
    public class AspNetUsersAPIController : Controller
    {
        private ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ExceptionHandlerService _exceptionHandlerService;
        private readonly ILogger<AspNetUsersAPIController> _logger;
        private readonly NotificationService _notificationService;
        private readonly IErrorCodeService _errorCodeService;

        public AspNetUsersAPIController(ApplicationDbContext context, ExceptionHandlerService exceptionHandlerService, ILogger<AspNetUsersAPIController> logger, NotificationService notificationService, IErrorCodeService errorCodeService, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _exceptionHandlerService = exceptionHandlerService;
            _logger = logger;
            _notificationService = notificationService;
            _errorCodeService = errorCodeService;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        public async Task<IActionResult> Get(DataSourceLoadOptions loadOptions, Guid? companyId) {
            try
            {
                var aspnetusers = from i in _context.AspNetUsers
                    where i.CompanyId == companyId
                    select new
                    {
                    i.Id,
                    i.UserName,
                    i.NormalizedUserName,
                    i.Email,
                    i.NormalizedEmail,
                    i.EmailConfirmed,
                    i.PasswordHash,
                    i.SecurityStamp,
                    i.ConcurrencyStamp,
                    i.PhoneNumber,
                    i.PhoneNumberConfirmed,
                    i.TwoFactorEnabled,
                    i.LockoutEnd,
                    i.LockoutEnabled,
                    i.AccessFailedCount,
                    i.CompanyId,
                    i.ProfileImagePath,
                    i.BioData,
                    i.Gender,
                    i.Country,
                    i.Name,
                    i.Dob,
                    i.CreatedAt,
                    i.UpdatedAt
                };

                return Json(await DataSourceLoader.LoadAsync(aspnetusers, loadOptions));
            }
            catch (Exception ex)
            {
                return _exceptionHandlerService.HandleException(ex, this);
            }

        }

        [HttpPost]
        public async Task<IActionResult> Post(string values)
        {
            try
            {
                var userProfile = JsonConvert.DeserializeObject<UserProfilePOCO>(values);

                if (userProfile == null)
                    return BadRequest("Invalid user data.");

                if (string.IsNullOrWhiteSpace(userProfile.Password))
                {
                    var defaultPassword = "@Medisat?";
                    userProfile.Password = defaultPassword;
                }
               
                var identityUser = IdentityUserHelper.BuildIdentityUser(userProfile);

                var passwordHasher = new PasswordHasher<IdentityUser>();
                identityUser.PasswordHash = passwordHasher.HashPassword(identityUser, userProfile.Password);

                // Build AspNetUser using helper
                var aspNetUser = AspNetUserHelper.BuildAspNetUser(identityUser, userProfile);

                _context.AspNetUsers.Add(aspNetUser);
                await _context.SaveChangesAsync();

                return Json(new { aspNetUser.Id });
            }
            catch (Exception ex)
            {
                return _exceptionHandlerService.HandleException(ex, this);
            }
        }

        public static class AspNetUserHelper
        {
            public static AspNetUser BuildAspNetUser(IdentityUser identityUser, UserProfilePOCO userProfile)
            {
                return new AspNetUser
                {
                    Id = identityUser.Id,
                    UserName = identityUser.UserName,
                    Email = identityUser.Email,
                    PhoneNumber = identityUser.PhoneNumber,
                    TwoFactorEnabled = true,
                    LockoutEnabled = true,
                    CompanyId = userProfile.CompanyId,
                    BioData = userProfile.BioData,
                    Gender = userProfile.Gender,
                    Country = userProfile.Country,
                    Name = userProfile.Name,
                    Dob = userProfile.Dob,
                    CreatedAt = userProfile.CreatedAt = DateTime.UtcNow
                };
            }
        }

        public static class IdentityUserHelper
        {
            public static IdentityUser BuildIdentityUser(UserProfilePOCO userProfile)
            {
                return new IdentityUser
                {
                    UserName = userProfile.UserName,
                    NormalizedUserName = userProfile.UserName?.ToUpperInvariant(),
                    Email = userProfile.Email,
                    NormalizedEmail = userProfile.Email?.ToUpperInvariant(),
                    PasswordHash = userProfile.Password,
                    PhoneNumber = userProfile.PhoneNumber,
                    TwoFactorEnabled = true, 
                    LockoutEnabled = true    
                };
            }
        }


        [HttpPut]
        public async Task<IActionResult> Put(string key, string values)
        {
            try
            {
                var model = await _context.AspNetUsers.FirstOrDefaultAsync(item => item.Id == key);
                if (model == null)
                    return StatusCode(409, new { success = false, message = "Object not found" });

                var valuesDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(values);

                if (valuesDict.ContainsKey("Password") && valuesDict["Password"] != null)
                {
                    var password = valuesDict["Password"].ToString();
                    var passwordHasher = new PasswordHasher<AspNetUser>();
                    var hashedPassword = passwordHasher.HashPassword(model, password);
                    model.PasswordHash = hashedPassword;
                }

                model.UpdatedAt = DateTime.Now;

                PopulateModel(model, valuesDict);

                if (!TryValidateModel(model))
                    return BadRequest(new { success = false, message = GetFullErrorMessage(ModelState) });

                await _context.SaveChangesAsync();
                return Ok(new { success = true, message = "Profile updated successfully" });
            }
            catch (Exception ex)
            {
                return _exceptionHandlerService.HandleException(ex, this);
            }
        }


        [HttpDelete]
        public async Task Delete(string key) {
            try
            {
                var model = await _context.AspNetUsers.FirstOrDefaultAsync(item => item.Id == key);

                string currentProfilePath = model.ProfileImagePath;

                DeleteUserProfilePic(currentProfilePath);

                _context.AspNetUsers.Remove(model);
                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                 _exceptionHandlerService.HandleException(ex, this);
            }
        }

        [HttpPut]
        public async Task<IActionResult> UploadUserProfilePic(string userId, IFormFile profilePicture)
        {
            try
            {
                if (profilePicture != null && profilePicture.Length > 0)
                {
                    string currentProfilePicPath = GetCurrentProfilePicFilePath(userId);
                    string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "userProfileImages");

                    DeleteUserProfilePic(currentProfilePicPath);

                    string newFileName = $"{userId}_{Path.GetFileName(profilePicture.FileName)}";

                    string newProfilePicFilePath = Path.Combine(folderPath, newFileName);

                    try
                    {
                        using (var fileStream = new FileStream(newProfilePicFilePath, FileMode.Create))
                        {
                            await profilePicture.CopyToAsync(fileStream);
                        }
                    }
                    catch (Exception ex)
                    {
                        var errorDetails = _errorCodeService.GetErrorDetails("FILE_SAVE_FAILED");
                        _logger.LogWarning("Error while saving the new file: " + ex.Message);
                        return Json(new { success = false, message = errorDetails.ErrorMessage });
                    }

                    bool updateSuccess = UpdateUserProfilePicPath(userId, newFileName);

                    if (!updateSuccess)
                    {
                        var errorDetails = _errorCodeService.GetErrorDetails("DB_UPDATE_FAILED");
                        return Json(new { success = false, message = errorDetails.ErrorMessage });
                    }

                    return Json(new { success = true, message = "Your profile has been successfully updated." });

                } else
                {
                    var errorDetails = _errorCodeService.GetErrorDetails("NO_FILE_UPLOADED");
                    return Json(new { success = false, message = errorDetails.ErrorMessage });
                }


            }
            catch (Exception ex)
            {
                _logger.LogWarning("An exception error occurred: " + ex.Message);
                var errorDetails = _errorCodeService.GetErrorDetails("NO_FILE_UPLOADED");
                return Json(new { success = false, message = errorDetails.ErrorMessage });
            }
        }

        private string GetCurrentProfilePicFilePath(string userId)
        {
            var user = _context.AspNetUsers.FirstOrDefault(u => u.Id == userId);

            if (user != null)
            {
                return user.ProfileImagePath;
            }
            else
            {
                return null;
            }
        }


        private bool UpdateUserProfilePicPath(string userId, string newFileName)
        {
            var user = _context.AspNetUsers.FirstOrDefault(u => u.Id == userId);

            if (user != null)
            {
                try
                {
                    user.ProfileImagePath = newFileName;

                    _context.SaveChanges();

                    return true;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning("Error while updating the user profile picture file path: " + ex.Message);
                    return false;
                }
            }
            else
            {
                return false;
            }
        }


        private void DeleteUserProfilePic(string currentProfilePath)
        {
            if (!string.IsNullOrEmpty(currentProfilePath))
            {
                string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "userProfileImages");

                string currentProfileFullPath = Path.Combine(folderPath, currentProfilePath);

                if (System.IO.File.Exists(currentProfileFullPath))
                {
                    try
                    {
                        System.IO.File.Delete(currentProfileFullPath); 
                    }
                    catch (Exception ex)
                    {
                        _exceptionHandlerService.HandleException(ex, this);                    }
                }
            }
        }

        private void PopulateModel(AspNetUser model, IDictionary values) {
            string ID = nameof(AspNetUser.Id);
            string USER_NAME = nameof(AspNetUser.UserName);
            string NORMALIZED_USER_NAME = nameof(AspNetUser.NormalizedUserName);
            string EMAIL = nameof(AspNetUser.Email);
            string NORMALIZED_EMAIL = nameof(AspNetUser.NormalizedEmail);
            string EMAIL_CONFIRMED = nameof(AspNetUser.EmailConfirmed);
            string PASSWORD_HASH = nameof(AspNetUser.PasswordHash);
            string SECURITY_STAMP = nameof(AspNetUser.SecurityStamp);
            string CONCURRENCY_STAMP = nameof(AspNetUser.ConcurrencyStamp);
            string PHONE_NUMBER = nameof(AspNetUser.PhoneNumber);
            string PHONE_NUMBER_CONFIRMED = nameof(AspNetUser.PhoneNumberConfirmed);
            string TWO_FACTOR_ENABLED = nameof(AspNetUser.TwoFactorEnabled);
            string LOCKOUT_END = nameof(AspNetUser.LockoutEnd);
            string LOCKOUT_ENABLED = nameof(AspNetUser.LockoutEnabled);
            string ACCESS_FAILED_COUNT = nameof(AspNetUser.AccessFailedCount);
            string COMPANY_ID = nameof(AspNetUser.CompanyId);
            string PROFILE_IMAGE_PATH = nameof(AspNetUser.ProfileImagePath);
            string BIO_DATA = nameof(AspNetUser.BioData);
            string GENDER = nameof(AspNetUser.Gender);
            string COUNTRY = nameof(AspNetUser.Country);
            string NAME = nameof(AspNetUser.Name);
            string DOB = nameof(AspNetUser.Dob);
            string CREATED_AT = nameof(AspNetUser.CreatedAt);
            string UPDATED_AT = nameof(AspNetUser.UpdatedAt);

            if(values.Contains(ID)) {
                model.Id = Convert.ToString(values[ID]);
            }

            if(values.Contains(USER_NAME)) {
                model.UserName = Convert.ToString(values[USER_NAME]);
            }

            if(values.Contains(NORMALIZED_USER_NAME)) {
                model.NormalizedUserName = Convert.ToString(values[NORMALIZED_USER_NAME]);
            }

            if(values.Contains(EMAIL)) {
                model.Email = Convert.ToString(values[EMAIL]);
            }

            if(values.Contains(NORMALIZED_EMAIL)) {
                model.NormalizedEmail = Convert.ToString(values[NORMALIZED_EMAIL]);
            }

            if(values.Contains(EMAIL_CONFIRMED)) {
                model.EmailConfirmed = Convert.ToBoolean(values[EMAIL_CONFIRMED]);
            }

            if(values.Contains(PASSWORD_HASH)) {
                model.PasswordHash = Convert.ToString(values[PASSWORD_HASH]);
            }

            if(values.Contains(SECURITY_STAMP)) {
                model.SecurityStamp = Convert.ToString(values[SECURITY_STAMP]);
            }

            if(values.Contains(CONCURRENCY_STAMP)) {
                model.ConcurrencyStamp = Convert.ToString(values[CONCURRENCY_STAMP]);
            }

            if(values.Contains(PHONE_NUMBER)) {
                model.PhoneNumber = Convert.ToString(values[PHONE_NUMBER]);
            }

            if(values.Contains(PHONE_NUMBER_CONFIRMED)) {
                model.PhoneNumberConfirmed = Convert.ToBoolean(values[PHONE_NUMBER_CONFIRMED]);
            }

            if(values.Contains(TWO_FACTOR_ENABLED)) {
                model.TwoFactorEnabled = Convert.ToBoolean(values[TWO_FACTOR_ENABLED]);
            }

            if(values.Contains(LOCKOUT_END)) {
                model.LockoutEnd = values[LOCKOUT_END] != null ? ConvertTo<System.DateTimeOffset>(values[LOCKOUT_END]) : (DateTimeOffset?)null;
            }

            if(values.Contains(LOCKOUT_ENABLED)) {
                model.LockoutEnabled = Convert.ToBoolean(values[LOCKOUT_ENABLED]);
            }

            if(values.Contains(ACCESS_FAILED_COUNT)) {
                model.AccessFailedCount = Convert.ToInt32(values[ACCESS_FAILED_COUNT]);
            }

            if(values.Contains(COMPANY_ID)) {
                model.CompanyId = values[COMPANY_ID] != null ? ConvertTo<System.Guid>(values[COMPANY_ID]) : (Guid?)null;
            }

            if(values.Contains(PROFILE_IMAGE_PATH)) {
                model.ProfileImagePath = Convert.ToString(values[PROFILE_IMAGE_PATH]);
            }

            if(values.Contains(BIO_DATA)) {
                model.BioData = Convert.ToString(values[BIO_DATA]);
            }

            if(values.Contains(GENDER)) {
                model.Gender = Convert.ToString(values[GENDER]);
            }

            if(values.Contains(COUNTRY)) {
                model.Country = Convert.ToString(values[COUNTRY]);
            }

            if(values.Contains(NAME)) {
                model.Name = Convert.ToString(values[NAME]);
            }

            if(values.Contains(DOB)) {
                model.Dob = values[DOB] != null ? Convert.ToDateTime(values[DOB]) : (DateTime?)null;
            }

            if(values.Contains(CREATED_AT)) {
                model.CreatedAt = Convert.ToDateTime(values[CREATED_AT]);
            }

            if(values.Contains(UPDATED_AT)) {
                model.UpdatedAt = values[UPDATED_AT] != null ? Convert.ToDateTime(values[UPDATED_AT]) : (DateTime?)null;
            }
        }

        private T ConvertTo<T>(object value) {
            var converter = System.ComponentModel.TypeDescriptor.GetConverter(typeof(T));
            if(converter != null) {
                return (T)converter.ConvertFrom(null, CultureInfo.InvariantCulture, value);
            } else {
                // If necessary, implement a type conversion here
                throw new NotImplementedException();
            }
        }

        private string GetFullErrorMessage(ModelStateDictionary modelState) {
            var messages = new List<string>();

            foreach(var entry in modelState) {
                foreach(var error in entry.Value.Errors)
                    messages.Add(error.ErrorMessage);
            }

            return String.Join(" ", messages);
        }
    }
}