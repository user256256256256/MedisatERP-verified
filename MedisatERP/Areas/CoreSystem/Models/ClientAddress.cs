using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MedisatERP.Areas.CoreSystem.Models;

public partial class ClientAddress
{

    [Key]
    public Guid AddressId { get; set; }

    public Guid ClientId { get; set; }

    public string Street { get; set; }

    public string City { get; set; }

    public string State { get; set; }

    public string PostalCode { get; set; }

    public string Country { get; set; }

    public virtual ICollection<CompanyClient> CompanyClients { get; set; } = new List<CompanyClient>();
}
