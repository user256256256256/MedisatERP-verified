using MedisatERP.Models;
using System;
using System.Collections.Generic;

namespace MedisatERP.Areas.NutritionCompany.Models;

public partial class DietPlan
{
    public Guid DietPlanId { get; set; }

    public Guid? ClientId { get; set; }

    public Guid? ConditionId { get; set; }

    public Guid? AllergyId { get; set; }

    public string DietPlanName { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public string CreatedBy { get; set; }

    public DateTime? LastUpdated { get; set; }

    public virtual Allergy Allergy { get; set; }

    public virtual CompanyClient Client { get; set; }

    public virtual MedicalCondition Condition { get; set; }

    public virtual ICollection<MealPlan> MealPlans { get; set; } = new List<MealPlan>();

    public virtual ICollection<NutritionalProfile> NutritionalProfiles { get; set; } = new List<NutritionalProfile>();
}
