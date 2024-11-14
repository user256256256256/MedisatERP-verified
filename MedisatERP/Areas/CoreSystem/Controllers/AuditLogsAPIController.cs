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
    public class AuditLogsAPIController : Controller
    {
        private MedisatErpDbContext _context;

        public AuditLogsAPIController(MedisatErpDbContext context) {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get(DataSourceLoadOptions loadOptions) {
            var auditlogs = _context.AuditLogs.Select(i => new {
                i.AuditLogId,
                i.UserId,
                i.Action,
                i.Timestamp,
                i.Details,
                i.IpAddress,
                i.DeviceInfo,
                i.EventType,
                i.EntityAffected,
                i.OldValue,
                i.NewValue,
                i.ComplianceStatus
            });

            // If underlying data is a large SQL table, specify PrimaryKey and PaginateViaPrimaryKey.
            // This can make SQL execution plans more efficient.
            // For more detailed information, please refer to this discussion: https://github.com/DevExpress/DevExtreme.AspNet.Data/issues/336.
            // loadOptions.PrimaryKey = new[] { "AuditLogId" };
            // loadOptions.PaginateViaPrimaryKey = true;

            return Json(await DataSourceLoader.LoadAsync(auditlogs, loadOptions));
        }

        [HttpPost]
        public async Task<IActionResult> Post(string values) {
            var model = new AuditLog();
            var valuesDict = JsonConvert.DeserializeObject<IDictionary>(values);
            PopulateModel(model, valuesDict);

            if(!TryValidateModel(model))
                return BadRequest(GetFullErrorMessage(ModelState));

            var result = _context.AuditLogs.Add(model);
            await _context.SaveChangesAsync();

            return Json(new { result.Entity.AuditLogId });
        }

        [HttpPut]
        public async Task<IActionResult> Put(Guid key, string values) {
            var model = await _context.AuditLogs.FirstOrDefaultAsync(item => item.AuditLogId == key);
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
            var model = await _context.AuditLogs.FirstOrDefaultAsync(item => item.AuditLogId == key);

            _context.AuditLogs.Remove(model);
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

        private void PopulateModel(AuditLog model, IDictionary values) {
            string AUDIT_LOG_ID = nameof(AuditLog.AuditLogId);
            string USER_ID = nameof(AuditLog.UserId);
            string ACTION = nameof(AuditLog.Action);
            string TIMESTAMP = nameof(AuditLog.Timestamp);
            string DETAILS = nameof(AuditLog.Details);
            string IP_ADDRESS = nameof(AuditLog.IpAddress);
            string DEVICE_INFO = nameof(AuditLog.DeviceInfo);
            string EVENT_TYPE = nameof(AuditLog.EventType);
            string ENTITY_AFFECTED = nameof(AuditLog.EntityAffected);
            string OLD_VALUE = nameof(AuditLog.OldValue);
            string NEW_VALUE = nameof(AuditLog.NewValue);
            string COMPLIANCE_STATUS = nameof(AuditLog.ComplianceStatus);

            if(values.Contains(AUDIT_LOG_ID)) {
                model.AuditLogId = ConvertTo<System.Guid>(values[AUDIT_LOG_ID]);
            }

            if(values.Contains(USER_ID)) {
                model.UserId = Convert.ToString(values[USER_ID]);
            }

            if(values.Contains(ACTION)) {
                model.Action = Convert.ToString(values[ACTION]);
            }

            if(values.Contains(TIMESTAMP)) {
                model.Timestamp = Convert.ToDateTime(values[TIMESTAMP]);
            }

            if(values.Contains(DETAILS)) {
                model.Details = Convert.ToString(values[DETAILS]);
            }

            if(values.Contains(IP_ADDRESS)) {
                model.IpAddress = Convert.ToString(values[IP_ADDRESS]);
            }

            if(values.Contains(DEVICE_INFO)) {
                model.DeviceInfo = Convert.ToString(values[DEVICE_INFO]);
            }

            if(values.Contains(EVENT_TYPE)) {
                model.EventType = Convert.ToString(values[EVENT_TYPE]);
            }

            if(values.Contains(ENTITY_AFFECTED)) {
                model.EntityAffected = Convert.ToString(values[ENTITY_AFFECTED]);
            }

            if(values.Contains(OLD_VALUE)) {
                model.OldValue = Convert.ToString(values[OLD_VALUE]);
            }

            if(values.Contains(NEW_VALUE)) {
                model.NewValue = Convert.ToString(values[NEW_VALUE]);
            }

            if(values.Contains(COMPLIANCE_STATUS)) {
                model.ComplianceStatus = Convert.ToString(values[COMPLIANCE_STATUS]);
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