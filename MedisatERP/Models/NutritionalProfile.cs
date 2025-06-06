﻿using System;
using System.Collections.Generic;

namespace MedisatERP.Models;

public partial class NutritionalProfile
{
    public Guid ProfileId { get; set; }

    public Guid? ClientId { get; set; }

    public Guid? ConditionId { get; set; }

    public Guid? AllergyId { get; set; }

    public Guid? DietPlanId { get; set; }

    public int? CalorieNeeds { get; set; }

    public int? ProteinNeeds { get; set; }

    public int? FatNeeds { get; set; }

    public int? CarbNeeds { get; set; }

    public decimal? Height { get; set; }

    public decimal? Weight { get; set; }

    public string ActivityLevel { get; set; }

    public int? DailyWaterIntake { get; set; }

    public virtual Allergy Allergy { get; set; }

    public virtual CompanyClient Client { get; set; }

    public virtual MedicalCondition Condition { get; set; }

    public virtual DietPlan DietPlan { get; set; }
}
