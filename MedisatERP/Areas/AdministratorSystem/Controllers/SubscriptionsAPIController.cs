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
using MedisatERP.Areas.AdministratorSystem.Models;
using System.ComponentModel.Design;
using MedisatERP.Models;
using Microsoft.Data.SqlClient;

namespace MedisatERP.Controllers
{
    [Route("api/[controller]/[action]")]
    public class SubscriptionsAPIController : Controller
    {
        private AdministratorSystemDbContext _context;

        public SubscriptionsAPIController(AdministratorSystemDbContext context) {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get(DataSourceLoadOptions loadOptions, Guid SubscriptionPlanId)
        {
            try
            {
                // Log the input parameters for debugging
                Console.WriteLine($"Get method called with planId: {SubscriptionPlanId}");

                // Build the subscriptions query
                var subscriptions = _context.Subscriptions
                    .Include(c => c.SubscriptionPlan)
                    .Include(c => c.Company)
                    .Select(i => new
                    {
                        i.Id,
                        i.CompanyId,
                        i.SubscriptionPlanId,
                        i.StartDate,
                        i.EndDate,
                        i.IsActive,
                        i.PaymentStatus,
                        SubscriptionPlan = new
                        {
                            i.SubscriptionPlan.Id,
                            i.SubscriptionPlan.PlanNameId,
                            i.SubscriptionPlan.Description,
                            i.SubscriptionPlan.Duration,
                            i.SubscriptionPlan.BillingCycleId,
                            i.SubscriptionPlan.BillingCycle.CycleName,
                            PlanName = new
                            {
                                i.SubscriptionPlan.PlanName.Id,
                                i.SubscriptionPlan.PlanName.PlanName,
                                i.SubscriptionPlan.PlanName.Price
                            },
                            BillingCycle = new
                            {
                                i.SubscriptionPlan.BillingCycle.Id,
                                i.SubscriptionPlan.BillingCycle.CycleName,
                            }
                        },
                        Company = new
                        {
                            i.Company.CompanyName
                        }
                    })
                    .Where(a => a.SubscriptionPlanId == SubscriptionPlanId)
                    .OrderBy(a => a.Id);

                // Debug the query before applying DataSourceLoader
                Console.WriteLine("Subscriptions query built. Applying DataSourceLoader...");

                // Apply filtering, sorting, and paging using DataSourceLoader
                var transformedData = await DataSourceLoader.LoadAsync(subscriptions, loadOptions);

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


        [HttpGet]
        public async Task<IActionResult> GetCompanySubscription(DataSourceLoadOptions loadOptions, Guid companyId)
        {
            try
            {
                // Log the input parameters for debugging
                Console.WriteLine($"Get method called with companyId: {companyId}");

                // Build the subscriptions query
                var subscriptions = _context.Subscriptions
                    .Include(c => c.SubscriptionPlan)
                    .Include(c => c.Company)
                    .Select(i => new
                    {
                        i.Id,
                        i.CompanyId,
                        i.SubscriptionPlanId,
                        i.StartDate,
                        i.EndDate,
                        i.IsActive,
                        i.PaymentStatus,
                        SubscriptionPlan = new
                        {
                            i.SubscriptionPlan.Id,
                            i.SubscriptionPlan.PlanNameId,
                            i.SubscriptionPlan.Description,
                            i.SubscriptionPlan.Duration,
                            i.SubscriptionPlan.BillingCycleId,
                            i.SubscriptionPlan.BillingCycle.CycleName,
                            PlanName = new
                            {
                                i.SubscriptionPlan.PlanName.Id,
                                i.SubscriptionPlan.PlanName.PlanName,
                                i.SubscriptionPlan.PlanName.Price
                            },
                            BillingCycle = new
                            {
                                i.SubscriptionPlan.BillingCycle.Id,
                                i.SubscriptionPlan.BillingCycle.CycleName,
                            }
                        },
                        Company = new
                        {
                            i.Company.CompanyName
                        }
                    })
                    .Where(a => a.CompanyId == companyId)
                    .OrderBy(a => a.Id);

                // Debug the query before applying DataSourceLoader
                Console.WriteLine("Subscriptions query built. Applying DataSourceLoader...");

                // Apply filtering, sorting, and paging using DataSourceLoader
                var transformedData = await DataSourceLoader.LoadAsync(subscriptions, loadOptions);

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
                var model = new Subscription();
                var valuesDict = JsonConvert.DeserializeObject<IDictionary>(values);
                PopulateModel(model, valuesDict);

                if (!TryValidateModel(model))
                    return BadRequest(GetFullErrorMessage(ModelState));

                var result = _context.Subscriptions.Add(model);
                await _context.SaveChangesAsync();

                return Json(new { result.Entity.Id });
            }
            catch (JsonSerializationException ex)
            {
                // Log the exception
                Console.WriteLine($"JSON serialization error: {ex.Message}");
                return BadRequest(new { message = "Invalid input format. Please check your data and try again." });
            }
            catch (SqlException ex)
            {
                // Log the exception
                Console.WriteLine(ex);
                return StatusCode(500, new { message = "A database error occurred. Please try again later." });
            }
            catch (InvalidOperationException ex)
            {
                // Log the exception
                Console.WriteLine(ex);
                return StatusCode(500, new { message = "An internal server error occurred. Please try again later." });
            }
            catch (Exception ex)
            {
                // Log the exception message and stack trace for debugging purposes
                Console.WriteLine($"An error occurred: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return StatusCode(500, new { message = "An unexpected error occurred. Please try again later.", error = ex.Message });
            }
        }

        [HttpPut]
        public async Task<IActionResult> Put(Guid key, string values)
        {
            try
            {
                // Retrieve the subscription by its unique identifier
                var model = await _context.Subscriptions.FirstOrDefaultAsync(item => item.Id == key);
                if (model == null)
                {
                    Console.WriteLine($"Subscription not found with key: {key}");
                    return StatusCode(409, "Object not found");
                }

                Console.WriteLine($"Subscription found with key: {key}, proceeding with updates.");

                // Deserialize the incoming updated values
                var valuesDict = JsonConvert.DeserializeObject<IDictionary>(values);
                PopulateModel(model, valuesDict);

                // Validate the updated model before saving
                if (!TryValidateModel(model))
                {
                    Console.WriteLine("Model validation failed.");
                    return BadRequest(GetFullErrorMessage(ModelState));
                }

                Console.WriteLine("Model validated successfully.");

                try
                {
                    // Save the changes to the database
                    await _context.SaveChangesAsync();
                    Console.WriteLine("Subscription updated successfully in the database.");
                    return Ok();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    // Handle the concurrency exception
                    Console.WriteLine("Concurrency exception occurred while updating subscription.");
                    var entry = ex.Entries.Single();
                    var databaseValues = entry.GetDatabaseValues();
                    if (databaseValues == null)
                    {
                        Console.WriteLine("The record you attempted to edit was deleted by another user.");
                        return NotFound(new { success = false, message = "The record you attempted to edit was deleted by another user." });
                    }
                    else
                    {
                        var dbValues = (Subscription)databaseValues.ToObject();
                        Console.WriteLine("The record you attempted to edit was modified by another user.");

                        // Optionally, reload the entity with current database values
                        await entry.ReloadAsync();
                        return Conflict(new { success = false, message = "The record you attempted to edit was modified by another user.", currentValues = dbValues });
                    }
                }
            }
            catch (JsonSerializationException ex)
            {
                // Log the exception
                Console.WriteLine($"JSON serialization error: {ex.Message}");
                return BadRequest(new { message = "Invalid input format. Please check your data and try again." });
            }
            catch (SqlException ex)
            {
                // Log the exception
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



        [HttpDelete]
        public async Task<IActionResult> Delete(Guid key)
        {
            try
            {
                var model = await _context.Subscriptions.FirstOrDefaultAsync(item => item.Id == key);

                if (model == null)
                {
                    Console.WriteLine($"Subscription not found with key: {key}");
                    return NotFound($"Subscription with ID {key} not found.");
                }

                _context.Subscriptions.Remove(model);
                await _context.SaveChangesAsync();

                Console.WriteLine($"Successfully deleted Subscription with ID: {key}");
                return NoContent();
            }
            catch (SqlException ex)
            {
                // Log the exception
                Console.WriteLine(ex);
                return StatusCode(500, new { message = "A database error occurred. Please try again later." });
            }
            catch (InvalidOperationException ex)
            {
                // Log the exception
                Console.WriteLine(ex);
                return StatusCode(500, new { message = "An internal server error occurred. Please try again later." });
            }
            catch (Exception ex)
            {
                // Log the exception message and stack trace for debugging purposes
                Console.WriteLine($"An error occurred: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return StatusCode(500, new { message = "An unexpected error occurred. Please try again later.", error = ex.Message });
            }
        }



        [HttpGet]
        public async Task<IActionResult> CompaniesLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = from i in _context.Companies
                             orderby i.CompanyName
                             select new
                             {
                                 Value = i.CompanyId,
                                 Text = i.CompanyName
                             };
                return Json(await DataSourceLoader.LoadAsync(lookup, loadOptions));
            }
            catch (SqlException ex)
            {
                // Log the exception
                Console.WriteLine(ex);
                return StatusCode(500, new { message = "A database error occurred. Please try again later." });
            }
            catch (InvalidOperationException ex)
            {
                // Log the exception
                Console.WriteLine(ex);
                return StatusCode(500, new { message = "An internal server error occurred. Please try again later." });
            }
            catch (Exception ex)
            {
                // Log the exception message and stack trace for debugging purposes
                Console.WriteLine($"An error occurred: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return StatusCode(500, new { message = "An unexpected error occurred. Please try again later.", error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> SubscriptionPlansLookup(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var lookup = from i in _context.SubscriptionPlans
                             orderby i.Description
                             select new
                             {
                                 Value = i.Id,
                                 Text = i.Description
                             };
                return Json(await DataSourceLoader.LoadAsync(lookup, loadOptions));
            }
            catch (SqlException ex)
            {
                // Log the exception
                Console.WriteLine(ex);
                return StatusCode(500, new { message = "A database error occurred. Please try again later." });
            }
            catch (InvalidOperationException ex)
            {
                // Log the exception
                Console.WriteLine(ex);
                return StatusCode(500, new { message = "An internal server error occurred. Please try again later." });
            }
            catch (Exception ex)
            {
                // Log the exception message and stack trace for debugging purposes
                Console.WriteLine($"An error occurred: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return StatusCode(500, new { message = "An unexpected error occurred. Please try again later.", error = ex.Message });
            }
        }


        private void PopulateModel(Subscription model, IDictionary values) {
            string ID = nameof(Subscription.Id);
            string COMPANY_ID = nameof(Subscription.CompanyId);
            string SUBSCRIPTION_PLAN_ID = nameof(Subscription.SubscriptionPlanId);
            string START_DATE = nameof(Subscription.StartDate);
            string END_DATE = nameof(Subscription.EndDate);
            string IS_ACTIVE = nameof(Subscription.IsActive);
            string PAYMENT_STATUS = nameof(Subscription.PaymentStatus);

            if(values.Contains(ID)) {
                model.Id = ConvertTo<System.Guid>(values[ID]);
            }

            if(values.Contains(COMPANY_ID)) {
                model.CompanyId = values[COMPANY_ID] != null ? ConvertTo<System.Guid>(values[COMPANY_ID]) : (Guid?)null;
            }

            if(values.Contains(SUBSCRIPTION_PLAN_ID)) {
                model.SubscriptionPlanId = ConvertTo<System.Guid>(values[SUBSCRIPTION_PLAN_ID]);
            }

            if(values.Contains(START_DATE)) {
                model.StartDate = Convert.ToDateTime(values[START_DATE]);
            }

            if(values.Contains(END_DATE)) {
                model.EndDate = values[END_DATE] != null ? Convert.ToDateTime(values[END_DATE]) : (DateTime?)null;
            }

            if(values.Contains(IS_ACTIVE)) {
                model.IsActive = Convert.ToBoolean(values[IS_ACTIVE]);
            }

            if(values.Contains(PAYMENT_STATUS)) {
                model.PaymentStatus = Convert.ToString(values[PAYMENT_STATUS]);
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