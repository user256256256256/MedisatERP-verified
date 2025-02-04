using MedisatERP.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MedisatERP.Areas.NutritionCompanySystem.Models;

public partial class MealPreference
{
    [Key]
    public Guid PreferenceId { get; set; }

    public Guid ClientId { get; set; }

    public string Preference { get; set; }

    public string Restriction { get; set; }

    public string Notes { get; set; }

    public virtual CompanyClient Client { get; set; }
}
