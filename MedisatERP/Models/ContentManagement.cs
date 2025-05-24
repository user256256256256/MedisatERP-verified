using System;
using System.Collections.Generic;

namespace MedisatERP.Models;

public partial class ContentManagement
{
    public Guid ContentId { get; set; }

    public string ContentType { get; set; }

    public string ContentTitle { get; set; }

    public string ContentBody { get; set; }

    public string Url { get; set; }

    public Guid AuthorId { get; set; }

    public DateTime PublishedDate { get; set; }

    public string Status { get; set; }

    public string Tags { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<ContentCategory> ContentCategories { get; set; } = new List<ContentCategory>();
}
