using MedisatERP.Models;
using System;
using System.Collections.Generic;

namespace MedisatERP.Areas.NutritionCompanySystem.Models;

public partial class Allergy
{
    public Guid AllergyId { get; set; }

    public Guid? ClientId { get; set; }

    public string AllergyName { get; set; }

    public string Severity { get; set; }

    public virtual CompanyClient Client { get; set; }

    public virtual ICollection<DietPlan> DietPlans { get; set; } = new List<DietPlan>();

    public virtual ICollection<NutritionalProfile> NutritionalProfiles { get; set; } = new List<NutritionalProfile>();
}
