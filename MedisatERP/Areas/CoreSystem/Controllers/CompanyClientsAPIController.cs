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
using MedisatERP.Areas.CoreSystem.Models;

namespace MedisatERP.Controllers
{
    [Route("api/[controller]/[action]")]
    public class CompanyClientsAPIController : Controller
    {
        private MedisatErpDbContext _context;

        public CompanyClientsAPIController(MedisatErpDbContext context) {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get(DataSourceLoadOptions loadOptions, Guid companyId) {
            var companyclients = _context.CompanyClients.Select(i => new
            {
                i.ClientId,
                i.CompanyId,
                i.ClientName,
                i.DateOfBirth,
                i.Gender,
                i.Email,
                i.PhoneNumber,
                i.AddressId,
                i.EmergencyContactName,
                i.EmergencyContactPhone,
                i.MaritalStatus,
                i.Nationality,
                i.CreatedAt,
                i.UpdatedAt,
            }).Where(a => a.CompanyId == companyId).OrderBy(a => a.ClientName);

            // If underlying data is a large SQL table, specify PrimaryKey and PaginateViaPrimaryKey.
            // This can make SQL execution plans more efficient.
            // For more detailed information, please refer to this discussion: https://github.com/DevExpress/DevExtreme.AspNet.Data/issues/336.
            // loadOptions.PrimaryKey = new[] { "ClientId" };
            // loadOptions.PaginateViaPrimaryKey = true;

            return Json(await DataSourceLoader.LoadAsync(companyclients, loadOptions));
        }

        [HttpPost]
        public async Task<IActionResult> Post(string values) {
            var model = new CompanyClient();
            var valuesDict = JsonConvert.DeserializeObject<IDictionary>(values);
            PopulateModel(model, valuesDict);

            if(!TryValidateModel(model))
                return BadRequest(GetFullErrorMessage(ModelState));

            var result = _context.CompanyClients.Add(model);
            await _context.SaveChangesAsync();

            return Json(new { result.Entity.ClientId });
        }

        [HttpPut]
        public async Task<IActionResult> Put(Guid key, string values) {
            var model = await _context.CompanyClients.FirstOrDefaultAsync(item => item.ClientId == key);
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
            var model = await _context.CompanyClients.FirstOrDefaultAsync(item => item.ClientId == key);

            _context.CompanyClients.Remove(model);
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

        private void PopulateModel(CompanyClient model, IDictionary values) {
            string CLIENT_ID = nameof(CompanyClient.ClientId);
            string COMPANY_ID = nameof(CompanyClient.CompanyId);
            string CLIENT_NAME = nameof(CompanyClient.ClientName);
            string DATE_OF_BIRTH = nameof(CompanyClient.DateOfBirth);
            string GENDER = nameof(CompanyClient.Gender);
            string EMAIL = nameof(CompanyClient.Email);
            string PHONE_NUMBER = nameof(CompanyClient.PhoneNumber);
            string ADDRESS_ID = nameof(CompanyClient.AddressId);
            string EMERGENCY_CONTACT_NAME = nameof(CompanyClient.EmergencyContactName);
            string EMERGENCY_CONTACT_PHONE = nameof(CompanyClient.EmergencyContactPhone);
            string MARITAL_STATUS = nameof(CompanyClient.MaritalStatus);
            string NATIONALITY = nameof(CompanyClient.Nationality);
            string CREATED_AT = nameof(CompanyClient.CreatedAt);
            string UPDATED_AT = nameof(CompanyClient.UpdatedAt);

            if(values.Contains(CLIENT_ID)) {
                model.ClientId = ConvertTo<System.Guid>(values[CLIENT_ID]);
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

            if(values.Contains(ADDRESS_ID)) {
                model.AddressId = ConvertTo<System.Guid>(values[ADDRESS_ID]);
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