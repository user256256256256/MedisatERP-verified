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
using MedisatERP.Areas.NutritionCompanySystem.Models;

namespace MedisatERP.Controllers
{
    [Route("api/[controller]/[action]")]
    public class OnlineApplicantsAPIController : Controller
    {
        private NutritionSystemDbContext _context;

        public OnlineApplicantsAPIController(NutritionSystemDbContext context) {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get(DataSourceLoadOptions loadOptions) {
            var onlineapplicants = _context.OnlineApplicants.Select(i => new {
                i.Id,
                i.FirstName,
                i.LastName,
                i.Email,
                i.MobilePhoneNo,
                i.Address,
                i.Age,
                i.Reason,
                i.PreferredSchedule,
                i.HowDidYouHearAboutUs,
                i.AcceptPrivacyPolicies,
                i.CreatedDate
            });

            return Json(await DataSourceLoader.LoadAsync(onlineapplicants, loadOptions));
        }
        
        [HttpPost]
        public async Task<IActionResult> Post(string values) {
            var model = new OnlineApplicants();
            var valuesDict = JsonConvert.DeserializeObject<IDictionary>(values);
            PopulateModel(model, valuesDict);

            if(!TryValidateModel(model))
                return BadRequest(GetFullErrorMessage(ModelState));

            var result = _context.OnlineApplicants.Add(model);
            await _context.SaveChangesAsync();

            return Json(new { result.Entity.Id });
        }

        [HttpPut]
        public async Task<IActionResult> Put(Guid key, string values) {
            var model = await _context.OnlineApplicants.FirstOrDefaultAsync(item => item.Id == key);
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
            var model = await _context.OnlineApplicants.FirstOrDefaultAsync(item => item.Id == key);

            _context.OnlineApplicants.Remove(model);
            await _context.SaveChangesAsync();
        }

        private void PopulateModel(OnlineApplicants model, IDictionary values) {
            string ID = nameof(OnlineApplicants.Id);
            string FIRST_NAME = nameof(OnlineApplicants.FirstName);
            string LAST_NAME = nameof(OnlineApplicants.LastName);
            string EMAIL = nameof(OnlineApplicants.Email);
            string MOBILE_PHONE_NO = nameof(OnlineApplicants.MobilePhoneNo);
            string ADDRESS = nameof(OnlineApplicants.Address);
            string AGE = nameof(OnlineApplicants.Age);
            string REASON = nameof(OnlineApplicants.Reason);
            string PREFERRED_SCHEDULE = nameof(OnlineApplicants.PreferredSchedule);
            string HOW_DID_YOU_HEAR_ABOUT_US = nameof(OnlineApplicants.HowDidYouHearAboutUs);
            string ACCEPT_PRIVACY_POLICIES = nameof(OnlineApplicants.AcceptPrivacyPolicies);
            string CREATED_DATE = nameof(OnlineApplicants.CreatedDate);

            if(values.Contains(ID)) {
                model.Id = ConvertTo<System.Guid>(values[ID]);
            }

            if(values.Contains(FIRST_NAME)) {
                model.FirstName = Convert.ToString(values[FIRST_NAME]);
            }

            if(values.Contains(LAST_NAME)) {
                model.LastName = Convert.ToString(values[LAST_NAME]);
            }

            if(values.Contains(EMAIL)) {
                model.Email = Convert.ToString(values[EMAIL]);
            }

            if(values.Contains(MOBILE_PHONE_NO)) {
                model.MobilePhoneNo = Convert.ToString(values[MOBILE_PHONE_NO]);
            }

            if(values.Contains(ADDRESS)) {
                model.Address = Convert.ToString(values[ADDRESS]);
            }

            if(values.Contains(AGE)) {
                model.Age = values[AGE] != null ? Convert.ToInt32(values[AGE]) : (int?)null;
            }

            if(values.Contains(REASON)) {
                model.Reason = Convert.ToString(values[REASON]);
            }

            if(values.Contains(PREFERRED_SCHEDULE)) {
                model.PreferredSchedule = values[PREFERRED_SCHEDULE] != null ? Convert.ToDateTime(values[PREFERRED_SCHEDULE]) : (DateTime?)null;
            }

            if(values.Contains(HOW_DID_YOU_HEAR_ABOUT_US)) {
                model.HowDidYouHearAboutUs = Convert.ToString(values[HOW_DID_YOU_HEAR_ABOUT_US]);
            }

            if(values.Contains(ACCEPT_PRIVACY_POLICIES)) {
                model.AcceptPrivacyPolicies = Convert.ToBoolean(values[ACCEPT_PRIVACY_POLICIES]);
            }

            if(values.Contains(CREATED_DATE)) {
                model.CreatedDate = Convert.ToDateTime(values[CREATED_DATE]);
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