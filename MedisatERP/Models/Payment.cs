using System;
using System.Collections.Generic;

namespace MedisatERP.Models;

public partial class Payment
{
    public Guid Id { get; set; }

    public Guid? SubscriptionId { get; set; }

    public DateTime PaymentDate { get; set; }

    public int PaymentStatusId { get; set; }

    public int PaymentMethodId { get; set; }

    public string TransactionId { get; set; }

    public bool IsRefunded { get; set; }

    public string Method { get; set; }

    public string Status { get; set; }

    public virtual Subscription Subscription { get; set; }
}
