﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MedisatERP.Areas.CoreSystem.Models;

public partial class AuditLog
{
    [Key]
    public Guid AuditLogId { get; set; }

    public string UserId { get; set; }

    public string Action { get; set; }

    public DateTime Timestamp { get; set; }

    public string Details { get; set; }

    public string IpAddress { get; set; }

    public string DeviceInfo { get; set; }

    public string EventType { get; set; }

    public string EntityAffected { get; set; }

    public string OldValue { get; set; }

    public string NewValue { get; set; }

    public string ComplianceStatus { get; set; }

    public virtual AspNetUser User { get; set; }
}