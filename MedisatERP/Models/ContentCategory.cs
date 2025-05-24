using System;
using System.Collections.Generic;

namespace MedisatERP.Models;

public partial class ContentCategory
{
    public Guid ContentCategoryId { get; set; }

    public Guid ContentId { get; set; }

    public Guid CategoryId { get; set; }

    public virtual Category Category { get; set; }

    public virtual ContentManagement Content { get; set; }
}
