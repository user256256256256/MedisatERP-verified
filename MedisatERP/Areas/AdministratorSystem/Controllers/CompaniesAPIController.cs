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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Azure.Core;

namespace MedisatERP.Controllers
{
    [Route("api/[controller]/[action]")]
    public class CompaniesAPIController : Controller
    {
        private ApplicationDbContext _context;
        private readonly ExceptionHandlerService _exceptionHandlerService;
        private readonly ILogger<CompaniesAPIController> _logger;
        private readonly NotificationService _notificationService;
        private readonly IErrorCodeService _errorCodeService;
        private readonly HandelRoleRedirectService _handelRoleRedirectService;
        private readonly IUserSessionService _sessionService;
        public CompaniesAPIController(ApplicationDbContext context, ExceptionHandlerService exceptionHandlerService, ILogger<CompaniesAPIController> logger, NotificationService notificationService, IErrorCodeService errorCodeService, HandelRoleRedirectService handelRoleRedirectService, IUserSessionService sessionService)
        {
            _context = context;
            _exceptionHandlerService = exceptionHandlerService;
            _logger = logger;
            _notificationService = notificationService;
            _errorCodeService = errorCodeService;
            _handelRoleRedirectService = handelRoleRedirectService;
            _sessionService = sessionService;
        }

        [HttpGet]
        public async Task<IActionResult> Get(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var companies = _context.Companies.Select(i => new {
                    i.CompanyId,
                    i.CompanyName,
                    i.ContactPerson,
                    i.CompanyEmail,
                    i.CompanyPhone,
                    i.ApiCode,
                    i.CompanyInitials,
                    i.Motto,
                    i.CompanyType,
                    i.CreatedAt,
                    i.CompanyLogoFilePath,
                    i.Status,
                    i.CompanyWebsite,
                    i.AboutCompany,
                    i.Street,
                    i.City,
                    i.State,
                    i.PostalCode,
                    i.Country,
                    i.ContactPersonPhone
                });

                return Json(await DataSourceLoader.LoadAsync(companies, loadOptions));
            }
            catch (Exception ex)
            {
                return _exceptionHandlerService.HandleException(ex, this);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post(string values) {
            try
            {
                var model = new Company();
                var valuesDict = JsonConvert.DeserializeObject<IDictionary>(values);
                PopulateModel(model, valuesDict);

                if (model.CreatedAt == null)
                {
                    model.CreatedAt = DateTime.Now;
                }

                if (!TryValidateModel(model))
                    return BadRequest(GetFullErrorMessage(ModelState));

                var result = _context.Companies.Add(model);
                await _context.SaveChangesAsync();

                await _notificationService.NotifyNewCompanyCreation(model.CompanyId);

                return Json(new { result.Entity.CompanyId });
            }
            catch (Exception ex)
            {
                return _exceptionHandlerService.HandleException(ex, this);
            }
            
        }

        [HttpPut]
        public async Task<IActionResult> Put(Guid key, string values) {
            try
            {
                var model = await _context.Companies.FirstOrDefaultAsync(item => item.CompanyId == key);
                if (model == null)
                    return StatusCode(409, "Object not found");

                var valuesDict = JsonConvert.DeserializeObject<IDictionary>(values);
                PopulateModel(model, valuesDict);

                if (!TryValidateModel(model))
                    return BadRequest(GetFullErrorMessage(ModelState));

                await _context.SaveChangesAsync();
                return Ok();
            } 
            catch (Exception ex)
            {
               return _exceptionHandlerService.HandleException(ex, this);
            }
        }

        [HttpDelete]
        public async Task Delete(Guid key) {
            try
            {
                var model = await _context.Companies.FirstOrDefaultAsync(item => item.CompanyId == key);

                // Retrieve the current logo file path from the database
                string currentLogoFilePath = model.CompanyLogoFilePath;

                // Check if the logo exists and delete it if necessary
                DeleteCompanyLogo(currentLogoFilePath);

                _context.Companies.Remove(model);
                await _context.SaveChangesAsync();

                await _notificationService.NotifyCompanyDeletion(model.CompanyId);

            }
            catch (Exception ex)
            {
                 _exceptionHandlerService.HandleException(ex, this);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCompanyLogo(Guid companyId )
        {
            try
            {
                if (companyId == Guid.Empty)
                {
                    return BadRequest("Unauthorized access: Invalid Company Id!");
                }

                var companyLogo = await _context.Companies
                .Where(c => c.CompanyId == companyId)
                .Select(c => c.CompanyLogoFilePath)
                .FirstOrDefaultAsync();

                if (string.IsNullOrEmpty(companyLogo))
                {
                    companyLogo = "defaultCompanyLogo.jpeg";
                }

                return Json(new
                {
                    data = new[]
                    {
                        new
                        {
                            CompanyId = companyId,
                            CompanyLogoFilePath = companyLogo
                        }
                    }
                });

            }
            catch (Exception ex)
            {
                return _exceptionHandlerService.HandleException(ex, this);
            }
        }

        [HttpPut] 
        public async Task<ActionResult> UploadLogo(string companyId, IFormFile companyLogo)
        {
            try
            {
                // Retrieve the current logo file path from the database using companyId
                string currentLogoFilePath = GetCurrentLogoFilePath(companyId);

                // Ensure the folder path where logos are stored
                string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "companyLogoImages");

                // Check if the logo exists and delete it 
                DeleteCompanyLogo(currentLogoFilePath);

                // Generate a new unique file name for the logo to avoid conflicts
                string newFileName = $"{companyId}_{Path.GetFileName(companyLogo.FileName)}";

                // Step 6: Construct the full path for the new logo
                string newLogoFilePath = Path.Combine(folderPath, newFileName);

                // Step 7: Save the new logo file to the server
                try
                {
                    using (var fileStream = new FileStream(newLogoFilePath, FileMode.Create))
                    {
                        await companyLogo.CopyToAsync(fileStream);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning("An exception error occurred: " + ex);
                    var errorDetails = _errorCodeService.GetErrorDetails("FILE_SAVE_FAILED");
                    return Json(new { success = false, message = errorDetails.ErrorMessage });
                }

                // Update the company's logo file path in the database with the new file name
                bool updateSuccess = UpdateCompanyLogoFilePath(companyId, newFileName);
                if (!updateSuccess) 
                {
                    var errorDetails = _errorCodeService.GetErrorDetails("DB_UPDATE_FAILED");
                    return Json(new {success = false, message = errorDetails.ErrorMessage});
                }

                return Json(new { success = true, message = "Company logo successfully uploaded" });
            }
            catch (Exception ex)
            {
                _logger.LogWarning("An exception error occurred: " + ex.Message);
                var errorDetails = _errorCodeService.GetErrorDetails("NO_FILE_UPLOADED");
                return Json(new { success = false, message = errorDetails.ErrorMessage });

            }
        }

        private void  DeleteCompanyLogo(string currentLogoFilePath)
        {
            if (!string.IsNullOrEmpty(currentLogoFilePath))
            {
                // Ensure the folder path where logos are stored
                string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "companyLogoImages");

                // Step 3: Construct the full path for the current logo file
                string currentLogoFullPath = Path.Combine(folderPath, currentLogoFilePath);

                // Step 4: Delete the existing logo file if it exists
                if (System.IO.File.Exists(currentLogoFullPath))
                {
                    try
                    {
                        // Delete the old logo file
                        System.IO.File.Delete(currentLogoFullPath);
                    }
                    catch (Exception ex)
                    {
                        _exceptionHandlerService.HandleException(ex, this);
                    }
                }
            }
        }

        private string GetCurrentLogoFilePath(string companyId)
        {
            if (Guid.TryParse(companyId, out Guid companyIdGuid))
            {
                var company = _context.Companies.FirstOrDefault(c => c.CompanyId == companyIdGuid);

                if (company != null)
                {
                    return company.CompanyLogoFilePath;
                }
                else
                {
                    return null;
                }
            }
            else { return null; }
        }

        private bool UpdateCompanyLogoFilePath(string companyId, string newFileName)
        {
            if (Guid.TryParse(companyId, out Guid companyIdGuid))
            {
                var company = _context.Companies.FirstOrDefault(c => c.CompanyId == companyIdGuid);

                if (company != null)
                {
                    try
                    {
                        company.CompanyLogoFilePath = newFileName;
                        _context.SaveChanges();

                        return true;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning("Error while updating the company logo file path: " + ex.Message);
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        [HttpPost]
        public async Task<ActionResult> RedirectToCompany(string userId, string companyId)
        {
            try
            {
                var user = await _context.AspNetUsers.FirstOrDefaultAsync(c => c.Id == userId);
                if (user == null)
                {
                    return Json(new { success = false, mresponse = _errorCodeService.GetErrorDetails("USER_NOT_FOUND") });
                }

                var email = user.Email;

                if (!Guid.TryParse(companyId, out Guid companyIdGuid)) 
                {
                    return Json(new { success = false, mresponse = _errorCodeService.GetErrorDetails("INVALID_INPUT") });
                }

                _sessionService.SetSessionData(userId, companyIdGuid);

                var redirectUrl = await _handelRoleRedirectService.HandleAdminToCompSystemAsync(email, companyIdGuid);

                if(string.IsNullOrEmpty(redirectUrl))
                {
                    return Json(new { success = false, mresponse = _errorCodeService.GetErrorDetails("ROLE_NOT_APPLICABLE") });
                }

                return Json(new { success = true, redirectUrl });
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Error occurred while redirection: {ex.Message}");
                var errorDetails = _errorCodeService.GetErrorDetails("INTERNAL_SERVER_ERROR");
                return Json(new {success = false, mresponse = errorDetails});
            }
        }

        private void PopulateModel(Company model, IDictionary values) {
            string COMPANY_ID = nameof(Company.CompanyId);
            string COMPANY_NAME = nameof(Company.CompanyName);
            string CONTACT_PERSON = nameof(Company.ContactPerson);
            string COMPANY_EMAIL = nameof(Company.CompanyEmail);
            string COMPANY_PHONE = nameof(Company.CompanyPhone);
            string API_CODE = nameof(Company.ApiCode);
            string COMPANY_INITIALS = nameof(Company.CompanyInitials);
            string MOTTO = nameof(Company.Motto);
            string COMPANY_TYPE = nameof(Company.CompanyType);
            string CREATED_AT = nameof(Company.CreatedAt);
            string COMPANY_LOGO_FILE_PATH = nameof(Company.CompanyLogoFilePath);
            string STREET = nameof(Company.Street);
            string CITY = nameof(Company.City);
            string STATE = nameof(Company.State);
            string POSTAL_CODE = nameof(Company.PostalCode);
            string COUNTRY = nameof(Company.Country);
            string STATUS = nameof(Company.Status);
            string COMPANY_WEBSITE = nameof(Company.CompanyWebsite);
            string ABOUT_COMPANY = nameof(Company.AboutCompany);
            string CONTACT_PERSON_PHONE = nameof(Company.ContactPersonPhone);

            if(values.Contains(COMPANY_ID)) {
                model.CompanyId = ConvertTo<System.Guid>(values[COMPANY_ID]);
            }

            if(values.Contains(COMPANY_NAME)) {
                model.CompanyName = Convert.ToString(values[COMPANY_NAME]);
            }

            if(values.Contains(CONTACT_PERSON)) {
                model.ContactPerson = Convert.ToString(values[CONTACT_PERSON]);
            }

            if(values.Contains(COMPANY_EMAIL)) {
                model.CompanyEmail = Convert.ToString(values[COMPANY_EMAIL]);
            }

            if(values.Contains(COMPANY_PHONE)) {
                model.CompanyPhone = Convert.ToString(values[COMPANY_PHONE]);
            }

            if(values.Contains(API_CODE)) {
                model.ApiCode = Convert.ToString(values[API_CODE]);
            }

            if(values.Contains(COMPANY_INITIALS)) {
                model.CompanyInitials = Convert.ToString(values[COMPANY_INITIALS]);
            }

            if(values.Contains(MOTTO)) {
                model.Motto = Convert.ToString(values[MOTTO]);
            }

            if(values.Contains(COMPANY_TYPE)) {
                model.CompanyType = Convert.ToString(values[COMPANY_TYPE]);
            }

            if(values.Contains(CREATED_AT)) {
                model.CreatedAt = values[CREATED_AT] != null ? Convert.ToDateTime(values[CREATED_AT]) : (DateTime?)null;
            }

            if(values.Contains(COMPANY_LOGO_FILE_PATH)) {
                model.CompanyLogoFilePath = Convert.ToString(values[COMPANY_LOGO_FILE_PATH]);
            }

            if(values.Contains(STREET)) {
                model.Street = Convert.ToString(values[STREET]);
            }

            if(values.Contains(CITY)) {
                model.City = Convert.ToString(values[CITY]);
            }

            if(values.Contains(STATE)) {
                model.State = Convert.ToString(values[STATE]);
            }

            if(values.Contains(POSTAL_CODE)) {
                model.PostalCode = Convert.ToString(values[POSTAL_CODE]);
            }

            if(values.Contains(COUNTRY)) {
                model.Country = Convert.ToString(values[COUNTRY]);
            }

            if(values.Contains(STATUS)) {
                model.Status = Convert.ToString(values[STATUS]);
            }

            if(values.Contains(COMPANY_WEBSITE)) {
                model.CompanyWebsite = Convert.ToString(values[COMPANY_WEBSITE]);
            }

            if(values.Contains(ABOUT_COMPANY)) {
                model.AboutCompany = Convert.ToString(values[ABOUT_COMPANY]);
            }

            if(values.Contains(CONTACT_PERSON_PHONE)) {
                model.ContactPersonPhone = Convert.ToString(values[CONTACT_PERSON_PHONE]);
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