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
using Microsoft.CodeAnalysis;

namespace MedisatERP.Controllers
{
    [Route("api/[controller]/[action]")]
    public class AppointmentsAPIController : Controller
    {
        private ApplicationDbContext _context;
        private readonly ExceptionHandlerService _exceptionHandlerService;
        private readonly ILogger<AppointmentsAPIController> _logger;
        private readonly NotificationService _notificationService;
        private readonly IErrorCodeService _errorCodeService;
        private readonly IEmailSender _emailSender;

        public AppointmentsAPIController(ApplicationDbContext context, ExceptionHandlerService exceptionHandlerService, ILogger<AppointmentsAPIController> logger, NotificationService notificationService, IErrorCodeService errorCodeService, IEmailSender emailSender) {
            _context = context;
            _exceptionHandlerService = exceptionHandlerService;
            _logger = logger;
            _notificationService = notificationService;
            _errorCodeService = errorCodeService;
            _emailSender = emailSender;
        }

        public async Task<IActionResult> Get(DataSourceLoadOptions loadOptions, Guid companyId)
        {
            try
            {
                var appointments = _context.Appointments
                    .Where(a => a.CompanyId == companyId &&
                                a.Nutritionist.Roles.Any(r => r.Name.Contains("Comp_Nutritionist"))) 
                    .Select(i => new {
                        i.AppointmentId,
                        i.ClientId,
                        i.NutritionistId,
                        i.ScheduledDate,
                        i.WorkplaceId,
                        i.Status,
                        i.Priority,
                        i.ReminderSent,
                        i.ReminderSentAt,
                        i.Notes,
                        i.CreatedAt,
                        i.UpdatedAt,
                        i.Duration,
                        i.CompanyId,
                        Client = new
                        {
                            i.Client.ClientName,
                            i.Client.Email,
                            i.Client.PhoneNumber
                        },
                        Workplace = new
                        {
                            i.Workplace.Workplace
                        },
                        Nutritionist = new
                        {
                            i.Nutritionist.UserName,
                            i.Nutritionist.Email,
                            i.Nutritionist.PhoneNumber
                        }
                    })
                    .OrderBy(c => c.AppointmentId);

                return Json(await DataSourceLoader.LoadAsync(appointments, loadOptions));
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
                var model = new Appointment();
                var valuesDict = JsonConvert.DeserializeObject<IDictionary>(values);
                PopulateModel(model, valuesDict);

                if (model.CreatedAt == DateTime.MinValue)
                {
                    model.CreatedAt = DateTime.Now;
                }

                if (valuesDict.Contains("CompanyId") && Guid.TryParse(valuesDict["CompanyId"].ToString(), out Guid companyId))
                {
                    model.CompanyId = companyId;
                }

                var clientId = model.ClientId;
                var nutritionistId = model.NutritionistId;

                await SendAppointmentInitializedEmail(clientId, nutritionistId, model.ScheduledDate);

                if (!TryValidateModel(model))
                    return BadRequest(GetFullErrorMessage(ModelState));

                var result = _context.Appointments.Add(model);
                await _context.SaveChangesAsync();

                return Json(new { result.Entity.AppointmentId });
            }
            catch (Exception ex)
            {
                return _exceptionHandlerService.HandleException(ex, this);
            }
        }

        private async Task SendAppointmentInitializedEmail(Guid clientId, string nutritionistId, DateTime scheduledDate)
        {
            try
            {
                var client = await _context.CompanyClients.FindAsync(clientId);
                var nutritionist = await _context.AspNetUsers.FindAsync(nutritionistId);

                if (client == null || nutritionist == null)
                    throw new Exception("Client or Nutritionist not found.");

                string clientMessage = $"Dear {client.ClientName}, your appointment is scheduled for {scheduledDate}.";
                string nutritionistMessage = $"Dear {nutritionist.UserName}, you have been assigned an appointment scheduled for {scheduledDate}.";

                await _emailSender.SendEmailAsync(client.Email, "Appointment Scheduled", clientMessage);
                await _emailSender.SendEmailAsync(nutritionist.Email, "New Appointment Assigned", nutritionistMessage);

                _logger.LogInformation("Emails sent to client and nutritionist successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while sending appointment emails.");
                _exceptionHandlerService.HandleException(ex);
            }
        }

        [HttpPut]
        public async Task<IActionResult> Put(Guid key, string values)
        {
            try
            {
                var model = await _context.Appointments.FirstOrDefaultAsync(item => item.AppointmentId == key);
                if (model == null)
                    return StatusCode(409, "Object not found");

                // Store the original ScheduledDate for comparison
                var originalScheduledDate = model.ScheduledDate;

                var valuesDict = JsonConvert.DeserializeObject<IDictionary>(values);
                PopulateModel(model, valuesDict);

                if (!TryValidateModel(model))
                    return BadRequest(GetFullErrorMessage(ModelState));

                if (model.ScheduledDate != originalScheduledDate)
                {
                    await SendRescheduleEmail(model.ClientId, model.NutritionistId, originalScheduledDate, model.ScheduledDate);
                }

                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return _exceptionHandlerService.HandleException(ex, this);
            }
        }

        private async Task SendRescheduleEmail(Guid clientId, string nutritionistId, DateTime originalDate, DateTime newDate)
        {
            try
            {
                var client = await _context.CompanyClients.FindAsync(clientId);
                if (client == null)
                    throw new Exception("Client not found.");

                var nutritionist = await _context.AspNetUsers.FindAsync(nutritionistId);
                if (nutritionist == null)
                    throw new Exception("Nutritionist not found.");

                string clientMessage = $"Dear {client.ClientName}, your appointment has been rescheduled from {originalDate} to {newDate}.";
                string nutritionistMessage = $"Dear {nutritionist.UserName}, the appointment you are assigned to has been rescheduled from {originalDate} to {newDate}.";

                await _emailSender.SendEmailAsync(client.Email, "Appointment Rescheduled", clientMessage);

                await _emailSender.SendEmailAsync(nutritionist.Email, "Appointment Rescheduled", nutritionistMessage);

                _logger.LogInformation("Reschedule emails sent to client and nutritionist successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while sending reschedule emails.");
                _exceptionHandlerService.HandleException(ex);
            }
        }

        [HttpDelete]
        public async Task Delete(Guid key) {
            try
            {
                var model = await _context.Appointments.FirstOrDefaultAsync(item => item.AppointmentId == key);

                _context.Appointments.Remove(model);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                 _exceptionHandlerService.HandleException(ex, this);
            }
        }


        [HttpGet]
        public async Task<IActionResult> CompanyClientsLookup(DataSourceLoadOptions loadOptions) {
            var lookup = from i in _context.CompanyClients
                         orderby i.ClientName
                         select new {
                             Value = i.Id,
                             Text = i.ClientName
                         };
            return Json(await DataSourceLoader.LoadAsync(lookup, loadOptions));
        }

        [HttpGet]
        public async Task<IActionResult> CompaniesLookup(DataSourceLoadOptions loadOptions) {
            var lookup = from i in _context.Companies
                         orderby i.CompanyName
                         select new {
                             Value = i.CompanyId,
                             Text = i.CompanyName
                         };
            return Json(await DataSourceLoader.LoadAsync(lookup, loadOptions));
        }

        [HttpGet]
        public async Task<IActionResult> AspNetUsersLookup(DataSourceLoadOptions loadOptions) {
            var lookup = from i in _context.AspNetUsers
                         orderby i.UserName
                         select new {
                             Value = i.Id,
                             Text = i.UserName
                         };
            return Json(await DataSourceLoader.LoadAsync(lookup, loadOptions));
        }

        [HttpGet]
        public async Task<IActionResult> WorkplaceLookupsLookup(DataSourceLoadOptions loadOptions) {
            var lookup = from i in _context.WorkplaceLookups
                         orderby i.Workplace
                         select new {
                             Value = i.Id,
                             Text = i.Workplace
                         };
            return Json(await DataSourceLoader.LoadAsync(lookup, loadOptions));
        }

        private void PopulateModel(Appointment model, IDictionary values) {
            string APPOINTMENT_ID = nameof(Appointment.AppointmentId);
            string CLIENT_ID = nameof(Appointment.ClientId);
            string NUTRITIONIST_ID = nameof(Appointment.NutritionistId);
            string SCHEDULED_DATE = nameof(Appointment.ScheduledDate);
            string WORKPLACE_ID = nameof(Appointment.WorkplaceId);
            string STATUS = nameof(Appointment.Status);
            string PRIORITY = nameof(Appointment.Priority);
            string REMINDER_SENT = nameof(Appointment.ReminderSent);
            string REMINDER_SENT_AT = nameof(Appointment.ReminderSentAt);
            string NOTES = nameof(Appointment.Notes);
            string CREATED_AT = nameof(Appointment.CreatedAt);
            string UPDATED_AT = nameof(Appointment.UpdatedAt);
            string DURATION = nameof(Appointment.Duration);
            string COMPANY_ID = nameof(Appointment.CompanyId);

            if(values.Contains(APPOINTMENT_ID)) {
                model.AppointmentId = ConvertTo<System.Guid>(values[APPOINTMENT_ID]);
            }

            if(values.Contains(CLIENT_ID)) {
                model.ClientId = ConvertTo<System.Guid>(values[CLIENT_ID]);
            }

            if(values.Contains(NUTRITIONIST_ID)) {
                model.NutritionistId = Convert.ToString(values[NUTRITIONIST_ID]);
            }

            if(values.Contains(SCHEDULED_DATE)) {
                model.ScheduledDate = Convert.ToDateTime(values[SCHEDULED_DATE]);
            }

            if(values.Contains(WORKPLACE_ID)) {
                model.WorkplaceId = Convert.ToInt32(values[WORKPLACE_ID]);
            }

            if(values.Contains(STATUS)) {
                model.Status = Convert.ToString(values[STATUS]);
            }

            if(values.Contains(PRIORITY)) {
                model.Priority = Convert.ToString(values[PRIORITY]);
            }

            if(values.Contains(REMINDER_SENT)) {
                model.ReminderSent = Convert.ToBoolean(values[REMINDER_SENT]);
            }

            if(values.Contains(REMINDER_SENT_AT)) {
                model.ReminderSentAt = values[REMINDER_SENT_AT] != null ? Convert.ToDateTime(values[REMINDER_SENT_AT]) : (DateTime?)null;
            }

            if(values.Contains(NOTES)) {
                model.Notes = Convert.ToString(values[NOTES]);
            }

            if(values.Contains(CREATED_AT)) {
                model.CreatedAt = Convert.ToDateTime(values[CREATED_AT]);
            }

            if(values.Contains(UPDATED_AT)) {
                model.UpdatedAt = values[UPDATED_AT] != null ? Convert.ToDateTime(values[UPDATED_AT]) : (DateTime?)null;
            }

            if(values.Contains(DURATION)) {
                model.Duration = values[DURATION] != null ? Convert.ToInt32(values[DURATION]) : (int?)null;
            }

            if(values.Contains(COMPANY_ID)) {
                model.CompanyId = ConvertTo<System.Guid>(values[COMPANY_ID]);
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