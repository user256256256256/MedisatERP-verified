using System;
using System.Collections.Generic;

namespace MedisatERP.Models;

public partial class HospitalSchedule
{
    public Guid ScheduleId { get; set; }

    public Guid HospitalId { get; set; }

    public string NutritionistId { get; set; }

    public string DayOfWeek { get; set; }

    public TimeSpan StartTime { get; set; }

    public TimeSpan EndTime { get; set; }

    public bool IsRecurring { get; set; }

    public string Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Hospital Hospital { get; set; }

    public virtual AspNetUser Nutritionist { get; set; }
}
