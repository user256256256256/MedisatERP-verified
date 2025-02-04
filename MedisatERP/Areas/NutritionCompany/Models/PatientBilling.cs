using MedisatERP.Models;
using System;
using System.Collections.Generic;

namespace MedisatERP.Areas.NutritionCompany.Models;

public partial class PatientBilling
{
    public Guid BillingId { get; set; }

    public Guid PatientId { get; set; }

    public Guid FeeId { get; set; }

    public DateTime BillingDate { get; set; }

    public decimal AmountBilled { get; set; }

    public string PaymentStatus { get; set; }

    public DateTime? DueDate { get; set; }

    public Guid? DiscountId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual PatientDiscount Discount { get; set; }

    public virtual PatientFeeStructure Fee { get; set; }

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    public virtual CompanyClient Patient { get; set; }
}
