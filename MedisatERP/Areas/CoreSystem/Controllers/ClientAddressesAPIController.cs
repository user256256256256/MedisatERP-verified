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
    public class ClientAddressesAPIController : Controller
    {
        private MedisatErpDbContext _context;

        public ClientAddressesAPIController(MedisatErpDbContext context) {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get(DataSourceLoadOptions loadOptions) {
            var clientaddresses = _context.ClientAddresses.Select(i => new {
                i.AddressId,
                i.ClientId,
                i.Street,
                i.City,
                i.State,
                i.PostalCode,
                i.Country
            });

            // If underlying data is a large SQL table, specify PrimaryKey and PaginateViaPrimaryKey.
            // This can make SQL execution plans more efficient.
            // For more detailed information, please refer to this discussion: https://github.com/DevExpress/DevExtreme.AspNet.Data/issues/336.
            // loadOptions.PrimaryKey = new[] { "AddressId" };
            // loadOptions.PaginateViaPrimaryKey = true;

            return Json(await DataSourceLoader.LoadAsync(clientaddresses, loadOptions));
        }

        [HttpPost]
        public async Task<IActionResult> Post(string values) {
            var model = new ClientAddress();
            var valuesDict = JsonConvert.DeserializeObject<IDictionary>(values);
            PopulateModel(model, valuesDict);

            if(!TryValidateModel(model))
                return BadRequest(GetFullErrorMessage(ModelState));

            var result = _context.ClientAddresses.Add(model);
            await _context.SaveChangesAsync();

            return Json(new { result.Entity.AddressId });
        }

        [HttpPut]
        public async Task<IActionResult> Put(Guid key, string values) {
            var model = await _context.ClientAddresses.FirstOrDefaultAsync(item => item.AddressId == key);
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
            var model = await _context.ClientAddresses.FirstOrDefaultAsync(item => item.AddressId == key);

            _context.ClientAddresses.Remove(model);
            await _context.SaveChangesAsync();
        }


        private void PopulateModel(ClientAddress model, IDictionary values) {
            string ADDRESS_ID = nameof(ClientAddress.AddressId);
            string CLIENT_ID = nameof(ClientAddress.ClientId);
            string STREET = nameof(ClientAddress.Street);
            string CITY = nameof(ClientAddress.City);
            string STATE = nameof(ClientAddress.State);
            string POSTAL_CODE = nameof(ClientAddress.PostalCode);
            string COUNTRY = nameof(ClientAddress.Country);

            if(values.Contains(ADDRESS_ID)) {
                model.AddressId = ConvertTo<System.Guid>(values[ADDRESS_ID]);
            }

            if(values.Contains(CLIENT_ID)) {
                model.ClientId = ConvertTo<System.Guid>(values[CLIENT_ID]);
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