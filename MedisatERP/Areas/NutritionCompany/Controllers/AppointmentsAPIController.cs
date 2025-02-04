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
using MedisatERP.Areas.NutritionCompany.Models;

namespace MedisatERP.Controllers
{
    [Route("api/[controller]/[action]")]
    public class AppointmentsAPIController : Controller
    {
        private MedisatErpDbContext _context;

        public AppointmentsAPIController(MedisatErpDbContext context) {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get(DataSourceLoadOptions loadOptions) {
            var appointments = _context.Appointments.Select(i => new {
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
                i.Duration
            });

            // If underlying data is a large SQL table, specify PrimaryKey and PaginateViaPrimaryKey.
            // This can make SQL execution plans more efficient.
            // For more detailed information, please refer to this discussion: https://github.com/DevExpress/DevExtreme.AspNet.Data/issues/336.
            // loadOptions.PrimaryKey = new[] { "AppointmentId" };
            // loadOptions.PaginateViaPrimaryKey = true;

            return Json(await DataSourceLoader.LoadAsync(appointments, loadOptions));
        }

        [HttpPost]
        public async Task<IActionResult> Post(string values) {
            var model = new Appointment();
            var valuesDict = JsonConvert.DeserializeObject<IDictionary>(values);
            PopulateModel(model, valuesDict);

            if(!TryValidateModel(model))
                return BadRequest(GetFullErrorMessage(ModelState));

            var result = _context.Appointments.Add(model);
            await _context.SaveChangesAsync();

            return Json(new { result.Entity.AppointmentId });
        }

        [HttpPut]
        public async Task<IActionResult> Put(Guid key, string values) {
            var model = await _context.Appointments.FirstOrDefaultAsync(item => item.AppointmentId == key);
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
            var model = await _context.Appointments.FirstOrDefaultAsync(item => item.AppointmentId == key);

            _context.Appointments.Remove(model);
            await _context.SaveChangesAsync();
        }


        [HttpGet]
        public async Task<IActionResult> CompanyClientsLookup(DataSourceLoadOptions loadOptions) {
            var lookup = from i in _context.CompanyClients
                         orderby i.ClientName
                         select new {
                             Value = i.ClientId,
                             Text = i.ClientName
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