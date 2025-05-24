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
    public class AspNetRolesAPIController : Controller
    {
        private ApplicationDbContext _context;
        private readonly ExceptionHandlerService _exceptionHandlerService;
        private readonly ILogger<AspNetRolesAPIController> _logger;

        public AspNetRolesAPIController(ApplicationDbContext context, ExceptionHandlerService exceptionHandlerService, ILogger<AspNetRolesAPIController> logger) {
            _context = context;
            _exceptionHandlerService = exceptionHandlerService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Get(DataSourceLoadOptions loadOptions) {
            try
            {
                var aspnetroles = _context.AspNetRoles.Select(i => new {
                    i.Id,
                    i.Name,
                    i.NormalizedName,
                    i.ConcurrencyStamp
                });

                return Json(await DataSourceLoader.LoadAsync(aspnetroles, loadOptions));
            }
            catch (Exception ex)
            {
                return _exceptionHandlerService.HandleException(ex, this);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post(string values) {
            var model = new AspNetRole();
            var valuesDict = JsonConvert.DeserializeObject<IDictionary>(values);
            PopulateModel(model, valuesDict);

            if(!TryValidateModel(model))
                return BadRequest(GetFullErrorMessage(ModelState));

            var result = _context.AspNetRoles.Add(model);
            await _context.SaveChangesAsync();

            return Json(new { result.Entity.Id });
        }

        [HttpPut]
        public async Task<IActionResult> Put(string key, string values) {
            var model = await _context.AspNetRoles.FirstOrDefaultAsync(item => item.Id == key);
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
        public async Task Delete(string key) {
            var model = await _context.AspNetRoles.FirstOrDefaultAsync(item => item.Id == key);

            _context.AspNetRoles.Remove(model);
            await _context.SaveChangesAsync();
        }


        private void PopulateModel(AspNetRole model, IDictionary values) {
            string ID = nameof(AspNetRole.Id);
            string NAME = nameof(AspNetRole.Name);
            string NORMALIZED_NAME = nameof(AspNetRole.NormalizedName);
            string CONCURRENCY_STAMP = nameof(AspNetRole.ConcurrencyStamp);

            if(values.Contains(ID)) {
                model.Id = Convert.ToString(values[ID]);
            }

            if(values.Contains(NAME)) {
                model.Name = Convert.ToString(values[NAME]);
            }

            if(values.Contains(NORMALIZED_NAME)) {
                model.NormalizedName = Convert.ToString(values[NORMALIZED_NAME]);
            }

            if(values.Contains(CONCURRENCY_STAMP)) {
                model.ConcurrencyStamp = Convert.ToString(values[CONCURRENCY_STAMP]);
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