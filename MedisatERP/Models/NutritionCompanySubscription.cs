using System;
using System.Collections.Generic;

namespace MedisatERP.Models;

public partial class NutritionCompanySubscription
{
    public Guid SubscriptionId { get; set; }

    public Guid ClientId { get; set; }

    public Guid ProductId { get; set; }

    public DateTime SubscriptionStartDate { get; set; }

    public DateTime? SubscriptionEndDate { get; set; }

    public bool IsActive { get; set; }

    public DateTime? RenewalDate { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual CompanyClient Client { get; set; }

    public virtual Product Product { get; set; }
}
