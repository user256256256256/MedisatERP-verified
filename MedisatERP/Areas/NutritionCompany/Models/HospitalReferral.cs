using MedisatERP.Areas.CoreSystem.Models;
using MedisatERP.Models;
using System;
using System.Collections.Generic;

namespace MedisatERP.Areas.NutritionCompany.Models;

public partial class HospitalReferral
{
    public Guid ReferralId { get; set; }

    public Guid HospitalId { get; set; }

    public string NutritionistId { get; set; }

    public Guid ClientId { get; set; }

    public DateTime ReferralDate { get; set; }

    public string Reason { get; set; }

    public string ReferralStatus { get; set; }

    public DateTime? FollowUpDate { get; set; }

    public string Outcome { get; set; }

    public string Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual CompanyClient Client { get; set; }

    public virtual Hospital Hospital { get; set; }

    public virtual AspNetUser Nutritionist { get; set; }
}
