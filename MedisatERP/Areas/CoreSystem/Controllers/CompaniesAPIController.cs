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
using Humanizer;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Newtonsoft.Json.Linq;

namespace MedisatERP.Controllers
{
    [Route("api/[controller]/[action]")]
    public class CompaniesAPIController : Controller
    {
        private MedisatErpDbContext _context;
        private readonly HttpClient _client;

        public CompaniesAPIController(MedisatErpDbContext context, HttpClient client) {
            _context = context;
            _client = client;
        }

        [HttpGet]
        public async Task<IActionResult> Get(DataSourceLoadOptions loadOptions)
        {
            var companies = _context.Companies
                                    .Include(c => c.Address)  // Ensure Address data is eagerly loaded
                                    .Select(i => new {
                                        i.CompanyId,
                                        i.CompanyName,
                                        i.ContactPerson,
                                        i.CompanyEmail,
                                        i.CompanyPhone,
                                        i.ExpDate,
                                        i.ApiCode,
                                        i.CompanyStatus,
                                        i.SubscriptionAmount,
                                        i.CompanyInitials,
                                        i.SmsAccount,
                                        i.PayAccount,
                                        i.PayBank,
                                        i.PayAccountName,
                                        i.Motto,
                                        i.CompanyType,
                                        i.CreatedAt,
                                        Address = new  {
                                            i.Address.AddressId,
                                            i.Address.Street,
                                            i.Address.City,
                                            i.Address.State,
                                            i.Address.PostalCode,
                                            i.Address.Country
                                        }
                                    });

            // Log raw data before any transformation
            var rawData = await companies.ToListAsync();
            string rawDataJson = JsonConvert.SerializeObject(rawData, Formatting.Indented);

            Console.WriteLine("Raw Data Before DataSourceLoader:");
            Console.WriteLine(rawDataJson);  // Log the data before applying any filters, sort, or pagination

            // Apply filtering, sorting, and paging
            var transformedData = await DataSourceLoader.LoadAsync(companies, loadOptions);

            // Log the data after DataSourceLoader processes it
            string transformedDataJson = JsonConvert.SerializeObject(transformedData, Formatting.Indented);
            Console.WriteLine("Transformed Data After DataSourceLoader:");
            Console.WriteLine(transformedDataJson);  // Log the transformed data that will be returned to the UI

            return Json(transformedData); // Return the processed data
        }


        //[HttpGet]
        //public async Task<IActionResult> Get(DataSourceLoadOptions loadOptions)
        //{
        //    var companies = _context.Companies
        //                            .Include(c => c.Address)  // Ensure Address data is eagerly loaded
        //                            .Select(i => new {
        //                                i.CompanyId,
        //                                i.CompanyName,
        //                                i.ContactPerson,
        //                                i.CompanyEmail,
        //                                i.CompanyPhone,
        //                                i.ExpDate,
        //                                i.ApiCode,
        //                                i.CompanyStatus,
        //                                i.SubscriptionAmount,
        //                                i.CompanyInitials,
        //                                i.SmsAccount,
        //                                i.PayAccount,
        //                                i.PayBank,
        //                                i.PayAccountName,
        //                                i.Motto,
        //                                i.CompanyType,
        //                                i.AddressId,
        //                                i.CreatedAt,
        //                                i.Address.Street,
        //                                i.Address.City,
        //                                i.Address.State,
        //                                i.Address.PostalCode,
        //                                i.Address.Country
        //                            });
        //    var rawData = await companies.ToListAsync(); // Execute the query and get the actual data
        //    string DataJson = JsonConvert.SerializeObject(rawData, Formatting.Indented);

        //    // Log the serialized data to the console
        //    Console.WriteLine("Extracted Data:");
        //    Console.WriteLine(DataJson);

        //    // Use DataSourceLoader to apply filtering, sorting, and paging
        //    return Json(await DataSourceLoader.LoadAsync(companies, loadOptions));
        //}


        //[HttpGet]
        //public async Task<IActionResult> Get(DataSourceLoadOptions loadOptions) {
        //    var companies = _context.Companies.Select(i => new {
        //        i.CompanyId,
        //        i.CompanyName,
        //        i.ContactPerson,
        //        i.CompanyEmail,
        //        i.CompanyPhone,
        //        i.ExpDate,
        //        i.ApiCode,
        //        i.CompanyStatus,
        //        i.SubscriptionAmount,
        //        i.CompanyInitials,
        //        i.SmsAccount,
        //        i.PayAccount,
        //        i.PayBank,
        //        i.PayAccountName,
        //        i.Motto,
        //        i.CompanyType,
        //        i.AddressId,
        //        i.CreatedAt, 
        //        i.Address.Street,
        //        i.Address.City,
        //        i.Address.State,
        //        i.Address.PostalCode,
        //        i.Address.Country
        //    });

        //    // If underlying data is a large SQL table, specify PrimaryKey and PaginateViaPrimaryKey.
        //    // This can make SQL execution plans more efficient.
        //    // For more detailed information, please refer to this discussion: https://github.com/DevExpress/DevExtreme.AspNet.Data/issues/336.
        //    // loadOptions.PrimaryKey = new[] { "CompanyId" };
        //    // loadOptions.PaginateViaPrimaryKey = true;

        //    return Json(await DataSourceLoader.LoadAsync(companies, loadOptions));
        //}


        [HttpPost]
        public async Task<IActionResult> Post(string values)
        {
            try
            {
                // Log the incoming request values
                Console.WriteLine("Received request values: " + values);

                var model = new Company();
                var valuesDict = JsonConvert.DeserializeObject<IDictionary<string, object>>(values); // Corrected the type here

                // Log after deserialization
                Console.WriteLine("Deserialized request values into dictionary.");

                // Extract the address data, which is nested under "Address"
                var addressData = valuesDict.ContainsKey("Address") ? valuesDict["Address"] as JObject : null;

                // Log the extracted address data
                Console.WriteLine("Extracted address data: " + JsonConvert.SerializeObject(addressData));

                // Populate the company model with the remaining values (excluding address data)
                var companyData = valuesDict
                    .Where(kv => kv.Key != "Address")  // Exclude the address data
                    .ToDictionary(kv => kv.Key, kv => kv.Value);

                // Log the company data being populated
                Console.WriteLine("Extracted company data: " + JsonConvert.SerializeObject(companyData));

                PopulateModel(model, companyData);

                // Ensure that Address is initialized to avoid NullReferenceException
                if (model.Address == null)
                {
                    model.Address = new CompanyAddress();
                    Console.WriteLine("Initialized new Company with Address.");
                }

                // If we have address data, attempt to populate the address model
                if (addressData != null)
                {
                    Console.WriteLine("Address data found, populating address model...");

                    // Populate the address model with data from the extracted addressData JObject
                    model.Address.Street = addressData["Street"]?.ToString();
                    model.Address.City = addressData["City"]?.ToString();
                    model.Address.State = addressData["State"]?.ToString();
                    model.Address.PostalCode = addressData["PostalCode"]?.ToString();
                    model.Address.Country = addressData["Country"]?.ToString();

                    // Log the populated address model before sending to API
                    Console.WriteLine("Populated address model: " + JsonConvert.SerializeObject(model.Address));

                    // If AddressId is empty, create a new address
                    if (model.AddressId == Guid.Empty)
                    {
                        var newAddressData = new
                        {
                            AddressId = Guid.NewGuid(),
                            Street = model.Address.Street,
                            City = model.Address.City,
                            State = model.Address.State,
                            PostalCode = model.Address.PostalCode,
                            Country = model.Address.Country
                        };

                        // Log the address data to be sent to the address creation API
                        Console.WriteLine("Preparing to create a new address: " + JsonConvert.SerializeObject(newAddressData));

                        var content = JsonContent.Create(newAddressData);

                        Console.WriteLine("Preparing to create a new address: " + JsonConvert.SerializeObject(content));

                        // Ensure BaseAddress is set before making the API request
                        if (_client.BaseAddress == null)
                        {
                            _client.BaseAddress = new Uri("http://localhost:59245/"); // Make sure this is the correct base URI
                            Console.WriteLine("Setting BaseAddress to: " + _client.BaseAddress);
                        }

                        // Make the request to create the address
                        var addressResponse = await _client.PostAsync("api/CompanyAddressesAPI/Post", content);

                        if (addressResponse.IsSuccessStatusCode)
                        {
                            var addressResponseContent = await addressResponse.Content.ReadFromJsonAsync<JsonElement>();
                            Console.WriteLine("Address creation response: " + JsonConvert.SerializeObject(addressResponseContent));

                            // Check if AddressId was returned in the response
                            if (addressResponseContent.TryGetProperty("AddressId", out var addressIdElement))
                            {
                                model.AddressId = addressIdElement.GetGuid();
                                Console.WriteLine("Successfully received AddressId: " + model.AddressId);
                            }
                            else
                            {
                                Console.WriteLine("Error: AddressId not found in response.");
                                return BadRequest("AddressId not found in response.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Error: Failed to create address. Status Code: " + addressResponse.StatusCode);
                            return BadRequest("Failed to create address.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("AddressId already exists, using existing AddressId: " + model.AddressId);
                    }
                }
                else
                {
                    Console.WriteLine("No address data found in request.");
                }

                // Save the company with the address
                Console.WriteLine("Saving the company with AddressId: " + model.AddressId);

                // Get time now the modle is created
                if (model.CreatedAt == null)
                {
                    Console.WriteLine("Creating Date the model has been created");
                    model.CreatedAt = DateTime.Now;
                }
                

                _context.Companies.Add(model);
                await _context.SaveChangesAsync();

                // Log success
                Console.WriteLine("Company saved successfully. CompanyId: " + model.CompanyId);

                return Json(new { model.CompanyId }); // Return the CompanyId or relevant response
            }
            catch (Exception ex)
            {
                // Log any unexpected exceptions
                Console.WriteLine("Error occurred: " + ex.Message);
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpPut]
        public async Task<IActionResult> Put(Guid key, string values) {
            var model = await _context.Companies.FirstOrDefaultAsync(item => item.CompanyId == key);
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
            var model = await _context.Companies.FirstOrDefaultAsync(item => item.CompanyId == key);

            _context.Companies.Remove(model);
            await _context.SaveChangesAsync();
        }


        private void PopulateModel(Company model, IDictionary values)
        {
            string COMPANY_ID = nameof(Company.CompanyId);
            string COMPANY_NAME = nameof(Company.CompanyName);
            string CONTACT_PERSON = nameof(Company.ContactPerson);
            string COMPANY_EMAIL = nameof(Company.CompanyEmail);
            string COMPANY_PHONE = nameof(Company.CompanyPhone);
            string EXP_DATE = nameof(Company.ExpDate);
            string API_CODE = nameof(Company.ApiCode);
            string COMPANY_STATUS = nameof(Company.CompanyStatus);
            string SUBSCRIPTION_AMOUNT = nameof(Company.SubscriptionAmount);
            string COMPANY_INITIALS = nameof(Company.CompanyInitials);
            string SMS_ACCOUNT = nameof(Company.SmsAccount);
            string PAY_ACCOUNT = nameof(Company.PayAccount);
            string PAY_BANK = nameof(Company.PayBank);
            string PAY_ACCOUNT_NAME = nameof(Company.PayAccountName);
            string MOTTO = nameof(Company.Motto);
            string COMPANY_TYPE = nameof(Company.CompanyType);
            string ADDRESS_ID = nameof(Company.AddressId);
            string CREATED_AT = nameof(Company.CreatedAt);

            // Populate basic fields for the Company model
            if (values.Contains(COMPANY_ID))
            {
                model.CompanyId = ConvertTo<System.Guid>(values[COMPANY_ID]);
            }

            if (values.Contains(COMPANY_NAME))
            {
                model.CompanyName = Convert.ToString(values[COMPANY_NAME]);
            }

            if (values.Contains(CONTACT_PERSON))
            {
                model.ContactPerson = Convert.ToString(values[CONTACT_PERSON]);
            }

            if (values.Contains(COMPANY_EMAIL))
            {
                model.CompanyEmail = Convert.ToString(values[COMPANY_EMAIL]);
            }

            if (values.Contains(COMPANY_PHONE))
            {
                model.CompanyPhone = Convert.ToString(values[COMPANY_PHONE]);
            }

            if (values.Contains(EXP_DATE))
            {
                model.ExpDate = values[EXP_DATE] != null ? Convert.ToDateTime(values[EXP_DATE]) : (DateTime?)null;
            }

            if (values.Contains(API_CODE))
            {
                model.ApiCode = Convert.ToString(values[API_CODE]);
            }

            if (values.Contains(COMPANY_STATUS))
            {
                model.CompanyStatus = Convert.ToString(values[COMPANY_STATUS]);
            }

            if (values.Contains(SUBSCRIPTION_AMOUNT))
            {
                model.SubscriptionAmount = values[SUBSCRIPTION_AMOUNT] != null ? Convert.ToDecimal(values[SUBSCRIPTION_AMOUNT], CultureInfo.InvariantCulture) : (decimal?)null;
            }

            if (values.Contains(COMPANY_INITIALS))
            {
                model.CompanyInitials = Convert.ToString(values[COMPANY_INITIALS]);
            }

            if (values.Contains(SMS_ACCOUNT))
            {
                model.SmsAccount = Convert.ToString(values[SMS_ACCOUNT]);
            }

            if (values.Contains(PAY_ACCOUNT))
            {
                model.PayAccount = Convert.ToString(values[PAY_ACCOUNT]);
            }

            if (values.Contains(PAY_BANK))
            {
                model.PayBank = Convert.ToString(values[PAY_BANK]);
            }

            if (values.Contains(PAY_ACCOUNT_NAME))
            {
                model.PayAccountName = Convert.ToString(values[PAY_ACCOUNT_NAME]);
            }

            if (values.Contains(MOTTO))
            {
                model.Motto = Convert.ToString(values[MOTTO]);
            }

            if (values.Contains(COMPANY_TYPE))
            {
                model.CompanyType = Convert.ToString(values[COMPANY_TYPE]);
            }

            if (values.Contains(ADDRESS_ID))
            {
                model.AddressId = ConvertTo<System.Guid>(values[ADDRESS_ID]);
            }

            if (values.Contains(CREATED_AT))
            {
                model.CreatedAt = values[CREATED_AT] != null ? Convert.ToDateTime(values[CREATED_AT]) : (DateTime?)null;
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