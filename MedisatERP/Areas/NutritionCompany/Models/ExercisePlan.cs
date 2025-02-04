using MedisatERP.Models;
using System;
using System.Collections.Generic;

namespace MedisatERP.Areas.NutritionCompany.Models;

public partial class ExercisePlan
{
    public Guid ExercisePlanId { get; set; }

    public Guid ClientId { get; set; }

    public string ExerciseName { get; set; }

    public int Duration { get; set; }

    public string Frequency { get; set; }

    public DateTime CreatedDate { get; set; }

    public virtual CompanyClient Client { get; set; }
}
