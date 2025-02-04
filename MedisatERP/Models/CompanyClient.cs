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

    public virtual ICollection<Allergy> Allergies { get; set; } = new List<Allergy>();

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual ICollection<ClientMembership> ClientMemberships { get; set; } = new List<ClientMembership>();

    public virtual ICollection<Communication> CommunicationClients { get; set; } = new List<Communication>();

    public virtual ICollection<Communication> CommunicationPractitioners { get; set; } = new List<Communication>();

    public virtual Company Company { get; set; }

    public virtual ICollection<DietPlan> DietPlans { get; set; } = new List<DietPlan>();

    public virtual ICollection<ExercisePlan> ExercisePlans { get; set; } = new List<ExercisePlan>();

    public virtual ICollection<FollowUp> FollowUpClients { get; set; } = new List<FollowUp>();

    public virtual ICollection<FollowUp> FollowUpPractitioners { get; set; } = new List<FollowUp>();

    public virtual ICollection<FoodDatabase> FoodDatabases { get; set; } = new List<FoodDatabase>();

    public virtual ICollection<Goal> Goals { get; set; } = new List<Goal>();

    public virtual ICollection<HealthMetric> HealthMetrics { get; set; } = new List<HealthMetric>();

    public virtual ICollection<HospitalReferral> HospitalReferrals { get; set; } = new List<HospitalReferral>();

    public virtual ICollection<MealLogging> MealLoggings { get; set; } = new List<MealLogging>();

    public virtual ICollection<MealPreference> MealPreferences { get; set; } = new List<MealPreference>();

    public virtual ICollection<MedicalCondition> MedicalConditions { get; set; } = new List<MedicalCondition>();

    public virtual ICollection<NutritionCompanySubscription> NutritionCompanySubscriptions { get; set; } = new List<NutritionCompanySubscription>();

    public virtual ICollection<NutritionalProfile> NutritionalProfiles { get; set; } = new List<NutritionalProfile>();

    public virtual ICollection<PatientBilling> PatientBillings { get; set; } = new List<PatientBilling>();

    public virtual ICollection<PatientDiscount> PatientDiscounts { get; set; } = new List<PatientDiscount>();

    public virtual ICollection<PatientPaymentTransaction> PatientPaymentTransactions { get; set; } = new List<PatientPaymentTransaction>();

    public virtual ICollection<ProgressTracking> ProgressTrackings { get; set; } = new List<ProgressTracking>();

    public virtual ICollection<Report> Reports { get; set; } = new List<Report>();


    public virtual ICollection<Supplement> Supplements { get; set; } = new List<Supplement>();
}