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
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace MedisatERP.Controllers
{
    [Route("api/[controller]/[action]")]
    public class OnlineApplicantsAPIController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ExceptionHandlerService _exceptionHandlerService;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<OnlineApplicantsAPIController> _logger;
        public OnlineApplicantsAPIController(ApplicationDbContext context, ExceptionHandlerService exceptionHandlerService, IEmailSender emailSender, ILogger<OnlineApplicantsAPIController> logger)
        {
            _context = context;
            _exceptionHandlerService = exceptionHandlerService;
            _emailSender = emailSender;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Get(DataSourceLoadOptions loadOptions) {
            try
            {
                var onlineapplicants = _context.OnlineApplicants.Select(i => new {
                    i.Id,
                    i.FirstName,
                    i.LastName,
                    i.Email,
                    i.MobilePhoneNo,
                    i.Address,
                    i.Age,
                    i.Reason,
                    i.PreferredSchedule,
                    i.HowDidYouHearAboutUs,
                    i.AcceptPrivacyPolicies,
                    i.CreatedDate
                });

                return Json(await DataSourceLoader.LoadAsync(onlineapplicants, loadOptions));
            }
            catch (Exception ex)
            {
                return _exceptionHandlerService.HandleException(ex, this);
            }
        }

        [HttpPost]
        public async Task RejectClientApplication(string clientId, string reason)
        {
            try
            {
                var client = await _context.CompanyClients.FindAsync(clientId);

                if (client == null || reason == null)
                    throw new Exception("Client or Reason not found.");

                string clientMessage = $"Dear {client.ClientName}, your application has been rejected because: {reason}. ";

                await _emailSender.SendEmailAsync(client.Email, "Appointment Rejected", clientMessage);

                _logger.LogInformation("Emails sent to client successfully.");

            }
            catch (Exception ex)
            {
                _exceptionHandlerService.HandleException(ex, this);
            }
        }

        [HttpPut]
        public async Task<IActionResult> ApproveApplicant(Guid applicantId, Guid companyId)
        {
            try
            {
                var applicant = await _context.OnlineApplicants.FindAsync(applicantId);

                if (applicant == null)
                    return NotFound(new { success = false, message = "Applicant not found." });
                
                int age = applicant.Age ?? 30; // Default to 30 if null --> You need to correct the assigning of default value of making the age nullabel for applicants
                DateTime estimatedDateOfBirth = DateTime.UtcNow.AddYears(-age);

                var newClient = new CompanyClient
                {
                    Id = Guid.NewGuid(),
                    CompanyId = companyId,
                    ClientName = $"{applicant.FirstName} {applicant.LastName}",
                    Gender = "Unknown",
                    Email = applicant.Email,
                    PhoneNumber = applicant.MobilePhoneNo,
                    EmergencyContactName = "N/A",
                    EmergencyContactPhone = "N/A",
                    MaritalStatus = "N/A",
                    Nationality = "N/A",
                    CreatedAt = DateTime.UtcNow,
                    Street = applicant.Address,
                    City = "N/A",
                    State = "N/A",
                    PostalCode = "N/A",
                    Country = "N/A",
                    EmergencyContactRelationship = "N/A",                  
                    DateOfBirth = estimatedDateOfBirth
                };

                await _context.CompanyClients.AddAsync(newClient);
                _context.OnlineApplicants.Remove(applicant);
                await _context.SaveChangesAsync();

                string clientMessage = $"Dear {newClient.ClientName}, your application has been approved and you are now registered as a client of LyfexAfrica!";
                await _emailSender.SendEmailAsync(newClient.Email, "Application Approved", clientMessage);

                _logger.LogInformation("Applicant approved and added as a client successfully.");

                return Ok(new { success = true, message = "Applicant approved successfully!", clientId = newClient.Id });
            }
            catch (Exception ex)
            {
                _exceptionHandlerService.HandleException(ex, this);
                return StatusCode(500, new { success = false, message = "An error occurred during approval." });
            }
        }


        [HttpPost]
        public async Task<IActionResult> Post(string values) {
            var model = new OnlineApplicant();
            var valuesDict = JsonConvert.DeserializeObject<IDictionary>(values);
            PopulateModel(model, valuesDict);

            if(!TryValidateModel(model))
                return BadRequest(GetFullErrorMessage(ModelState));

            var result = _context.OnlineApplicants.Add(model);
            await _context.SaveChangesAsync();

            return Json(new { result.Entity.Id });
        }

        [HttpPut]
        public async Task<IActionResult> Put(Guid key, string values) {
            var model = await _context.OnlineApplicants.FirstOrDefaultAsync(item => item.Id == key);
            if(model == null)
                return StatusCode(409, "Object not found");

            var valuesDict = JsonConvert.DeserializeObject<IDictionary>(values);
            PopulateModel(model, valuesDict);

            if(!TryValidateModel(model))
                return BadRequest(GetFullErrorMessage(ModelState));

            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete]
        public async Task Delete(Guid key) {
            var model = await _context.OnlineApplicants.FirstOrDefaultAsync(item => item.Id == key);

            _context.OnlineApplicants.Remove(model);
            await _context.SaveChangesAsync();
        }


        private void PopulateModel(OnlineApplicant model, IDictionary values) {
            string ID = nameof(OnlineApplicant.Id);
            string FIRST_NAME = nameof(OnlineApplicant.FirstName);
            string LAST_NAME = nameof(OnlineApplicant.LastName);
            string EMAIL = nameof(OnlineApplicant.Email);
            string MOBILE_PHONE_NO = nameof(OnlineApplicant.MobilePhoneNo);
            string ADDRESS = nameof(OnlineApplicant.Address);
            string AGE = nameof(OnlineApplicant.Age);
            string REASON = nameof(OnlineApplicant.Reason);
            string PREFERRED_SCHEDULE = nameof(OnlineApplicant.PreferredSchedule);
            string HOW_DID_YOU_HEAR_ABOUT_US = nameof(OnlineApplicant.HowDidYouHearAboutUs);
            string ACCEPT_PRIVACY_POLICIES = nameof(OnlineApplicant.AcceptPrivacyPolicies);
            string CREATED_DATE = nameof(OnlineApplicant.CreatedDate);

            if(values.Contains(ID)) {
                model.Id = ConvertTo<System.Guid>(values[ID]);
            }

            if(values.Contains(FIRST_NAME)) {
                model.FirstName = Convert.ToString(values[FIRST_NAME]);
            }

            if(values.Contains(LAST_NAME)) {
                model.LastName = Convert.ToString(values[LAST_NAME]);
            }

            if(values.Contains(EMAIL)) {
                model.Email = Convert.ToString(values[EMAIL]);
            }

            if(values.Contains(MOBILE_PHONE_NO)) {
                model.MobilePhoneNo = Convert.ToString(values[MOBILE_PHONE_NO]);
            }

            if(values.Contains(ADDRESS)) {
                model.Address = Convert.ToString(values[ADDRESS]);
            }

            if(values.Contains(AGE)) {
                model.Age = values[AGE] != null ? Convert.ToInt32(values[AGE]) : (int?)null;
            }

            if(values.Contains(REASON)) {
                model.Reason = Convert.ToString(values[REASON]);
            }

            if(values.Contains(PREFERRED_SCHEDULE)) {
                model.PreferredSchedule = values[PREFERRED_SCHEDULE] != null ? Convert.ToDateTime(values[PREFERRED_SCHEDULE]) : (DateTime?)null;
            }

            if(values.Contains(HOW_DID_YOU_HEAR_ABOUT_US)) {
                model.HowDidYouHearAboutUs = Convert.ToString(values[HOW_DID_YOU_HEAR_ABOUT_US]);
            }

            if(values.Contains(ACCEPT_PRIVACY_POLICIES)) {
                model.AcceptPrivacyPolicies = Convert.ToBoolean(values[ACCEPT_PRIVACY_POLICIES]);
            }

            if(values.Contains(CREATED_DATE)) {
                model.CreatedDate = Convert.ToDateTime(values[CREATED_DATE]);
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