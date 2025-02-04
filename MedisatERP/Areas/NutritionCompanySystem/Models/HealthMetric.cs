using MedisatERP.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MedisatERP.Areas.NutritionCompanySystem.Models;

public partial class HealthMetric
{
    [Key]
    public Guid MetricId { get; set; }

    public Guid ClientId { get; set; }

    public DateTime MeasurementDate { get; set; }

    public string BloodPressure { get; set; }

    public int? HeartRate { get; set; }

    public int? Cholesterol { get; set; }

    public int? BloodSugar { get; set; }

    public string Notes { get; set; }

    public virtual CompanyClient Client { get; set; }
}
