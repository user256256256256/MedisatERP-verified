using System;
using System.Collections.Generic;

namespace MedisatERP.Areas.NutritionCompanySystem.Models;

public partial class WorkplaceLookup
{
    public int Id { get; set; }

    public string Workplace { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}
