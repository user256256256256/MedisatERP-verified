using MedisatERP.Models;
using System;
using System.Collections.Generic;

namespace MedisatERP.Areas.NutritionCompany.Models;

public partial class PatientDiscount
{
    public Guid DiscountId { get; set; }

    public Guid PatientId { get; set; }

    public Guid FeeId { get; set; }

    public decimal DiscountAmount { get; set; }

    public string Reason { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual PatientFeeStructure Fee { get; set; }

    public virtual CompanyClient Patient { get; set; }

    public virtual ICollection<PatientBilling> PatientBillings { get; set; } = new List<PatientBilling>();
}
