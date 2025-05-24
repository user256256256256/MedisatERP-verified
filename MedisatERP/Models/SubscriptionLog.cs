using System;
using System.Collections.Generic;

namespace MedisatERP.Models;

public partial class SubscriptionLog
{
    public Guid Id { get; set; }

    public Guid SubscriptionId { get; set; }

    public DateTime LogDate { get; set; }

    public string Activity { get; set; }

    public virtual Subscription Subscription { get; set; }
}
