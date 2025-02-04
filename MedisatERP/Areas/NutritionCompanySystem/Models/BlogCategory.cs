using System;
using System.Collections.Generic;

namespace MedisatERP.Areas.NutritionCompanySystem.Models;

public partial class BlogCategory
{
    public Guid BlogCategoryId { get; set; }

    public Guid BlogId { get; set; }

    public Guid CategoryId { get; set; }

    public virtual Blog Blog { get; set; }

    public virtual Category Category { get; set; }
}
