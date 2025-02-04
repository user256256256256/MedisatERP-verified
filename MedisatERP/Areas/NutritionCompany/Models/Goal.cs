using MedisatERP.Models;
using System;
using System.Collections.Generic;

namespace MedisatERP.Areas.NutritionCompany.Models;

public partial class Goal
{
    public Guid GoalId { get; set; }

    public Guid ClientId { get; set; }

    public string GoalDescription { get; set; }

    public DateTime TargetDate { get; set; }

    public bool IsAchieved { get; set; }

    public virtual CompanyClient Client { get; set; }
}
