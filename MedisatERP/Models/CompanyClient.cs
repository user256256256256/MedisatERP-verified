using MedisatERP.Areas.CoreSystem.Models;
using MedisatERP.Areas.NutritionCompany.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MedisatERP.Models;

public partial class CompanyClient
{
    [Key]
    public Guid ClientId { get; set; }

    public Guid CompanyId { get; set; }

    public string ClientName { get; set; }

    public DateTime DateOfBirth { get; set; }

    public string Gender { get; set; }

    public string Email { get; set; }

    public string PhoneNumber { get; set; }

    public Guid AddressId { get; set; }

    public string EmergencyContactName { get; set; }

    public string EmergencyContactPhone { get; set; }

    public string MaritalStatus { get; set; }

    public string Nationality { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ClientAddress Address { get; set; }

    public virtual Company Company { get; set; }

    public virtual ICollection<Allergy> Allergies { get; set; } = new List<Allergy>();

    public virtual ICollection<DietPlan> DietPlans { get; set; } = new List<DietPlan>();

    public virtual ICollection<FoodDatabase> FoodDatabases { get; set; } = new List<FoodDatabase>();

    public virtual ICollection<MealLogging> MealLoggings { get; set; } = new List<MealLogging>();

    public virtual ICollection<MedicalCondition> MedicalConditions { get; set; } = new List<MedicalCondition>();

    public virtual ICollection<NutritionalProfile> NutritionalProfiles { get; set; } = new List<NutritionalProfile>();

    public virtual ICollection<Report> Reports { get; set; } = new List<Report>();
}
