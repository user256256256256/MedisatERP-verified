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
    public class SubscriptionsAPIController : Controller
    {
        private ApplicationDbContext _context;
        private readonly ExceptionHandlerService _exceptionHandlerService;
        private readonly ILogger<SubscriptionsAPIController> _logger;

        public SubscriptionsAPIController(ApplicationDbContext context, ExceptionHandlerService exceptionHandlerService, ILogger<SubscriptionsAPIController> logger)
        {
            _context = context;
            _exceptionHandlerService = exceptionHandlerService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Get(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var subscriptions = _context.Subscriptions.Select(i => new {
                    i.Id,
                    i.CompanyId,
                    i.SubscriptionPlanId,
                    i.StartDate,
                    i.EndDate,
                    i.IsActive,
                    SubscriptionPlan = new
                    {
                        i.SubscriptionPlan.Description,
                        i.SubscriptionPlan.Duration,
                        i.SubscriptionPlan.BillingCycle,

                        PlanName = new
                        {
                            i.SubscriptionPlan.PlanName.PlanName,
                            i.SubscriptionPlan.PlanName.Price,
                        }
                    },
                    Company = new
                    {
                        i.Company.CompanyName
                    }
                });

                return Json(await DataSourceLoader.LoadAsync(subscriptions, loadOptions));
            }
            catch (Exception ex)
            {
                return _exceptionHandlerService.HandleException(ex, this);
            }
        }

        [HttpGet] 
        public async Task<IActionResult> GetCompanySubscription(DataSourceLoadOptions loadOptions, Guid companyId)
        {
            try
            {
                var subscription = _context.Subscriptions
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
                            i.SubscriptionPlan.Description,
                            i.SubscriptionPlan.Duration,
                            i.SubscriptionPlan.BillingCycle,

                            PlanName = new
                            {
                                i.SubscriptionPlan.PlanName.PlanName,
                                i.SubscriptionPlan.PlanName.Price,
                            }
                        },
                        Company = new
                        {
                            i.Company.CompanyName
                        }
                    }).Where(a => a.CompanyId == companyId).OrderBy(a => a.Id);

                return Json(await DataSourceLoader.LoadAsync(subscription, loadOptions));
            }
            catch (Exception ex)
            {
               return _exceptionHandlerService.HandleException(ex, this);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post(string values) {
            var model = new Subscription();
            var valuesDict = JsonConvert.DeserializeObject<IDictionary>(values);
            PopulateModel(model, valuesDict);

            if(!TryValidateModel(model))
                return BadRequest(GetFullErrorMessage(ModelState));

            var result = _context.Subscriptions.Add(model);
            await _context.SaveChangesAsync();

            return Json(new { result.Entity.Id });
        }

        [HttpPut]
        public async Task<IActionResult> Put(Guid key, string values) {
            var model = await _context.Subscriptions.FirstOrDefaultAsync(item => item.Id == key);
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
            var model = await _context.Subscriptions.FirstOrDefaultAsync(item => item.Id == key);

            _context.Subscriptions.Remove(model);
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

        [HttpGet]
        public async Task<IActionResult> SubscriptionPlansLookup(DataSourceLoadOptions loadOptions) {
            var lookup = from i in _context.SubscriptionPlans
                         orderby i.Description
                         select new {
                             Value = i.Id,
                             Text = i.Description
                         };
            return Json(await DataSourceLoader.LoadAsync(lookup, loadOptions));
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
                model.CompanyId = ConvertTo<System.Guid>(values[COMPANY_ID]);
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