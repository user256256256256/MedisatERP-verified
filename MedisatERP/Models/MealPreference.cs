using System;
using System.Collections.Generic;

namespace MedisatERP.Models;

public partial class MealPreference
{
    public Guid PreferenceId { get; set; }

    public Guid ClientId { get; set; }

    public string Preference { get; set; }

    public string Restriction { get; set; }

    public string Notes { get; set; }

    public virtual CompanyClient Client { get; set; }
}
