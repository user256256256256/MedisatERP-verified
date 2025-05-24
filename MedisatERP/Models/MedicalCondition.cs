using System;
using System.Collections.Generic;

namespace MedisatERP.Models;

public partial class MedicalCondition
{
    public Guid ConditionId { get; set; }

    public Guid? ClientId { get; set; }

    public string ConditionName { get; set; }

    public DateTime? DiagnosisDate { get; set; }

    public string Severity { get; set; }

    public virtual CompanyClient Client { get; set; }

    public virtual ICollection<DietPlan> DietPlans { get; set; } = new List<DietPlan>();

    public virtual ICollection<NutritionalProfile> NutritionalProfiles { get; set; } = new List<NutritionalProfile>();
}
