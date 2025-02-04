using MedisatERP.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MedisatERP.Areas.NutritionCompanySystem.Models;

public partial class ProgressTracking
{
    [Key]
    public Guid ProgressId { get; set; }

    public Guid ClientId { get; set; }

    public DateTime DateRecorded { get; set; }

    public decimal? Weight { get; set; }

    public decimal? Bmi { get; set; }

    public decimal? BodyFatPercentage { get; set; }

    public string Notes { get; set; }

    public virtual CompanyClient Client { get; set; }
}
