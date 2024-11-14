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
    public class CompanyAddressesAPIController : Controller
    {
        private MedisatErpDbContext _context;

        public CompanyAddressesAPIController(MedisatErpDbContext context) {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get(DataSourceLoadOptions loadOptions) {
            var companyaddresses = _context.CompanyAddresses.Select(i => new {
                i.AddressId,
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

            return Json(await DataSourceLoader.LoadAsync(companyaddresses, loadOptions));
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CompanyAddress model)
        {
            // Log the received model data
            Console.WriteLine("Received CompanyAddress model: " + JsonConvert.SerializeObject(model));

            // Ensure the model is properly populated
            if (model == null)
            {
                Console.WriteLine("Error: Invalid model data.");
                return BadRequest("Invalid model data.");
            }

            // Validate the model
            if (!TryValidateModel(model))
            {
                // Log validation errors
                Console.WriteLine("Error: Model validation failed. " + GetFullErrorMessage(ModelState));
                return BadRequest(GetFullErrorMessage(ModelState));
            }

            // Log that we are about to add the model to the database
            Console.WriteLine("Adding new CompanyAddress to database.");

            // Add the new CompanyAddress to the context
            _context.CompanyAddresses.Add(model);
            await _context.SaveChangesAsync();

            // Log the result after saving
            Console.WriteLine("Saved CompanyAddress with AddressId: " + model.AddressId);

            // Return the newly created AddressId
            return Json(new { AddressId = model.AddressId });
        }


        [HttpPut]
        public async Task<IActionResult> Put(Guid key, string values) {
            var model = await _context.CompanyAddresses.FirstOrDefaultAsync(item => item.AddressId == key);
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
            var model = await _context.CompanyAddresses.FirstOrDefaultAsync(item => item.AddressId == key);

            _context.CompanyAddresses.Remove(model);
            await _context.SaveChangesAsync();
        }


        private void PopulateModel(CompanyAddress model, IDictionary values) {
            string ADDRESS_ID = nameof(CompanyAddress.AddressId);
            string STREET = nameof(CompanyAddress.Street);
            string CITY = nameof(CompanyAddress.City);
            string STATE = nameof(CompanyAddress.State);
            string POSTAL_CODE = nameof(CompanyAddress.PostalCode);
            string COUNTRY = nameof(CompanyAddress.Country);

            if(values.Contains(ADDRESS_ID)) {
                model.AddressId = ConvertTo<System.Guid>(values[ADDRESS_ID]);
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