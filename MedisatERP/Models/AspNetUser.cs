using System;
using System.Collections.Generic;

namespace MedisatERP.Models;

public partial class AspNetUser
{
    public string Id { get; set; }

    public string UserName { get; set; }

    public string NormalizedUserName { get; set; }

    public string Email { get; set; }

    public string NormalizedEmail { get; set; }

    public bool EmailConfirmed { get; set; }

    public string PasswordHash { get; set; }

    public string SecurityStamp { get; set; }

    public string ConcurrencyStamp { get; set; }

    public string PhoneNumber { get; set; }

    public bool PhoneNumberConfirmed { get; set; }

    public bool TwoFactorEnabled { get; set; }

    public DateTimeOffset? LockoutEnd { get; set; }

    public bool LockoutEnabled { get; set; }

    public int AccessFailedCount { get; set; }

    public Guid? CompanyId { get; set; }

    public string ProfileImagePath { get; set; }

    public string BioData { get; set; }

    public string Gender { get; set; }

    public string Country { get; set; }

    public string Name { get; set; }

    public DateTime? Dob { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual ICollection<AspNetUserClaim> AspNetUserClaims { get; set; } = new List<AspNetUserClaim>();

    public virtual ICollection<AspNetUserLogin> AspNetUserLogins { get; set; } = new List<AspNetUserLogin>();

    public virtual ICollection<AspNetUserToken> AspNetUserTokens { get; set; } = new List<AspNetUserToken>();

    public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual ICollection<HospitalReferral> HospitalReferrals { get; set; } = new List<HospitalReferral>();

    public virtual ICollection<HospitalSchedule> HospitalSchedules { get; set; } = new List<HospitalSchedule>();

    public virtual ICollection<PartnershipAgreement> PartnershipAgreements { get; set; } = new List<PartnershipAgreement>();

    public virtual ICollection<AspNetRole> Roles { get; set; } = new List<AspNetRole>();
}
