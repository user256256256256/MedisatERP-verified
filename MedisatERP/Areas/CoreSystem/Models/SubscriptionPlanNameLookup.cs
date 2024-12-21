using System;
using System.Collections.Generic;

namespace MedisatERP.Areas.CoreSystem.Models;

public partial class SubscriptionPlanNameLookup
{
    public int Id { get; set; }

    public string PlanName { get; set; }

    public decimal Price { get; set; }

    public virtual ICollection<SubscriptionPlan> SubscriptionPlans { get; set; } = new List<SubscriptionPlan>();
}
