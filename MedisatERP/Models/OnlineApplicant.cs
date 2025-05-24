using System;
using System.Collections.Generic;

namespace MedisatERP.Models;

public partial class OnlineApplicant
{
    public Guid Id { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Email { get; set; }

    public string MobilePhoneNo { get; set; }

    public string Address { get; set; }

    public int? Age { get; set; }

    public string Reason { get; set; }

    public DateTime? PreferredSchedule { get; set; }

    public string HowDidYouHearAboutUs { get; set; }

    public bool AcceptPrivacyPolicies { get; set; }

    public DateTime CreatedDate { get; set; }
}
