using MedisatERP.Models;
using System;
using System.Collections.Generic;

namespace MedisatERP.Areas.NutritionCompanySystem.Models;

public partial class Report
{
    public Guid ReportId { get; set; }

    public Guid? ClientId { get; set; }

    public string ReportType { get; set; }

    public DateTime? ReportDate { get; set; }

    public string ReportContent { get; set; }

    public virtual CompanyClient Client { get; set; }
}
