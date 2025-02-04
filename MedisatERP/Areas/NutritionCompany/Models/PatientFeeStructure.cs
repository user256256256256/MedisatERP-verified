using System;
using System.Collections.Generic;

namespace MedisatERP.Areas.NutritionCompany.Models;

public partial class PatientFeeStructure
{
    public Guid FeeId { get; set; }

    public string ServiceName { get; set; }

    public string Description { get; set; }

    public decimal FeeAmount { get; set; }

    public DateTime EffectiveDate { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<PatientBilling> PatientBillings { get; set; } = new List<PatientBilling>();

    public virtual ICollection<PatientDiscount> PatientDiscounts { get; set; } = new List<PatientDiscount>();
}
