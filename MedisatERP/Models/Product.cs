using System;
using System.Collections.Generic;

namespace MedisatERP.Models;

public partial class Product
{
    public Guid ProductId { get; set; }

    public string ProductName { get; set; }

    public string Description { get; set; }

    public decimal Price { get; set; }

    public string Category { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<NutritionCompanySubscription> NutritionCompanySubscriptions { get; set; } = new List<NutritionCompanySubscription>();

    public virtual ICollection<Subscription1> Subscription1s { get; set; } = new List<Subscription1>();
}
