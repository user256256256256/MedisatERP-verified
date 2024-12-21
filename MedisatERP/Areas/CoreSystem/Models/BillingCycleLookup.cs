using System;
using System.Collections.Generic;

namespace MedisatERP.Areas.CoreSystem.Models;

public partial class BillingCycleLookup
{
    public int Id { get; set; }

    public string CycleName { get; set; }

    public virtual ICollection<SubscriptionPlan> SubscriptionPlans { get; set; } = new List<SubscriptionPlan>();
}
