using System;
using System.Collections.Generic;

namespace MedisatERP.Models;

public partial class DataMigration
{
    public Guid Id { get; set; }

    public string SourceSystem { get; set; }

    public string DestinationSystem { get; set; }

    public string Status { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public int? RecordsMigrated { get; set; }

    public int? ErrorCount { get; set; }

    public string Log { get; set; }

    public string MappingRules { get; set; }

    public Guid? CompanyId { get; set; }

    public virtual Company Company { get; set; }
}
