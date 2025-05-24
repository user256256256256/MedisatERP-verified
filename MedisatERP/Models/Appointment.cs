using System;
using System.Collections.Generic;

namespace MedisatERP.Models;

public partial class Appointment
{
    public Guid AppointmentId { get; set; }

    public Guid ClientId { get; set; }

    public string NutritionistId { get; set; }

    public DateTime ScheduledDate { get; set; }

    public int WorkplaceId { get; set; }

    public string Status { get; set; }

    public string Priority { get; set; }

    public bool ReminderSent { get; set; }

    public DateTime? ReminderSentAt { get; set; }

    public string Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? Duration { get; set; }

    public Guid CompanyId { get; set; }

    public virtual CompanyClient Client { get; set; }

    public virtual Company Company { get; set; }

    public virtual AspNetUser Nutritionist { get; set; }

    public virtual WorkplaceLookup Workplace { get; set; }
}
