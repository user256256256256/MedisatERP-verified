using System;
using System.Collections.Generic;

namespace MedisatERP.Areas.AdministratorSystem.Models;

public partial class BillingCycleLookup
{
    public int Id { get; set; }

    public string CycleName { get; set; }

    public virtual ICollection<SubscriptionPlan> SubscriptionPlans { get; set; } = new List<SubscriptionPlan>();
}
