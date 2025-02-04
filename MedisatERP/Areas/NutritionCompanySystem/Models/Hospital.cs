using System;
using System.Collections.Generic;

namespace MedisatERP.Areas.NutritionCompanySystem.Models;

public partial class Hospital
{

    public Guid HospitalId { get; set; }

    public string Name { get; set; }

    public string Address { get; set; }

    public string City { get; set; }

    public string State { get; set; }

    public string PostalCode { get; set; }

    public string Country { get; set; }

    public string PhoneNumber { get; set; }

    public string Email { get; set; }

    public string Website { get; set; }

    public string ContactName { get; set; }

    public string ContactPosition { get; set; }

    public string ContactPhoneNumber { get; set; }

    public string ContactEmail { get; set; }

    public string ContactNotes { get; set; }

    public string HospitalType { get; set; }

    public string Specialties { get; set; }

    public string AccreditationStatus { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<HospitalReferral> HospitalReferrals { get; set; } = new List<HospitalReferral>();

    public virtual ICollection<HospitalSchedule> HospitalSchedules { get; set; } = new List<HospitalSchedule>();

    public virtual ICollection<PartnershipAgreement> PartnershipAgreements { get; set; } = new List<PartnershipAgreement>();
}
