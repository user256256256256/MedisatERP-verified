﻿using System;
using System.Collections.Generic;

namespace MedisatERP.Models;

public partial class Subscription
{
    public Guid Id { get; set; }

    public Guid CompanyId { get; set; }

    public Guid SubscriptionPlanId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public bool IsActive { get; set; }

    public string PaymentStatus { get; set; }

    public virtual Company Company { get; set; }

    public virtual ICollection<SubscriptionLog> SubscriptionLogs { get; set; } = new List<SubscriptionLog>();

    public virtual ICollection<SubscriptionPayment> SubscriptionPayments { get; set; } = new List<SubscriptionPayment>();

    public virtual SubscriptionPlan SubscriptionPlan { get; set; }
}
