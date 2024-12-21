using System;
using System.Collections.Generic;

namespace MedisatERP.Areas.CoreSystem.Models;

public partial class PaymentMethodLookup
{
    public int Id { get; set; }

    public string Method { get; set; }

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
