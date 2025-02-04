using System;
using System.Collections.Generic;

namespace MedisatERP.Areas.NutritionCompany.Models;

public partial class LeaveManagement
{
    public Guid LeaveId { get; set; }

    public Guid StaffId { get; set; }

    public string LeaveType { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public string Status { get; set; }

    public string Comments { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual StaffInfo Staff { get; set; }
}
