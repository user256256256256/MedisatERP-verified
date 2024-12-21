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
using MedisatERP.Areas.CoreSystem.Models;

namespace MedisatERP.Controllers
{
    [Route("api/[controller]/[action]")]
    public class TrialNotificationsAPIController : Controller
    {
        private MedisatErpDbContext _context;

        public TrialNotificationsAPIController(MedisatErpDbContext context) {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get(DataSourceLoadOptions loadOptions) {
            var trialnotifications = _context.TrialNotifications.Select(i => new {
                i.Id,
                i.CompanyId,
                i.TrialStartDate,
                i.TrialEndDate,
                i.IsNotified,
                i.ReminderDate,
                i.SentAt,
                i.NotificationTypeId,
                Company = new
                {
                    i.Company.CompanyName,
                    i.Company.CompanyEmail,
                    i.Company.CompanyPhone
                },
                NotificationType = new
                { 
                    i.NotificationType.Type,
                    i.NotificationType.Message
                }
            });

            // Debug the query before applying DataSourceLoader
            Console.WriteLine("Tiral notification query built. Applying DataSourceLoader...");

            // Apply filtering, sorting, and paging using DataSourceLoader
            var transformedData = await DataSourceLoader.LoadAsync(trialnotifications, loadOptions);

            // Serialize the retrieved object and log it
            var serializedData = JsonConvert.SerializeObject(transformedData, Formatting.Indented);
            Console.WriteLine($"Data transformation completed. Retrieved data: {serializedData}");

            // Return the transformed data as JSON
            return Json(transformedData);
        }

        [HttpPost]
        public async Task<IActionResult> Post(string values) {
            var model = new TrialNotification();
            var valuesDict = JsonConvert.DeserializeObject<IDictionary>(values);
            PopulateModel(model, valuesDict);

            if(!TryValidateModel(model))
                return BadRequest(GetFullErrorMessage(ModelState));

            var result = _context.TrialNotifications.Add(model);
            await _context.SaveChangesAsync();

            return Json(new { result.Entity.Id });
        }

        [HttpPut]
        public async Task<IActionResult> Put(Guid key, string values) {
            var model = await _context.TrialNotifications.FirstOrDefaultAsync(item => item.Id == key);
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
            var model = await _context.TrialNotifications.FirstOrDefaultAsync(item => item.Id == key);

            _context.TrialNotifications.Remove(model);
            await _context.SaveChangesAsync();
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
        public async Task<IActionResult> TrialNotificationLookupsLookup(DataSourceLoadOptions loadOptions) {
            var lookup = from i in _context.TrialNotificationLookups
                         orderby i.Type
                         select new {
                             Value = i.Id,
                             Text = i.Type
                         };
            return Json(await DataSourceLoader.LoadAsync(lookup, loadOptions));
        }

        private void PopulateModel(TrialNotification model, IDictionary values) {
            string ID = nameof(TrialNotification.Id);
            string COMPANY_ID = nameof(TrialNotification.CompanyId);
            string TRIAL_START_DATE = nameof(TrialNotification.TrialStartDate);
            string TRIAL_END_DATE = nameof(TrialNotification.TrialEndDate);
            string IS_NOTIFIED = nameof(TrialNotification.IsNotified);
            string REMINDER_DATE = nameof(TrialNotification.ReminderDate);
            string SENT_AT = nameof(TrialNotification.SentAt);
            string NOTIFICATION_TYPE_ID = nameof(TrialNotification.NotificationTypeId);

            if(values.Contains(ID)) {
                model.Id = ConvertTo<System.Guid>(values[ID]);
            }

            if(values.Contains(COMPANY_ID)) {
                model.CompanyId = values[COMPANY_ID] != null ? ConvertTo<System.Guid>(values[COMPANY_ID]) : (Guid?)null;
            }

            if(values.Contains(TRIAL_START_DATE)) {
                model.TrialStartDate = Convert.ToDateTime(values[TRIAL_START_DATE]);
            }

            if(values.Contains(TRIAL_END_DATE)) {
                model.TrialEndDate = Convert.ToDateTime(values[TRIAL_END_DATE]);
            }

            if(values.Contains(IS_NOTIFIED)) {
                model.IsNotified = Convert.ToBoolean(values[IS_NOTIFIED]);
            }

            if(values.Contains(REMINDER_DATE)) {
                model.ReminderDate = values[REMINDER_DATE] != null ? Convert.ToDateTime(values[REMINDER_DATE]) : (DateTime?)null;
            }

            if(values.Contains(SENT_AT)) {
                model.SentAt = values[SENT_AT] != null ? Convert.ToDateTime(values[SENT_AT]) : (DateTime?)null;
            }

            if(values.Contains(NOTIFICATION_TYPE_ID)) {
                model.NotificationTypeId = Convert.ToInt32(values[NOTIFICATION_TYPE_ID]);
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