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
using MedisatERP.Areas.NutritionCompanySystem.Models;
using Microsoft.Data.SqlClient;
using MedisatERP.Services;

namespace MedisatERP.Controllers
{
    [Route("api/[controller]/[action]")]
    public class AppointmentsAPIController : Controller
    {
        private NutritionSystemDbContext _nutritionSystemDbContext;
        private AdministratorSystemDbContext _administratorSystemDbContext;
        private IErrorCodeService _errorCodeService;

        public AppointmentsAPIController(NutritionSystemDbContext nutritionSystemDbContext, AdministratorSystemDbContext administratorSystemDbContext, IErrorCodeService errorCodeService) {
            _nutritionSystemDbContext = nutritionSystemDbContext;
            _administratorSystemDbContext = administratorSystemDbContext;
            _errorCodeService = errorCodeService;
        }

        [HttpGet]
        public IActionResult Get(DataSourceLoadOptions loadOptions, Guid companyId)
        {
            // Log the input parameters for debugging
            Console.WriteLine($"Get method called with companyId: {companyId}");

            try
            {
                // Fetch related data from shared context
                var clients = _administratorSystemDbContext.CompanyClients.ToList();
                var nutritionists = _administratorSystemDbContext.AspNetUsers.ToList();

                var appointments = _nutritionSystemDbContext.Appointments
                    .Include(a => a.Workplace)
                    .Where(a => a.CompanyId == companyId) // Filter by companyId first
                    .OrderBy(a => a.AppointmentId)
                    .AsEnumerable() // Switch to client-side evaluation
                    .Select(i => new
                    {
                        i.AppointmentId,
                        i.ClientId,
                        i.NutritionistId,
                        i.ScheduledDate,
                        i.WorkplaceId,
                        i.CompanyId,
                        i.Status,
                        i.Priority,
                        i.ReminderSent,
                        i.ReminderSentAt,
                        i.Notes,
                        i.CreatedAt,
                        i.UpdatedAt,
                        i.Duration,
                        Workplace = i.Workplace != null ? new
                        {
                            i.Workplace.Workplace
                        } : null,
                        Client = clients.FirstOrDefault(c => c.ClientId == i.ClientId) != null ? new
                        {
                            ClientName = clients.FirstOrDefault(c => c.ClientId == i.ClientId).ClientName,
                            Email = clients.FirstOrDefault(c => c.ClientId == i.ClientId).Email,
                            PhoneNumber = clients.FirstOrDefault(c => c.ClientId == i.ClientId).PhoneNumber
                        } : null,
                        Nutritionist = nutritionists.FirstOrDefault(n => n.Id == i.NutritionistId) != null ? new
                        {
                            UserName = nutritionists.FirstOrDefault(n => n.Id == i.NutritionistId).UserName,
                            Email = nutritionists.FirstOrDefault(n => n.Id == i.NutritionistId).Email,
                            PhoneNumber = nutritionists.FirstOrDefault(n => n.Id == i.NutritionistId).PhoneNumber
                        } : null
                    }).ToList(); // Convert to List for client-side evaluation

                // Debug the query before applying DataSourceLoader
                Console.WriteLine("Appointments query built. Applying DataSourceLoader...");

                // Apply filtering, sorting, and paging using DataSourceLoader
                var transformedData = DataSourceLoader.Load(appointments.AsQueryable(), loadOptions);

                // Serialize the retrieved object and log it
                var serializedData = JsonConvert.SerializeObject(transformedData, Formatting.Indented);
                Console.WriteLine($"Data transformation completed. Retrieved data: {serializedData}");

                // Return the transformed data as JSON
                return Json(transformedData);
            }
            catch (SqlException ex)
            {
                // Log the exception (consider using a logging framework)
                Console.WriteLine(ex);  // Replace with your logging mechanism
                return StatusCode(500, new { message = "A database error occurred. Please try again later." });
            }
            catch (InvalidOperationException ex)
            {
                // Log the exception
                Console.WriteLine(ex);  // Replace with your logging mechanism
                return StatusCode(500, new { message = "An internal server error occurred. Please try again later." });
            }
            catch (Exception ex)
            {
                // Log the exception message and stack trace for debugging purposes
                Console.WriteLine($"An error occurred: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");

                // Return a standardized error response
                return StatusCode(500, new { message = "An unexpected error occurred. Please try again later.", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post(string values)
        {
            try
            {
                // Log the received values
                Console.WriteLine($"Received values: {values}");

                var model = new Appointment();
                var valuesDict = JsonConvert.DeserializeObject<IDictionary>(values);
                PopulateModel(model, valuesDict);

                // Ensure non-nullable fields are set
                model.AppointmentId = Guid.NewGuid(); // Generate a new GUID for the AppointmentId
                model.CreatedAt = DateTime.UtcNow; // Set CreatedAt to the current UTC time
                model.UpdatedAt = DateTime.UtcNow; // Initialize UpdatedAt to the current UTC time

                // Convert CompanyId from string to Guid
                if (valuesDict.Contains("CompanyId") && Guid.TryParse(valuesDict["CompanyId"].ToString(), out Guid companyId))
                {
                    model.CompanyId = companyId;
                }

                // Log the populated model
                var modelJson = JsonConvert.SerializeObject(model, Formatting.Indented);
                Console.WriteLine($"Populated model: {modelJson}");

                if (!TryValidateModel(model))
                {
                    var errorMessage = GetFullErrorMessage(ModelState);
                    Console.WriteLine($"Validation error: {errorMessage}");
                    return BadRequest(errorMessage);
                }

                var result = _nutritionSystemDbContext.Appointments.Add(model);
                await _nutritionSystemDbContext.SaveChangesAsync();

                // Log the saved appointment ID
                Console.WriteLine($"Appointment created with ID: {result.Entity.AppointmentId}");

                return Json(new { success = true, message = $"Appointment scheduled successfully!" });
            }
            catch (JsonSerializationException ex)
            {
                // Log the exception
                Console.WriteLine($"JSON serialization error: {ex.Message}");
                return BadRequest(new { message = "Invalid input format. Please check your data and try again." });
            }
            catch (SqlException ex)
            {
                // Log the exception (consider using a logging framework)
                Console.WriteLine(ex);  // Replace with your logging mechanism
                return StatusCode(500, new { message = "A database error occurred. Please try again later." });
            }
            catch (InvalidOperationException ex)
            {
                // Log the exception
                Console.WriteLine(ex);  // Replace with your logging mechanism
                return StatusCode(500, new { message = "An internal server error occurred. Please try again later." });
            }
            catch (Exception ex)
            {
                // Log the exception message and stack trace for debugging purposes
                Console.WriteLine($"An error occurred: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");

                // Return a standardized error response
                return StatusCode(500, new { message = "An unexpected error occurred. Please try again later.", error = ex.Message });
            }
        }





        [HttpPut]
        public async Task<IActionResult> Put(Guid key, string values) {
            var model = await _nutritionSystemDbContext.Appointments.FirstOrDefaultAsync(item => item.AppointmentId == key);
            if(model == null)
                return StatusCode(409, "Object not found");

            var valuesDict = JsonConvert.DeserializeObject<IDictionary>(values);
            PopulateModel(model, valuesDict);

            if(!TryValidateModel(model))
                return BadRequest(GetFullErrorMessage(ModelState));

            await _nutritionSystemDbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete]
        public async Task Delete(Guid key) {
            var model = await _nutritionSystemDbContext.Appointments.FirstOrDefaultAsync(item => item.AppointmentId == key);

            _nutritionSystemDbContext.Appointments.Remove(model);
            await _nutritionSystemDbContext.SaveChangesAsync();
        }


        //[HttpGet]
        //public async Task<IActionResult> CompanyClientsLookup(DataSourceLoadOptions loadOptions) {
        //    var lookup = from i in _nutritionSystemDbContext.CompanyClients
        //                 orderby i.ClientName
        //                 select new {
        //                     Value = i.ClientId,
        //                     Text = i.ClientName
        //                 };
        //    return Json(await DataSourceLoader.LoadAsync(lookup, loadOptions));
        //}

        //[HttpGet]
        //public async Task<IActionResult> AspNetUsersLookup(DataSourceLoadOptions loadOptions) {
        //    var lookup = from i in _nutritionSystemDbContext.AspNetUsers
        //                 orderby i.UserName
        //                 select new {
        //                     Value = i.Id,
        //                     Text = i.UserName
        //                 };
        //    return Json(await DataSourceLoader.LoadAsync(lookup, loadOptions));
        //}

        [HttpGet]
        public async Task<IActionResult> WorkplaceLookupsLookup(DataSourceLoadOptions loadOptions) {
            var lookup = from i in _nutritionSystemDbContext.WorkplaceLookups
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