﻿using DevExtreme.AspNet.Data;
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
using System.Diagnostics;

namespace MedisatERP.Controllers
{
    [Route("api/[controller]/[action]")]
    public class SubscriptionLogsAPIController : Controller
    {
        private MedisatErpDbContext _context;

        public SubscriptionLogsAPIController(MedisatErpDbContext context) {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get(DataSourceLoadOptions loadOptions) {
            var subscriptionlogs = _context.SubscriptionLogs
                .Include(c => c.Subscription)
                .Select(i => new {
                i.Id,
                i.SubscriptionId,
                i.ActivityId,
                i.LogDate,
                Activity = new
                {
                    i.Activity.Id,
                    i.Activity.ActivityName
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
            var transformedData = await DataSourceLoader.LoadAsync(subscriptionlogs, loadOptions);

            // Serialize the retrieved object and log it
            var serializedData = JsonConvert.SerializeObject(transformedData, Formatting.Indented);
            Console.WriteLine($"Data transformation completed. Retrieved data: {serializedData}");

            // Return the transformed data as JSON
            return Json(transformedData);
        }

        [HttpPost]
        public async Task<IActionResult> Post(string values) {
            var model = new SubscriptionLog();
            var valuesDict = JsonConvert.DeserializeObject<IDictionary>(values);
            PopulateModel(model, valuesDict);

            if(!TryValidateModel(model))
                return BadRequest(GetFullErrorMessage(ModelState));

            var result = _context.SubscriptionLogs.Add(model);
            await _context.SaveChangesAsync();

            return Json(new { result.Entity.Id });
        }

        [HttpPut]
        public async Task<IActionResult> Put(Guid key, string values) {
            var model = await _context.SubscriptionLogs.FirstOrDefaultAsync(item => item.Id == key);
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
            var model = await _context.SubscriptionLogs.FirstOrDefaultAsync(item => item.Id == key);

            _context.SubscriptionLogs.Remove(model);
            await _context.SaveChangesAsync();
        }


        [HttpGet]
        public async Task<IActionResult> SubscriptionActivityLookupsLookup(DataSourceLoadOptions loadOptions) {
            var lookup = from i in _context.SubscriptionActivityLookups
                         orderby i.ActivityName
                         select new {
                             Value = i.Id,
                             Text = i.ActivityName
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

        private void PopulateModel(SubscriptionLog model, IDictionary values) {
            string ID = nameof(SubscriptionLog.Id);
            string SUBSCRIPTION_ID = nameof(SubscriptionLog.SubscriptionId);
            string ACTIVITY_ID = nameof(SubscriptionLog.ActivityId);
            string LOG_DATE = nameof(SubscriptionLog.LogDate);

            if(values.Contains(ID)) {
                model.Id = ConvertTo<System.Guid>(values[ID]);
            }

            if(values.Contains(SUBSCRIPTION_ID)) {
                model.SubscriptionId = ConvertTo<System.Guid>(values[SUBSCRIPTION_ID]);
            }

            if(values.Contains(ACTIVITY_ID)) {
                model.ActivityId = Convert.ToInt32(values[ACTIVITY_ID]);
            }

            if(values.Contains(LOG_DATE)) {
                model.LogDate = Convert.ToDateTime(values[LOG_DATE]);
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