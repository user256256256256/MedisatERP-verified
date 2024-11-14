using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MedisatERP.Areas.CoreSystem.Models;

public partial class CompanyAddress
{
    [Key]
    public Guid AddressId { get; set; }

    public string Street { get; set; }

    public string City { get; set; }

    public string State { get; set; }

    public string PostalCode { get; set; }

    public string Country { get; set; }

    public virtual ICollection<Company> Companies { get; set; } = new List<Company>();
}
