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
    public class SubscriptionPaymentsAPIController : Controller
    {
        private ApplicationDbContext _context;
        private readonly ExceptionHandlerService _exceptionHandlerService;
        private readonly ILogger<SubscriptionPaymentsAPIController> _logger;

        public SubscriptionPaymentsAPIController(ApplicationDbContext context, ExceptionHandlerService exceptionHandlerService, ILogger<SubscriptionPaymentsAPIController> logger)
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
                var subscriptionpayments = _context.SubscriptionPayments.Select(i => new {
                    i.Id,
                    i.SubscriptionId,
                    i.PaymentDate,
                    i.Status,
                    i.Method,
                    i.TransactionId,
                    i.IsRefunded,

                    Subscription = new
                    {
                        Company = new
                        {
                            i.Subscription.Company.CompanyName
                        },
                        SubscriptionPlan = new
                        {
                            PlanName = new
                            {
                                i.Subscription.SubscriptionPlan.PlanName.PlanName,
                                i.Subscription.SubscriptionPlan.PlanName.Price,
                            },

                            i.Subscription.SubscriptionPlan.Duration,
                            i.Subscription.SubscriptionPlan.BillingCycle,
                        }
                    }
                });

                return Json(await DataSourceLoader.LoadAsync(subscriptionpayments, loadOptions));
            }
            catch (Exception ex)
            {
                return _exceptionHandlerService.HandleException(ex, this);
            }
        }


        [HttpPost]
        public async Task<IActionResult> Post(string values) {
            var model = new SubscriptionPayment();
            var valuesDict = JsonConvert.DeserializeObject<IDictionary>(values);
            PopulateModel(model, valuesDict);

            if(!TryValidateModel(model))
                return BadRequest(GetFullErrorMessage(ModelState));

            var result = _context.SubscriptionPayments.Add(model);
            await _context.SaveChangesAsync();

            return Json(new { result.Entity.Id });
        }

        [HttpPut]
        public async Task<IActionResult> Put(Guid key, string values) {
            var model = await _context.SubscriptionPayments.FirstOrDefaultAsync(item => item.Id == key);
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
            var model = await _context.SubscriptionPayments.FirstOrDefaultAsync(item => item.Id == key);

            _context.SubscriptionPayments.Remove(model);
            await _context.SaveChangesAsync();
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

        private void PopulateModel(SubscriptionPayment model, IDictionary values) {
            string ID = nameof(SubscriptionPayment.Id);
            string SUBSCRIPTION_ID = nameof(SubscriptionPayment.SubscriptionId);
            string PAYMENT_DATE = nameof(SubscriptionPayment.PaymentDate);
            string PAYMENT_STATUS_ID = nameof(SubscriptionPayment.PaymentStatusId);
            string PAYMENT_METHOD_ID = nameof(SubscriptionPayment.PaymentMethodId);
            string TRANSACTION_ID = nameof(SubscriptionPayment.TransactionId);
            string IS_REFUNDED = nameof(SubscriptionPayment.IsRefunded);
            string METHOD = nameof(SubscriptionPayment.Method);
            string STATUS = nameof(SubscriptionPayment.Status);

            if(values.Contains(ID)) {
                model.Id = ConvertTo<System.Guid>(values[ID]);
            }

            if(values.Contains(SUBSCRIPTION_ID)) {
                model.SubscriptionId = ConvertTo<System.Guid>(values[SUBSCRIPTION_ID]);
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

            if(values.Contains(METHOD)) {
                model.Method = Convert.ToString(values[METHOD]);
            }

            if(values.Contains(STATUS)) {
                model.Status = Convert.ToString(values[STATUS]);
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