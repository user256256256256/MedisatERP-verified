using System;
using System.Collections.Generic;

namespace MedisatERP.Areas.NutritionCompany.Models;

public partial class ContractManagement
{
    public Guid ContractId { get; set; }

    public Guid StaffId { get; set; }

    public string ContractType { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public decimal Salary { get; set; }

    public string Benefits { get; set; }

    public DateTime? RenewalDate { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual StaffInfo Staff { get; set; }
}
