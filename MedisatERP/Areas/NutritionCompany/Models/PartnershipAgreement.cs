using MedisatERP.Areas.CoreSystem.Models;
using System;
using System.Collections.Generic;

namespace MedisatERP.Areas.NutritionCompany.Models;

public partial class PartnershipAgreement
{
    public Guid AgreementId { get; set; }

    public Guid HospitalId { get; set; }

    public string NutritionistId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public DateTime? RenewalDate { get; set; }

    public string AgreementStatus { get; set; }

    public string Terms { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Hospital Hospital { get; set; }

    public virtual AspNetUser Nutritionist { get; set; }
}
