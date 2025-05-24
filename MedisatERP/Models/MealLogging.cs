using System;
using System.Collections.Generic;

namespace MedisatERP.Models;

public partial class MealLogging
{
    public Guid LogId { get; set; }

    public Guid? ClientId { get; set; }

    public Guid? MealPlanId { get; set; }

    public Guid? FoodItemId { get; set; }

    public DateTime? LogDate { get; set; }

    public int? PortionSize { get; set; }

    public int? CaloriesConsumed { get; set; }

    public int? ProteinConsumed { get; set; }

    public int? FatsConsumed { get; set; }

    public int? CarbsConsumed { get; set; }

    public string MealTime { get; set; }

    public virtual CompanyClient Client { get; set; }

    public virtual FoodDatabase FoodItem { get; set; }

    public virtual MealPlan MealPlan { get; set; }
}
