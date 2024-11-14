using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MedisatERP.Areas.CoreSystem.Models;

public partial class DataMigration
{
    [Key]
    public Guid MigrationId { get; set; }

    public string SourceSystem { get; set; }

    public string DestinationSystem { get; set; }

    public string Status { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public int? RecordsMigrated { get; set; }

    public int? ErrorCount { get; set; }

    public string Log { get; set; }

    public string MappingRules { get; set; }
}
