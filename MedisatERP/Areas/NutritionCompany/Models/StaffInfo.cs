using System;
using System.Collections.Generic;
using MedisatERP.Areas.CoreSystem.Models;

namespace MedisatERP.Areas.NutritionCompany.Models;

public partial class StaffInfo
{
    public Guid StaffId { get; set; }

    public string UserId { get; set; }

    public DateTime HireDate { get; set; }

    public string Department { get; set; }

    public string Position { get; set; }

    public decimal? Salary { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Blog> Blogs { get; set; } = new List<Blog>();

    public virtual ICollection<ContentManagement> ContentManagements { get; set; } = new List<ContentManagement>();

    public virtual ICollection<ContractManagement> ContractManagements { get; set; } = new List<ContractManagement>();

    public virtual ICollection<Image> Images { get; set; } = new List<Image>();

    public virtual ICollection<LeaveManagement> LeaveManagements { get; set; } = new List<LeaveManagement>();

    public virtual ICollection<PerformanceTracking> PerformanceTrackings { get; set; } = new List<PerformanceTracking>();

    public virtual AspNetUser User { get; set; }
}
