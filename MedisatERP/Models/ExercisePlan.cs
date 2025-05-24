using System;
using System.Collections.Generic;

namespace MedisatERP.Models;

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
