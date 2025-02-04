using MedisatERP.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MedisatERP.Areas.NutritionCompanySystem.Models;

public partial class MedicalCondition
{
    [Key]
    public Guid ConditionId { get; set; }

    public Guid? ClientId { get; set; }

    public string ConditionName { get; set; }

    public DateTime? DiagnosisDate { get; set; }

    public string Severity { get; set; }

    public virtual CompanyClient Client { get; set; }

    public virtual ICollection<DietPlan> DietPlans { get; set; } = new List<DietPlan>();

    public virtual ICollection<NutritionalProfile> NutritionalProfiles { get; set; } = new List<NutritionalProfile>();
}
