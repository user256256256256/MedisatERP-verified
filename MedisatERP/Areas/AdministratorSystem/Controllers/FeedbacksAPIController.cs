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
    public class FeedbacksAPIController : Controller
    {
        private ApplicationDbContext _context;
        private readonly ExceptionHandlerService _exceptionHandlerService;
        private readonly ILogger<FeedbacksAPIController> _logger;

        public FeedbacksAPIController(ApplicationDbContext context, ExceptionHandlerService exceptionHandlerService, ILogger<FeedbacksAPIController> logger)
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
                var feedbacks = _context.Feedbacks.Select(i => new {
                    i.Id,
                    i.UserId,
                    i.FeedbackText,
                    i.Rating,
                    i.Category,
                    i.SubmittedAt,
                    i.Resolved,

                    User = new
                    {
                        i.User.UserName
                    }

                });

                return Json(await DataSourceLoader.LoadAsync(feedbacks, loadOptions));
            }
            catch (Exception ex)
            {
                return _exceptionHandlerService.HandleException(ex, this);
            }
        }


        [HttpPost]
        public async Task<IActionResult> Post(string values) {
            try
            {
                var model = new Feedback();
                var valuesDict = JsonConvert.DeserializeObject<IDictionary>(values);
                PopulateModel(model, valuesDict);

                if (!TryValidateModel(model))
                    return BadRequest(GetFullErrorMessage(ModelState));

                var result = _context.Feedbacks.Add(model);
                await _context.SaveChangesAsync();

                return Json(new { result.Entity.Id });
            }
            catch (Exception ex)
            {
                return _exceptionHandlerService.HandleException(ex, this);
            }

        }

        [HttpPut]
        public async Task<IActionResult> Put(Guid key, string values) {
            try
            {
                var model = await _context.Feedbacks.FirstOrDefaultAsync(item => item.Id == key);
                if (model == null)
                    return StatusCode(409, "Object not found");

                var valuesDict = JsonConvert.DeserializeObject<IDictionary>(values);
                PopulateModel(model, valuesDict);

                if (!TryValidateModel(model))
                    return BadRequest(GetFullErrorMessage(ModelState));

                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return _exceptionHandlerService.HandleException(ex, this);
            }
        }

        [HttpDelete]
        public async Task Delete(Guid key) {
            try
            {
                var model = await _context.Feedbacks.FirstOrDefaultAsync(item => item.Id == key);

                _context.Feedbacks.Remove(model);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _exceptionHandlerService.HandleException(ex, this);
            }
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
        public async Task<IActionResult> AspNetUsersLookup(DataSourceLoadOptions loadOptions) {
            var lookup = from i in _context.AspNetUsers
                         orderby i.UserName
                         select new {
                             Value = i.Id,
                             Text = i.UserName
                         };
            return Json(await DataSourceLoader.LoadAsync(lookup, loadOptions));
        }

        private void PopulateModel(Feedback model, IDictionary values) {
            string ID = nameof(Feedback.Id);
            string USER_ID = nameof(Feedback.UserId);
            string FEEDBACK_TEXT = nameof(Feedback.FeedbackText);
            string RATING = nameof(Feedback.Rating);
            string CATEGORY = nameof(Feedback.Category);
            string SUBMITTED_AT = nameof(Feedback.SubmittedAt);
            string RESOLVED = nameof(Feedback.Resolved);
            string COMPANY_ID = nameof(Feedback.CompanyId);

            if(values.Contains(ID)) {
                model.Id = ConvertTo<System.Guid>(values[ID]);
            }

            if(values.Contains(USER_ID)) {
                model.UserId = Convert.ToString(values[USER_ID]);
            }

            if(values.Contains(FEEDBACK_TEXT)) {
                model.FeedbackText = Convert.ToString(values[FEEDBACK_TEXT]);
            }

            if(values.Contains(RATING)) {
                model.Rating = values[RATING] != null ? Convert.ToInt32(values[RATING]) : (int?)null;
            }

            if(values.Contains(CATEGORY)) {
                model.Category = Convert.ToString(values[CATEGORY]);
            }

            if(values.Contains(SUBMITTED_AT)) {
                model.SubmittedAt = Convert.ToDateTime(values[SUBMITTED_AT]);
            }

            if(values.Contains(RESOLVED)) {
                model.Resolved = Convert.ToBoolean(values[RESOLVED]);
            }

            if(values.Contains(COMPANY_ID)) {
                model.CompanyId = values[COMPANY_ID] != null ? ConvertTo<System.Guid>(values[COMPANY_ID]) : (Guid?)null;
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