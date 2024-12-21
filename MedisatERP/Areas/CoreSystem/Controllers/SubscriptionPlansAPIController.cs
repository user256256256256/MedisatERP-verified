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
    public class SubscriptionPlansAPIController : Controller
    {
        private MedisatErpDbContext _context;

        public SubscriptionPlansAPIController(MedisatErpDbContext context) {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get(DataSourceLoadOptions loadOptions)
        {
            var subscriptionPlans = _context.SubscriptionPlans
                .Include(c => c.PlanName)
                .Select(i => new {
                    i.Id,
                    PlanName = i.PlanName.PlanName,  // Directly include PlanName
                    i.PlanNameId,
                    i.Description,
                    i.Duration,
                    i.BillingCycleId,
                    Price = i.PlanName.Price // Include the Price if necessary
                });

            return Json(await DataSourceLoader.LoadAsync(subscriptionPlans, loadOptions));
        }


        [HttpPost]
        public async Task<IActionResult> Post(string values) {
            var model = new SubscriptionPlan();
            var valuesDict = JsonConvert.DeserializeObject<IDictionary>(values);
            PopulateModel(model, valuesDict);

            if(!TryValidateModel(model))
                return BadRequest(GetFullErrorMessage(ModelState));

            var result = _context.SubscriptionPlans.Add(model);
            await _context.SaveChangesAsync();

            return Json(new { result.Entity.Id });
        }

        [HttpPut]
        public async Task<IActionResult> Put(Guid key, string values) {
            var model = await _context.SubscriptionPlans.FirstOrDefaultAsync(item => item.Id == key);
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
            var model = await _context.SubscriptionPlans.FirstOrDefaultAsync(item => item.Id == key);

            _context.SubscriptionPlans.Remove(model);
            await _context.SaveChangesAsync();
        }


        [HttpGet]
        public async Task<IActionResult> BillingCycleLookupsLookup(DataSourceLoadOptions loadOptions) {
            var lookup = from i in _context.BillingCycleLookups
                         orderby i.CycleName
                         select new {
                             Value = i.Id,
                             Text = i.CycleName
                         };
            return Json(await DataSourceLoader.LoadAsync(lookup, loadOptions));
        }

        //[HttpGet]
        //public async Task<IActionResult> SubscriptionPlanNameLookupsLookup(DataSourceLoadOptions loadOptions) {
        //    var lookup = from i in _context.SubscriptionPlanNameLookups
        //                 orderby i.PlanName
        //                 select new {
        //                     Value = i.Id,
        //                     Text = i.PlanName
        //                 };
        //    return Json(await DataSourceLoader.LoadAsync(lookup, loadOptions));
        //}

        private void PopulateModel(SubscriptionPlan model, IDictionary values) {
            string ID = nameof(SubscriptionPlan.Id);
            string PLAN_NAME_ID = nameof(SubscriptionPlan.PlanNameId);
            string DESCRIPTION = nameof(SubscriptionPlan.Description);
            string DURATION = nameof(SubscriptionPlan.Duration);
            string BILLING_CYCLE_ID = nameof(SubscriptionPlan.BillingCycleId);

            if(values.Contains(ID)) {
                model.Id = ConvertTo<System.Guid>(values[ID]);
            }

            if(values.Contains(PLAN_NAME_ID)) {
                model.PlanNameId = Convert.ToInt32(values[PLAN_NAME_ID]);
            }

            if(values.Contains(DESCRIPTION)) {
                model.Description = Convert.ToString(values[DESCRIPTION]);
            }

            if(values.Contains(DURATION)) {
                model.Duration = Convert.ToInt32(values[DURATION]);
            }

            if(values.Contains(BILLING_CYCLE_ID)) {
                model.BillingCycleId = Convert.ToInt32(values[BILLING_CYCLE_ID]);
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