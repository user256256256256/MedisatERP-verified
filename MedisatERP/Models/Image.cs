using System;
using System.Collections.Generic;

namespace MedisatERP.Models;

public partial class Image
{
    public Guid ImageId { get; set; }

    public string ImageUrl { get; set; }

    public string AltText { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public Guid UploadedBy { get; set; }

    public DateTime UploadDate { get; set; }

    public string Tags { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
