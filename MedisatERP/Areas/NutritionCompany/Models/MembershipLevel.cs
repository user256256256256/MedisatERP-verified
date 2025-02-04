using System;
using System.Collections.Generic;

namespace MedisatERP.Areas.NutritionCompany.Models;

public partial class MembershipLevel
{
    public Guid MembershipLevelId { get; set; }

    public string LevelName { get; set; }

    public string Description { get; set; }

    public string Benefits { get; set; }

    public decimal Price { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<ClientMembership> ClientMemberships { get; set; } = new List<ClientMembership>();
}
