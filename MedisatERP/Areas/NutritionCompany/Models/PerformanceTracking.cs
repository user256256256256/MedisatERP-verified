using System;
using System.Collections.Generic;

namespace MedisatERP.Areas.NutritionCompany.Models;

public partial class PerformanceTracking
{
    public Guid PerformanceId { get; set; }

    public Guid StaffId { get; set; }

    public DateTime ReviewDate { get; set; }

    public int PerformanceScore { get; set; }

    public string Comments { get; set; }

    public string ReviewedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual StaffInfo Staff { get; set; }
}
