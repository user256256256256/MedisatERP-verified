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
    public class PaymentsAPIController : Controller
    {
        private MedisatErpDbContext _context;

        public PaymentsAPIController(MedisatErpDbContext context) {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get(DataSourceLoadOptions loadOptions) {
            var payments = _context.Payments
                .Include(c => c.Subscription)
                .Select(i => new {
                i.Id,
                i.SubscriptionId,
                i.PaymentDate,
                i.PaymentStatusId,
                i.PaymentMethodId,
                i.TransactionId,
                i.IsRefunded,
                PaymentMethod = new
                {
                    i.PaymentMethod.Id,
                    i.PaymentMethod.Method
                },
                PaymentStatus = new
                {
                    i.PaymentStatus.Id,
                    i.PaymentStatus.Status
                },
                Subscription = new
                {
                    Company = new
                    {
                        i.Subscription.Company.CompanyId,
                        i.Subscription.Company.CompanyName,
                        i.Subscription.Company.CompanyPhone,
                        i.Subscription.Company.CompanyEmail
                    },

                    SubscriptionPlan = new
                    {
                        i.Subscription.SubscriptionPlan.Description,
                        i.Subscription.SubscriptionPlan.Duration,
                        PlanName = new
                        {
                            i.Subscription.SubscriptionPlan.PlanName.Id,
                            i.Subscription.SubscriptionPlan.PlanName.PlanName,
                            i.Subscription.SubscriptionPlan.PlanName.Price,                           
                        }
                    }
                }
            });

            // Debug the query before applying DataSourceLoader
            Console.WriteLine("Payments query built. Applying DataSourceLoader...");

            // Apply filtering, sorting, and paging using DataSourceLoader
            var transformedData = await DataSourceLoader.LoadAsync(payments, loadOptions);

            // Serialize the retrieved object and log it
            var serializedData = JsonConvert.SerializeObject(transformedData, Formatting.Indented);
            Console.WriteLine($"Data transformation completed. Retrieved data: {serializedData}");

            // Return the transformed data as JSON
            return Json(transformedData);
        }

        [HttpGet]
        public async Task<IActionResult> GetPaymentsInfo(DataSourceLoadOptions loadOptions, Guid companyId)
        {
            // Log the input parameters for debugging
            Console.WriteLine($"Get method called with companyId: {companyId}");

            var payments = _context.Payments
                .Include(c => c.Subscription)
                .Select(i => new {
                    i.Id,
                    i.SubscriptionId,
                    i.PaymentDate,
                    i.PaymentStatusId,
                    i.PaymentMethodId,
                    i.TransactionId,
                    i.IsRefunded,
                    PaymentMethod = new
                    {
                        i.PaymentMethod.Id,
                        i.PaymentMethod.Method
                    },
                    PaymentStatus = new
                    {
                        i.PaymentStatus.Id,
                        i.PaymentStatus.Status
                    },
                    Subscription = new
                    {
                        Company = new
                        {
                            i.Subscription.Company.CompanyId,
                            i.Subscription.Company.CompanyName,
                            i.Subscription.Company.CompanyPhone,
                            i.Subscription.Company.CompanyEmail
                        },

                        SubscriptionPlan = new
                        {
                            i.Subscription.SubscriptionPlan.Description,
                            i.Subscription.SubscriptionPlan.Duration,
                            PlanName = new
                            {
                                i.Subscription.SubscriptionPlan.PlanName.Id,
                                i.Subscription.SubscriptionPlan.PlanName.PlanName,
                                i.Subscription.SubscriptionPlan.PlanName.Price,
                            }
                        }
                    }
                })
                .Where(a => a.Subscription.Company.CompanyId == companyId)
                .OrderBy(a => a.Id);


            // Debug the query before applying DataSourceLoader
            Console.WriteLine("Payments query built. Applying DataSourceLoader...");

            // Apply filtering, sorting, and paging using DataSourceLoader
            var transformedData = await DataSourceLoader.LoadAsync(payments, loadOptions);

            // Serialize the retrieved object and log it
            var serializedData = JsonConvert.SerializeObject(transformedData, Formatting.Indented);
            Console.WriteLine($"Data transformation completed. Retrieved data: {serializedData}");

            // Return the transformed data as JSON
            return Json(transformedData);
        }

        [HttpPost]
        public async Task<IActionResult> Post(string values) {
            var model = new Payment();
            var valuesDict = JsonConvert.DeserializeObject<IDictionary>(values);
            PopulateModel(model, valuesDict);

            if(!TryValidateModel(model))
                return BadRequest(GetFullErrorMessage(ModelState));

            var result = _context.Payments.Add(model);
            await _context.SaveChangesAsync();

            return Json(new { result.Entity.Id });
        }

        [HttpPut]
        public async Task<IActionResult> Put(Guid key, string values) {
            var model = await _context.Payments.FirstOrDefaultAsync(item => item.Id == key);
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
            var model = await _context.Payments.FirstOrDefaultAsync(item => item.Id == key);

            _context.Payments.Remove(model);
            await _context.SaveChangesAsync();
        }


        [HttpGet]
        public async Task<IActionResult> PaymentMethodLookupsLookup(DataSourceLoadOptions loadOptions) {
            var lookup = from i in _context.PaymentMethodLookups
                         orderby i.Method
                         select new {
                             Value = i.Id,
                             Text = i.Method
                         };
            return Json(await DataSourceLoader.LoadAsync(lookup, loadOptions));
        }

        [HttpGet]
        public async Task<IActionResult> PaymentStatusLookupsLookup(DataSourceLoadOptions loadOptions) {
            var lookup = from i in _context.PaymentStatusLookups
                         orderby i.Status
                         select new {
                             Value = i.Id,
                             Text = i.Status
                         };
            return Json(await DataSourceLoader.LoadAsync(lookup, loadOptions));
        }

        [HttpGet]
        public async Task<IActionResult> SubscriptionsLookup(DataSourceLoadOptions loadOptions) {
            var lookup = from i in _context.Subscriptions
                         orderby i.PaymentStatus
                         select new {
                             Value = i.Id,
                             Text = i.PaymentStatus
                         };
            return Json(await DataSourceLoader.LoadAsync(lookup, loadOptions));
        }

        private void PopulateModel(Payment model, IDictionary values) {
            string ID = nameof(Payment.Id);
            string SUBSCRIPTION_ID = nameof(Payment.SubscriptionId);
            string PAYMENT_DATE = nameof(Payment.PaymentDate);
            string PAYMENT_STATUS_ID = nameof(Payment.PaymentStatusId);
            string PAYMENT_METHOD_ID = nameof(Payment.PaymentMethodId);
            string TRANSACTION_ID = nameof(Payment.TransactionId);
            string IS_REFUNDED = nameof(Payment.IsRefunded);

            if(values.Contains(ID)) {
                model.Id = ConvertTo<System.Guid>(values[ID]);
            }

            if(values.Contains(SUBSCRIPTION_ID)) {
                model.SubscriptionId = values[SUBSCRIPTION_ID] != null ? ConvertTo<System.Guid>(values[SUBSCRIPTION_ID]) : (Guid?)null;
            }

            if(values.Contains(PAYMENT_DATE)) {
                model.PaymentDate = Convert.ToDateTime(values[PAYMENT_DATE]);
            }

            if(values.Contains(PAYMENT_STATUS_ID)) {
                model.PaymentStatusId = Convert.ToInt32(values[PAYMENT_STATUS_ID]);
            }

            if(values.Contains(PAYMENT_METHOD_ID)) {
                model.PaymentMethodId = Convert.ToInt32(values[PAYMENT_METHOD_ID]);
            }

            if(values.Contains(TRANSACTION_ID)) {
                model.TransactionId = Convert.ToString(values[TRANSACTION_ID]);
            }

            if(values.Contains(IS_REFUNDED)) {
                model.IsRefunded = Convert.ToBoolean(values[IS_REFUNDED]);
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