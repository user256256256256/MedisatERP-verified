using System;
using System.Collections.Generic;

namespace MedisatERP.Areas.NutritionCompanySystem.Models;

public partial class Blog
{
    public Guid BlogId { get; set; }

    public string Title { get; set; }

    public string Content { get; set; }

    public Guid AuthorId { get; set; }

    public DateTime PublishedDate { get; set; }

    public string Status { get; set; }

    public string Tags { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<BlogCategory> BlogCategories { get; set; } = new List<BlogCategory>();
}
