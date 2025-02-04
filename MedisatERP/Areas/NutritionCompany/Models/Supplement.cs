using MedisatERP.Models;
using System;
using System.Collections.Generic;

namespace MedisatERP.Areas.NutritionCompany.Models;

public partial class Supplement
{
    public Guid SupplementId { get; set; }

    public Guid ClientId { get; set; }

    public string SupplementName { get; set; }

    public string Dosage { get; set; }

    public string Frequency { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public virtual CompanyClient Client { get; set; }
}
