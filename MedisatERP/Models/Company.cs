using System;
using System.Collections.Generic;

namespace MedisatERP.Models;

public partial class Company
{
    public Guid CompanyId { get; set; }

    public string CompanyName { get; set; }

    public string ContactPerson { get; set; }

    public string CompanyEmail { get; set; }

    public string CompanyPhone { get; set; }

    public string ApiCode { get; set; }

    public string CompanyInitials { get; set; }

    public string Motto { get; set; }

    public string CompanyType { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string CompanyLogoFilePath { get; set; }

    public string Street { get; set; }

    public string City { get; set; }

    public string State { get; set; }

    public string PostalCode { get; set; }

    public string Country { get; set; }

    public string Status { get; set; }

    public string CompanyWebsite { get; set; }

    public string AboutCompany { get; set; }

    public string ContactPersonPhone { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();

    public virtual ICollection<DataMigration> DataMigrations { get; set; } = new List<DataMigration>();

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();

    public virtual ICollection<TrialNotification> TrialNotifications { get; set; } = new List<TrialNotification>();
}
