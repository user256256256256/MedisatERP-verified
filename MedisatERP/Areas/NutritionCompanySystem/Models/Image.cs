using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MedisatERP.Areas.NutritionCompanySystem.Models;

public partial class Image
{
    [Key]
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
