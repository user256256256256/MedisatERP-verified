﻿using System;
using System.Collections.Generic;
using MedisatERP.Areas.AdministratorSystem.Models;
using MedisatERP.Models;

namespace MedisatERP.Areas.NutritionCompanySystem.Models;

public partial class ClientMembership
{
    public Guid ClientMembershipId { get; set; }

    public Guid ClientId { get; set; }

    public Guid MembershipLevelId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public bool? IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual CompanyClient Client { get; set; }

    public virtual MembershipLevel MembershipLevel { get; set; }
}
