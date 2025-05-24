using System;
using System.Collections.Generic;

namespace MedisatERP.Models;

public partial class Category
{
    public Guid CategoryId { get; set; }

    public string CategoryName { get; set; }

    public string Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<BlogCategory> BlogCategories { get; set; } = new List<BlogCategory>();

    public virtual ICollection<ContentCategory> ContentCategories { get; set; } = new List<ContentCategory>();
}
