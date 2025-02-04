using System;
using System.Collections.Generic;

namespace MedisatERP.Areas.NutritionCompanySystem.Models;

public partial class Recipe
{
    public Guid RecipeId { get; set; }

    public string RecipeName { get; set; }

    public string Ingredients { get; set; }

    public string Instructions { get; set; }

    public int? Calories { get; set; }

    public int? Protein { get; set; }

    public int? Fats { get; set; }

    public int? Carbs { get; set; }
}
