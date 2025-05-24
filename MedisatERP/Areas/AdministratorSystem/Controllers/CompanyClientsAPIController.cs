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

namespace MedisatERP.Controllers
{
    [Route("api/[controller]/[action]")]
    public class CompanyClientsAPIController : Controller
    {
        private ApplicationDbContext _context;
        private readonly ExceptionHandlerService _exceptionHandlerService;
        private readonly ILogger<CompanyClientsAPIController> _logger;

        public CompanyClientsAPIController(ApplicationDbContext context, ExceptionHandlerService exceptionHandlerService, ILogger<CompanyClientsAPIController> logger ) {
            _context = context;
            _exceptionHandlerService = exceptionHandlerService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Get(DataSourceLoadOptions loadOptions, Guid companyId) {
            try
            {
                var companyclients = _context.CompanyClients.Select(i => new {
                    i.Id,
                    i.CompanyId,
                    i.ClientName,
                    i.DateOfBirth,
                    i.Gender,
                    i.Email,
                    i.PhoneNumber,
                    i.EmergencyContactName,
                    i.EmergencyContactPhone,
                    i.MaritalStatus,
                    i.Nationality,
                    i.CreatedAt,
                    i.UpdatedAt,
                    i.Street,
                    i.City,
                    i.State,
                    i.PostalCode,
                    i.Country,
                    i.EmergencyContactRelationship
                }).Where(a => a.CompanyId == companyId).OrderBy(a => a.Id);

                return Json(await DataSourceLoader.LoadAsync(companyclients, loadOptions));
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
                var model = new CompanyClient();
                var valuesDict = JsonConvert.DeserializeObject<IDictionary>(values);
                PopulateModel(model, valuesDict);

                if (!TryValidateModel(model))
                {
                    var errors = GetFullErrorMessage(ModelState);
                    _logger.LogWarning($"Validation failed: {errors}");
                    return BadRequest(errors);
                }

                if (model.CreatedAt == DateTime.MinValue)
                {
                    model.CreatedAt = DateTime.Now;
                }
                
                var result = _context.CompanyClients.Add(model);
                await _context.SaveChangesAsync();

                return Json(new { result.Entity.Id });
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
                var model = await _context.CompanyClients.FirstOrDefaultAsync(item => item.Id == key);
                if (model == null)
                    return StatusCode(409, "Object not found");


                var valuesDict = JsonConvert.DeserializeObject<IDictionary>(values);
                model.UpdatedAt = DateTime.Now;
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
                var model = await _context.CompanyClients.FirstOrDefaultAsync(item => item.Id == key);

                if (model == null)
                {
                     NotFound("No record found with Id");
                }

                _context.CompanyClients.Remove(model);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _exceptionHandlerService.HandleException(ex, this);
            }
        }

        private void PopulateModel(CompanyClient model, IDictionary values) {
            string ID = nameof(CompanyClient.Id);
            string COMPANY_ID = nameof(CompanyClient.CompanyId);
            string CLIENT_NAME = nameof(CompanyClient.ClientName);
            string DATE_OF_BIRTH = nameof(CompanyClient.DateOfBirth);
            string GENDER = nameof(CompanyClient.Gender);
            string EMAIL = nameof(CompanyClient.Email);
            string PHONE_NUMBER = nameof(CompanyClient.PhoneNumber);
            string EMERGENCY_CONTACT_NAME = nameof(CompanyClient.EmergencyContactName);
            string EMERGENCY_CONTACT_PHONE = nameof(CompanyClient.EmergencyContactPhone);
            string MARITAL_STATUS = nameof(CompanyClient.MaritalStatus);
            string NATIONALITY = nameof(CompanyClient.Nationality);
            string CREATED_AT = nameof(CompanyClient.CreatedAt);
            string UPDATED_AT = nameof(CompanyClient.UpdatedAt);
            string STREET = nameof(CompanyClient.Street);
            string CITY = nameof(CompanyClient.City);
            string STATE = nameof(CompanyClient.State);
            string POSTAL_CODE = nameof(CompanyClient.PostalCode);
            string COUNTRY = nameof(CompanyClient.Country);
            string EMERGENCY_CONTACT_RELATIONSHIP = nameof(CompanyClient.EmergencyContactRelationship);

            if(values.Contains(ID)) {
                model.Id = ConvertTo<System.Guid>(values[ID]);
            }

            if(values.Contains(COMPANY_ID)) {
                model.CompanyId = ConvertTo<System.Guid>(values[COMPANY_ID]);
            }

            if(values.Contains(CLIENT_NAME)) {
                model.ClientName = Convert.ToString(values[CLIENT_NAME]);
            }

            if(values.Contains(DATE_OF_BIRTH)) {
                model.DateOfBirth = Convert.ToDateTime(values[DATE_OF_BIRTH]);
            }

            if(values.Contains(GENDER)) {
                model.Gender = Convert.ToString(values[GENDER]);
            }

            if(values.Contains(EMAIL)) {
                model.Email = Convert.ToString(values[EMAIL]);
            }

            if(values.Contains(PHONE_NUMBER)) {
                model.PhoneNumber = Convert.ToString(values[PHONE_NUMBER]);
            }

            if(values.Contains(EMERGENCY_CONTACT_NAME)) {
                model.EmergencyContactName = Convert.ToString(values[EMERGENCY_CONTACT_NAME]);
            }

            if(values.Contains(EMERGENCY_CONTACT_PHONE)) {
                model.EmergencyContactPhone = Convert.ToString(values[EMERGENCY_CONTACT_PHONE]);
            }

            if(values.Contains(MARITAL_STATUS)) {
                model.MaritalStatus = Convert.ToString(values[MARITAL_STATUS]);
            }

            if(values.Contains(NATIONALITY)) {
                model.Nationality = Convert.ToString(values[NATIONALITY]);
            }

            if(values.Contains(CREATED_AT)) {
                model.CreatedAt = Convert.ToDateTime(values[CREATED_AT]);
            }

            if(values.Contains(UPDATED_AT)) {
                model.UpdatedAt = values[UPDATED_AT] != null ? Convert.ToDateTime(values[UPDATED_AT]) : (DateTime?)null;
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

            if(values.Contains(EMERGENCY_CONTACT_RELATIONSHIP)) {
                model.EmergencyContactRelationship = Convert.ToString(values[EMERGENCY_CONTACT_RELATIONSHIP]);
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