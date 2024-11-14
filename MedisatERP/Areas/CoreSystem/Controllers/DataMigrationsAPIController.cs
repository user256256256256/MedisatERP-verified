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
    public class DataMigrationsAPIController : Controller
    {
        private MedisatErpDbContext _context;

        public DataMigrationsAPIController(MedisatErpDbContext context) {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get(DataSourceLoadOptions loadOptions) {
            var datamigrations = _context.DataMigrations.Select(i => new {
                i.MigrationId,
                i.SourceSystem,
                i.DestinationSystem,
                i.Status,
                i.StartDate,
                i.EndDate,
                i.RecordsMigrated,
                i.ErrorCount,
                i.Log,
                i.MappingRules
            });

            // If underlying data is a large SQL table, specify PrimaryKey and PaginateViaPrimaryKey.
            // This can make SQL execution plans more efficient.
            // For more detailed information, please refer to this discussion: https://github.com/DevExpress/DevExtreme.AspNet.Data/issues/336.
            // loadOptions.PrimaryKey = new[] { "MigrationId" };
            // loadOptions.PaginateViaPrimaryKey = true;

            return Json(await DataSourceLoader.LoadAsync(datamigrations, loadOptions));
        }

        [HttpPost]
        public async Task<IActionResult> Post(string values) {
            var model = new DataMigration();
            var valuesDict = JsonConvert.DeserializeObject<IDictionary>(values);
            PopulateModel(model, valuesDict);

            if(!TryValidateModel(model))
                return BadRequest(GetFullErrorMessage(ModelState));

            var result = _context.DataMigrations.Add(model);
            await _context.SaveChangesAsync();

            return Json(new { result.Entity.MigrationId });
        }

        [HttpPut]
        public async Task<IActionResult> Put(Guid key, string values) {
            var model = await _context.DataMigrations.FirstOrDefaultAsync(item => item.MigrationId == key);
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
            var model = await _context.DataMigrations.FirstOrDefaultAsync(item => item.MigrationId == key);

            _context.DataMigrations.Remove(model);
            await _context.SaveChangesAsync();
        }


        private void PopulateModel(DataMigration model, IDictionary values) {
            string MIGRATION_ID = nameof(DataMigration.MigrationId);
            string SOURCE_SYSTEM = nameof(DataMigration.SourceSystem);
            string DESTINATION_SYSTEM = nameof(DataMigration.DestinationSystem);
            string STATUS = nameof(DataMigration.Status);
            string START_DATE = nameof(DataMigration.StartDate);
            string END_DATE = nameof(DataMigration.EndDate);
            string RECORDS_MIGRATED = nameof(DataMigration.RecordsMigrated);
            string ERROR_COUNT = nameof(DataMigration.ErrorCount);
            string LOG = nameof(DataMigration.Log);
            string MAPPING_RULES = nameof(DataMigration.MappingRules);

            if(values.Contains(MIGRATION_ID)) {
                model.MigrationId = ConvertTo<System.Guid>(values[MIGRATION_ID]);
            }

            if(values.Contains(SOURCE_SYSTEM)) {
                model.SourceSystem = Convert.ToString(values[SOURCE_SYSTEM]);
            }

            if(values.Contains(DESTINATION_SYSTEM)) {
                model.DestinationSystem = Convert.ToString(values[DESTINATION_SYSTEM]);
            }

            if(values.Contains(STATUS)) {
                model.Status = Convert.ToString(values[STATUS]);
            }

            if(values.Contains(START_DATE)) {
                model.StartDate = Convert.ToDateTime(values[START_DATE]);
            }

            if(values.Contains(END_DATE)) {
                model.EndDate = values[END_DATE] != null ? Convert.ToDateTime(values[END_DATE]) : (DateTime?)null;
            }

            if(values.Contains(RECORDS_MIGRATED)) {
                model.RecordsMigrated = values[RECORDS_MIGRATED] != null ? Convert.ToInt32(values[RECORDS_MIGRATED]) : (int?)null;
            }

            if(values.Contains(ERROR_COUNT)) {
                model.ErrorCount = values[ERROR_COUNT] != null ? Convert.ToInt32(values[ERROR_COUNT]) : (int?)null;
            }

            if(values.Contains(LOG)) {
                model.Log = Convert.ToString(values[LOG]);
            }

            if(values.Contains(MAPPING_RULES)) {
                model.MappingRules = Convert.ToString(values[MAPPING_RULES]);
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