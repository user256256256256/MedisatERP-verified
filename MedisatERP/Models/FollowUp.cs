using System;
using System.Collections.Generic;

namespace MedisatERP.Models;

public partial class FollowUp
{
    public Guid FollowUpId { get; set; }

    public Guid ClientId { get; set; }

    public Guid PractitionerId { get; set; }

    public DateTime FollowUpDate { get; set; }

    public string FollowUpType { get; set; }

    public string CurrentStatus { get; set; }

    public bool? Complying { get; set; }

    public bool? Struggling { get; set; }

    public string Notes { get; set; }

    public bool ReminderSent { get; set; }

    public DateTime? ReminderSentAt { get; set; }

    public string FollowUpOutcome { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual CompanyClient Client { get; set; }

    public virtual CompanyClient Practitioner { get; set; }
}
