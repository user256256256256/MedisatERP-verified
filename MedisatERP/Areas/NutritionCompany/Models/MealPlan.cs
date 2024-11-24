using System;
using System.Collections.Generic;

namespace MedisatERP.Areas.NutritionCompany.Models;

public partial class MealPlan
{
    public Guid MealPlanId { get; set; }

    public Guid? DietPlanId { get; set; }

    public string MealName { get; set; }

    public string FoodCategory { get; set; }

    public int? Calories { get; set; }

    public int? Protein { get; set; }

    public int? Fats { get; set; }

    public int? Carbs { get; set; }

    public virtual DietPlan DietPlan { get; set; }

    public virtual ICollection<MealLogging> MealLoggings { get; set; } = new List<MealLogging>();
}
