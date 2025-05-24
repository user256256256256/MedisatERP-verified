using System;
using System.Collections.Generic;

namespace MedisatERP.Models;

public partial class FoodDatabase
{
    public Guid FoodItemId { get; set; }

    public string FoodName { get; set; }

    public Guid? ClientId { get; set; }

    public string Category { get; set; }

    public int? Calories { get; set; }

    public int? Protein { get; set; }

    public int? Fats { get; set; }

    public int? Carbs { get; set; }

    public virtual CompanyClient Client { get; set; }

    public virtual ICollection<MealLogging> MealLoggings { get; set; } = new List<MealLogging>();
}
