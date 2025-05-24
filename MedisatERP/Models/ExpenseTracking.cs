using System;
using System.Collections.Generic;

namespace MedisatERP.Models;

public partial class ExpenseTracking
{
    public Guid ExpenseId { get; set; }

    public DateTime ExpenseDate { get; set; }

    public decimal Amount { get; set; }

    public string Category { get; set; }

    public string Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
