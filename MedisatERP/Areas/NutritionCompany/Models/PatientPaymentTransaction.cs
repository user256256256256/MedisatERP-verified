using MedisatERP.Models;
using System;
using System.Collections.Generic;

namespace MedisatERP.Areas.NutritionCompany.Models;

public partial class PatientPaymentTransaction
{
    public Guid TransactionId { get; set; }

    public Guid ClientId { get; set; }

    public Guid? ProductId { get; set; }

    public Guid? SubscriptionId { get; set; }

    public decimal Amount { get; set; }

    public DateTime TransactionDate { get; set; }

    public string PaymentMethod { get; set; }

    public string PaymentStatus { get; set; }

    public string IssuedBy { get; set; }

    public string Description { get; set; }

    public string TransactionReference { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual CompanyClient Client { get; set; }

    public virtual Product Product { get; set; }

}
