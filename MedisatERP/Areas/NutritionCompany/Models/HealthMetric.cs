using MedisatERP.Models;
using System;
using System.Collections.Generic;

namespace MedisatERP.Areas.NutritionCompany.Models;

public partial class HealthMetric
{
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
