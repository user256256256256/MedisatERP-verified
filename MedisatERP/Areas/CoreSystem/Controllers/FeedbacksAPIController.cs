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
    public class FeedbacksAPIController : Controller
    {
        private MedisatErpDbContext _context;

        public FeedbacksAPIController(MedisatErpDbContext context) {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get(DataSourceLoadOptions loadOptions) {
            var feedbacks = _context.Feedbacks.Select(i => new {
                i.FeedbackId,
                i.UserId,
                i.FeedbackText,
                i.Rating,
                i.Category,
                i.SubmittedAt,
                i.Resolved
            });

            // If underlying data is a large SQL table, specify PrimaryKey and PaginateViaPrimaryKey.
            // This can make SQL execution plans more efficient.
            // For more detailed information, please refer to this discussion: https://github.com/DevExpress/DevExtreme.AspNet.Data/issues/336.
            // loadOptions.PrimaryKey = new[] { "FeedbackId" };
            // loadOptions.PaginateViaPrimaryKey = true;

            return Json(await DataSourceLoader.LoadAsync(feedbacks, loadOptions));
        }

        [HttpPost]
        public async Task<IActionResult> Post(string values) {
            var model = new Feedback();
            var valuesDict = JsonConvert.DeserializeObject<IDictionary>(values);
            PopulateModel(model, valuesDict);

            if(!TryValidateModel(model))
                return BadRequest(GetFullErrorMessage(ModelState));

            var result = _context.Feedbacks.Add(model);
            await _context.SaveChangesAsync();

            return Json(new { result.Entity.FeedbackId });
        }

        [HttpPut]
        public async Task<IActionResult> Put(Guid key, string values) {
            var model = await _context.Feedbacks.FirstOrDefaultAsync(item => item.FeedbackId == key);
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
            var model = await _context.Feedbacks.FirstOrDefaultAsync(item => item.FeedbackId == key);

            _context.Feedbacks.Remove(model);
            await _context.SaveChangesAsync();
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
            string FEEDBACK_ID = nameof(Feedback.FeedbackId);
            string USER_ID = nameof(Feedback.UserId);
            string FEEDBACK_TEXT = nameof(Feedback.FeedbackText);
            string RATING = nameof(Feedback.Rating);
            string CATEGORY = nameof(Feedback.Category);
            string SUBMITTED_AT = nameof(Feedback.SubmittedAt);
            string RESOLVED = nameof(Feedback.Resolved);

            if(values.Contains(FEEDBACK_ID)) {
                model.FeedbackId = ConvertTo<System.Guid>(values[FEEDBACK_ID]);
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