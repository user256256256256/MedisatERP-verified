using System;
using System.Collections.Generic;
using MedisatERP.Areas.CoreSystem.Models;
using MedisatERP.Areas.NutritionCompany.Models;
using MedisatERP.Models;
using Microsoft.EntityFrameworkCore;

namespace MedisatERP.Data;

public partial class MedisatErpDbContext : DbContext
{
    public MedisatErpDbContext()
    {
    }

    public MedisatErpDbContext(DbContextOptions<MedisatErpDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AspNetRole> AspNetRoles { get; set; }

    public virtual DbSet<AspNetRoleClaim> AspNetRoleClaims { get; set; }

    public virtual DbSet<AspNetUser> AspNetUsers { get; set; }

    public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }

    public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }

    public virtual DbSet<AspNetUserToken> AspNetUserTokens { get; set; }

    public virtual DbSet<AuditLog> AuditLogs { get; set; }

    public virtual DbSet<ClientAddress> ClientAddresses { get; set; }

    public virtual DbSet<Company> Companies { get; set; }

    public virtual DbSet<CompanyAddress> CompanyAddresses { get; set; }

    public virtual DbSet<CompanyClient> CompanyClients { get; set; }

    public virtual DbSet<DataMigration> DataMigrations { get; set; }

    public virtual DbSet<Feedback> Feedbacks { get; set; }

    public virtual DbSet<Allergy> Allergies { get; set; }

    public virtual DbSet<DietPlan> DietPlans { get; set; }

    public virtual DbSet<FoodDatabase> FoodDatabases { get; set; }

    public virtual DbSet<MealLogging> MealLoggings { get; set; }

    public virtual DbSet<MealPlan> MealPlans { get; set; }

    public virtual DbSet<MedicalCondition> MedicalConditions { get; set; }

    public virtual DbSet<NutritionalProfile> NutritionalProfiles { get; set; }

    public virtual DbSet<Report> Reports { get; set; }

    public virtual DbSet<BillingCycleLookup> BillingCycleLookups { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<PaymentMethodLookup> PaymentMethodLookups { get; set; }

    public virtual DbSet<PaymentStatusLookup> PaymentStatusLookups { get; set; }

    public virtual DbSet<Subscription> Subscriptions { get; set; }

    public virtual DbSet<SubscriptionActivityLookup> SubscriptionActivityLookups { get; set; }

    public virtual DbSet<SubscriptionLog> SubscriptionLogs { get; set; }

    public virtual DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }

    public virtual DbSet<SubscriptionPlanNameLookup> SubscriptionPlanNameLookups { get; set; }

    public virtual DbSet<TrialNotification> TrialNotifications { get; set; }

    public virtual DbSet<TrialNotificationLookup> TrialNotificationLookups { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Data Source=medisaterp.lyfexafrica.com;uid=adminMedisatERP;pwd=Planchinobo256;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AspNetRole>(entity =>
        {
            entity.HasIndex(e => e.NormalizedName, "RoleNameIndex")
                .IsUnique()
                .HasFilter("([NormalizedName] IS NOT NULL)");

            entity.Property(e => e.Name).HasMaxLength(256);
            entity.Property(e => e.NormalizedName).HasMaxLength(256);
        });

        modelBuilder.Entity<AspNetRoleClaim>(entity =>
        {
            entity.HasIndex(e => e.RoleId, "IX_AspNetRoleClaims_RoleId");

            entity.Property(e => e.RoleId).IsRequired();

            entity.HasOne(d => d.Role).WithMany(p => p.AspNetRoleClaims).HasForeignKey(d => d.RoleId);
        });

        // Configure relationships
        modelBuilder.Entity<AspNetUser>(entity =>
        {
            entity.HasIndex(e => e.NormalizedEmail).HasDatabaseName("EmailIndex");

            entity.HasIndex(e => e.NormalizedUserName).HasDatabaseName("UserNameIndex")
                .IsUnique()
                .HasFilter("[NormalizedUserName] IS NOT NULL");

            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.NormalizedEmail).HasMaxLength(256);
            entity.Property(e => e.NormalizedUserName).HasMaxLength(256);
            entity.Property(e => e.UserName).HasMaxLength(256);

            entity.HasMany(d => d.Roles).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "AspNetUserRole",
                    r => r.HasOne<AspNetRole>().WithMany().HasForeignKey("RoleId"),
                    l => l.HasOne<AspNetUser>().WithMany().HasForeignKey("UserId"),
                    j =>
                    {
                        j.HasKey("UserId", "RoleId");
                        j.ToTable("AspNetUserRoles");
                        j.HasIndex(new[] { "RoleId" }, "IX_AspNetUserRoles_RoleId");
                    });

            // Add the CompanyId property and configure the relationship
            entity.Property(e => e.CompanyId).IsRequired(false);

            entity.HasOne(d => d.Company)
                .WithMany(p => p.Users)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure the new ProfileImagePath property
            entity.Property(e => e.ProfileImagePath)
                .HasMaxLength(255) 
                .IsRequired(false);

            // Configure the new BioData property
            entity.Property(e => e.BioData)
                .HasMaxLength(500)  // Adjust the max length as needed
                .IsRequired(false);
        });

        modelBuilder.Entity<AspNetUserClaim>(entity =>
        {
            entity.HasIndex(e => e.UserId, "IX_AspNetUserClaims_UserId");

            entity.Property(e => e.UserId).IsRequired();

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserClaims).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<AspNetUserLogin>(entity =>
        {
            entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });

            entity.HasIndex(e => e.UserId, "IX_AspNetUserLogins_UserId");

            entity.Property(e => e.UserId).IsRequired();

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserLogins).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<AspNetUserToken>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserTokens).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.AuditLogId).HasName("PK__AuditLog__EB5F6CBD75CDDDCB");

            entity.Property(e => e.AuditLogId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Action)
                .IsRequired()
                .HasMaxLength(256);
            entity.Property(e => e.CompanyId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.ComplianceStatus).HasMaxLength(100);
            entity.Property(e => e.DeviceInfo).HasMaxLength(512);
            entity.Property(e => e.EntityAffected)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.EventType)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.IpAddress).HasMaxLength(45);
            entity.Property(e => e.Timestamp)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UserId)
                .IsRequired()
                .HasMaxLength(450);

            entity.HasOne(d => d.Company).WithMany(p => p.AuditLogs)
                .HasForeignKey(d => d.CompanyId)
                .HasConstraintName("FK_AuditLogs_Company");

            entity.HasOne(d => d.User).WithMany(p => p.AuditLogs)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__AuditLogs__UserI__0D7A0286");
        });

        modelBuilder.Entity<ClientAddress>(entity =>
        {
            entity.HasKey(e => e.AddressId);

            entity.ToTable("ClientAddress");

            entity.Property(e => e.AddressId).ValueGeneratedNever();
            entity.Property(e => e.City)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.Country)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.PostalCode).HasMaxLength(20);
            entity.Property(e => e.State).HasMaxLength(100);
            entity.Property(e => e.Street)
                .IsRequired()
                .HasMaxLength(256);
        });

        modelBuilder.Entity<Company>(entity =>
        {
            entity.Property(e => e.CompanyId).ValueGeneratedNever();
            entity.Property(e => e.ApiCode).HasMaxLength(256);
            entity.Property(e => e.CompanyEmail).HasMaxLength(256);
            entity.Property(e => e.CompanyInitials).HasMaxLength(10);
            entity.Property(e => e.CompanyLogoFilePath).HasMaxLength(512);
            entity.Property(e => e.CompanyName).HasMaxLength(256);
            entity.Property(e => e.CompanyPhone).HasMaxLength(15);
            entity.Property(e => e.CompanyStatus).HasMaxLength(50);
            entity.Property(e => e.CompanyType).HasMaxLength(100);
            entity.Property(e => e.ContactPerson).HasMaxLength(256);
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.ExpDate).HasColumnType("datetime");
            entity.Property(e => e.Motto).HasMaxLength(512);
            entity.Property(e => e.PayAccount).HasMaxLength(256);
            entity.Property(e => e.PayAccountName).HasMaxLength(256);
            entity.Property(e => e.PayBank).HasMaxLength(256);
            entity.Property(e => e.SmsAccount).HasMaxLength(256);
            entity.Property(e => e.SubscriptionAmount).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Address).WithMany(p => p.Companies)
                .HasForeignKey(d => d.AddressId)
                .HasConstraintName("FK_Companies_CompanyAddress");
        });

        modelBuilder.Entity<CompanyAddress>(entity =>
        {
            entity.HasKey(e => e.AddressId);

            entity.ToTable("CompanyAddress");

            entity.Property(e => e.AddressId).ValueGeneratedNever();
            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.Country).HasMaxLength(100);
            entity.Property(e => e.PostalCode).HasMaxLength(20);
            entity.Property(e => e.State).HasMaxLength(100);
            entity.Property(e => e.Street).HasMaxLength(256);
        });

        modelBuilder.Entity<CompanyClient>(entity =>
        {
            entity.HasKey(e => e.ClientId);

            entity.Property(e => e.ClientId).ValueGeneratedNever();
            entity.Property(e => e.ClientName)
                .IsRequired()
                .HasMaxLength(200);
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.DateOfBirth).HasColumnType("datetime");
            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(256);
            entity.Property(e => e.EmergencyContactName).HasMaxLength(256);
            entity.Property(e => e.EmergencyContactPhone).HasMaxLength(15);
            entity.Property(e => e.Gender)
                .IsRequired()
                .HasMaxLength(10);
            entity.Property(e => e.MaritalStatus)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.Nationality)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.PhoneNumber)
                .IsRequired()
                .HasMaxLength(15);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.Address).WithMany(p => p.CompanyClients)
                .HasForeignKey(d => d.AddressId)
                .HasConstraintName("FK_Client_Address");

            entity.HasOne(d => d.Company).WithMany(p => p.CompanyClients)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Company_Client");
        });

        modelBuilder.Entity<DataMigration>(entity =>
        {
            entity.HasKey(e => e.MigrationId).HasName("PK__DataMigr__E5D3573B40556A31");

            entity.ToTable("DataMigration");

            entity.Property(e => e.MigrationId).ValueGeneratedNever();
            entity.Property(e => e.DestinationSystem)
                .IsRequired()
                .HasMaxLength(256);
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.SourceSystem)
                .IsRequired()
                .HasMaxLength(256);
            entity.Property(e => e.StartDate).HasColumnType("datetime");
            entity.Property(e => e.Status)
                .IsRequired()
                .HasMaxLength(50);

            entity.HasOne(d => d.Company).WithMany(p => p.DataMigrations)
                .HasForeignKey(d => d.CompanyId)
                .HasConstraintName("FK_DataMigration_Company");
        });

        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.HasKey(e => e.FeedbackId).HasName("PK__Feedback__6A4BEDD6D745BCAA");

            entity.ToTable("Feedback");

            entity.Property(e => e.FeedbackId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Category).HasMaxLength(100);
            entity.Property(e => e.CompanyId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.FeedbackText).IsRequired();
            entity.Property(e => e.SubmittedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UserId)
                .IsRequired()
                .HasMaxLength(450);

            entity.HasOne(d => d.Company).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.CompanyId)
                .HasConstraintName("FK_Feedback_Company");

            entity.HasOne(d => d.User).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Feedback__UserId__08B54D69");
        });


        modelBuilder.Entity<Allergy>(entity =>
        {
            entity.HasKey(e => e.AllergyId).HasName("PK__Allergie__A49EBE42E4BFCC36");

            entity.ToTable("Allergies", "dbo");

            entity.Property(e => e.AllergyId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.AllergyName)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Severity)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Client).WithMany(p => p.Allergies)
                .HasForeignKey(d => d.ClientId)
                .HasConstraintName("FK__Allergies__Clien__55BFB948");
        });

        modelBuilder.Entity<CompanyClient>(entity =>
        {
            entity.HasKey(e => e.ClientId);

            entity.ToTable("CompanyClients", "dbo");

            entity.Property(e => e.ClientId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.ClientName)
                .IsRequired()
                .HasMaxLength(200);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DateOfBirth).HasColumnType("datetime");
            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(256);
            entity.Property(e => e.EmergencyContactName).HasMaxLength(256);
            entity.Property(e => e.EmergencyContactPhone).HasMaxLength(15);
            entity.Property(e => e.Gender)
                .IsRequired()
                .HasMaxLength(10);
            entity.Property(e => e.MaritalStatus)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.Nationality)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.PhoneNumber)
                .IsRequired()
                .HasMaxLength(15);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<DietPlan>(entity =>
        {
            entity.HasKey(e => e.DietPlanId).HasName("PK__DietPlan__D256E10A3F13B2C9");

            entity.ToTable("DietPlan", "dbo");

            entity.Property(e => e.DietPlanId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedBy)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.DietPlanName)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.EndDate).HasColumnType("date");
            entity.Property(e => e.LastUpdated).HasColumnType("datetime");
            entity.Property(e => e.StartDate).HasColumnType("date");

            entity.HasOne(d => d.Allergy).WithMany(p => p.DietPlans)
                .HasForeignKey(d => d.AllergyId)
                .HasConstraintName("FK__DietPlan__Allerg__7167D3BD");

            entity.HasOne(d => d.Client).WithMany(p => p.DietPlans)
                .HasForeignKey(d => d.ClientId)
                .HasConstraintName("FK__DietPlan__Client__7073AF84");
        });

        modelBuilder.Entity<FoodDatabase>(entity =>
        {
            entity.HasKey(e => e.FoodItemId).HasName("PK__FoodData__464DC8124C775F72");

            entity.ToTable("FoodDatabase", "dbo");

            entity.Property(e => e.FoodItemId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Category)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.FoodName)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.Client).WithMany(p => p.FoodDatabases)
                .HasForeignKey(d => d.ClientId)
                .HasConstraintName("FK__FoodDatab__Carbs__0A338187");
        });

        modelBuilder.Entity<MealLogging>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("PK__MealLogg__5E548648CA2986E7");

            entity.ToTable("MealLogging", "dbo");

            entity.Property(e => e.LogId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.LogDate).HasColumnType("datetime");

            entity.HasOne(d => d.Client).WithMany(p => p.MealLoggings)
                .HasForeignKey(d => d.ClientId)
                .HasConstraintName("FK__MealLoggi__Clien__15A53433");

            entity.HasOne(d => d.FoodItem).WithMany(p => p.MealLoggings)
                .HasForeignKey(d => d.FoodItemId)
                .HasConstraintName("FK__MealLoggi__FoodI__178D7CA5");
        });

        modelBuilder.Entity<MealPlan>(entity =>
        {
            entity.HasKey(e => e.MealPlanId).HasName("PK__MealPlan__0620DB7677F15438");

            entity.ToTable("MealPlans", "dbo");

            entity.Property(e => e.MealPlanId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.FoodCategory)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MealName)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.DietPlan).WithMany(p => p.MealPlans)
                .HasForeignKey(d => d.DietPlanId)
                .HasConstraintName("FK__MealPlans__DietP__11D4A34F");
        });

        modelBuilder.Entity<MedicalCondition>(entity =>
        {
            entity.HasKey(e => e.ConditionId).HasName("PK__MedicalC__37F5C0CF98AE7599");

            entity.ToTable("MedicalConditions", "dbo");

            entity.Property(e => e.ConditionId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.ConditionName)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.DiagnosisDate).HasColumnType("date");

            entity.HasOne(d => d.Client).WithMany(p => p.MedicalConditions)
                .HasForeignKey(d => d.ClientId)
                .HasConstraintName("FK__MedicalCo__Clien__59904A2C");
        });

        modelBuilder.Entity<NutritionalProfile>(entity =>
        {
            entity.HasKey(e => e.ProfileId).HasName("PK__Nutritio__290C88E46E0FE18E");

            entity.ToTable("NutritionalProfile", "dbo");

            entity.Property(e => e.ProfileId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Height)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("height");
            entity.Property(e => e.Weight)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("weight");

            entity.HasOne(d => d.Allergy).WithMany(p => p.NutritionalProfiles)
                .HasForeignKey(d => d.AllergyId)
                .HasConstraintName("FK__Nutrition__Aller__047AA831");

            entity.HasOne(d => d.Client).WithMany(p => p.NutritionalProfiles)
                .HasForeignKey(d => d.ClientId)
                .HasConstraintName("FK__Nutrition__Clien__038683F8");

            entity.HasOne(d => d.DietPlan).WithMany(p => p.NutritionalProfiles)
                .HasForeignKey(d => d.DietPlanId)
                .HasConstraintName("FK__Nutrition__DietP__0662F0A3");
        });

        modelBuilder.Entity<Report>(entity =>
        {
            entity.HasKey(e => e.ReportId).HasName("PK__Report__D5BD48054B37C2EE");

            entity.ToTable("Report", "dbo");

            entity.Property(e => e.ReportId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.ReportContent).HasColumnType("text");
            entity.Property(e => e.ReportDate).HasColumnType("datetime");
            entity.Property(e => e.ReportType)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.Client).WithMany(p => p.Reports)
                .HasForeignKey(d => d.ClientId)
                .HasConstraintName("FK__Report__ClientId__6CA31EA0");
        });

        modelBuilder.Entity<BillingCycleLookup>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__BillingC__3214EC07271F0E00");

            entity.ToTable("BillingCycleLookup", "dbo");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CycleName)
                .IsRequired()
                .HasMaxLength(50);
        });


        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Payment__3214EC072C033188");

            entity.ToTable("Payment", "dbo");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.PaymentDate).HasColumnType("datetime");
            entity.Property(e => e.TransactionId).HasMaxLength(255);

            entity.HasOne(d => d.PaymentMethod).WithMany(p => p.Payments)
                .HasForeignKey(d => d.PaymentMethodId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Payment__Payment__60FC61CA");

            entity.HasOne(d => d.PaymentStatus).WithMany(p => p.Payments)
                .HasForeignKey(d => d.PaymentStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Payment__Payment__60083D91");

            entity.HasOne(d => d.Subscription).WithMany(p => p.Payments)
                .HasForeignKey(d => d.SubscriptionId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__Payment__Subscri__5F141958");
        });

        modelBuilder.Entity<PaymentMethodLookup>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PaymentM__3214EC073CC5077C");

            entity.ToTable("PaymentMethodLookup", "dbo");

            entity.Property(e => e.Method)
                .IsRequired()
                .HasMaxLength(50);
        });

        modelBuilder.Entity<PaymentStatusLookup>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PaymentS__3214EC07340D8E27");

            entity.ToTable("PaymentStatusLookup", "dbo");

            entity.Property(e => e.Status)
                .IsRequired()
                .HasMaxLength(50);
        });

        modelBuilder.Entity<Subscription>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Subscrip__3214EC07EB97F597");

            entity.ToTable("Subscription", "dbo");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.PaymentStatus)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.StartDate).HasColumnType("datetime");

            entity.HasOne(d => d.Company).WithMany(p => p.Subscriptions)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Company_Subscription");

            entity.HasOne(d => d.SubscriptionPlan).WithMany(p => p.Subscriptions)
                .HasForeignKey(d => d.SubscriptionPlanId)
                .HasConstraintName("FK_SubscriptionPlan_Subscription");
        });

        modelBuilder.Entity<SubscriptionActivityLookup>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Subscrip__3214EC070751D0BB");

            entity.ToTable("SubscriptionActivityLookup", "dbo");

            entity.Property(e => e.ActivityName)
                .IsRequired()
                .HasMaxLength(255);
        });

        modelBuilder.Entity<SubscriptionLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Subscrip__3214EC070EA444F0");

            entity.ToTable("SubscriptionLog", "dbo");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.LogDate).HasColumnType("datetime");

            entity.HasOne(d => d.Activity).WithMany(p => p.SubscriptionLogs)
                .HasForeignKey(d => d.ActivityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Subscript__Activ__0539C240");

            entity.HasOne(d => d.Subscription).WithMany(p => p.SubscriptionLogs)
                .HasForeignKey(d => d.SubscriptionId)
                .HasConstraintName("FK__Subscript__Subsc__04459E07");
        });

        modelBuilder.Entity<SubscriptionPlan>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Subscrip__3214EC077710149E");

            entity.ToTable("SubscriptionPlan", "dbo");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Description).HasMaxLength(500);

            entity.HasOne(d => d.BillingCycle).WithMany(p => p.SubscriptionPlans)
                .HasForeignKey(d => d.BillingCycleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BillingCycle_SubscriptionPlan");

            entity.HasOne(d => d.PlanName).WithMany(p => p.SubscriptionPlans)
                .HasForeignKey(d => d.PlanNameId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PlanName_SubscriptionPlan");
        });

        modelBuilder.Entity<SubscriptionPlanNameLookup>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Subscrip__3214EC0726F9188B");

            entity.ToTable("SubscriptionPlanNameLookup", "dbo");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.PlanName)
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<TrialNotification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TrialNot__3214EC07BD034B76");

            entity.ToTable("TrialNotification", "dbo");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.ReminderDate).HasColumnType("datetime");
            entity.Property(e => e.SentAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TrialEndDate).HasColumnType("datetime");
            entity.Property(e => e.TrialStartDate).HasColumnType("datetime");

            entity.HasOne(d => d.Company).WithMany(p => p.TrialNotifications)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Company_TrialNotification");

            entity.HasOne(d => d.NotificationType).WithMany(p => p.TrialNotifications)
                .HasForeignKey(d => d.NotificationTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_NotificationType_TrialNotification");
        });

        modelBuilder.Entity<TrialNotificationLookup>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TrialNot__3214EC0794269932");

            entity.ToTable("TrialNotificationLookup", "dbo");

            entity.Property(e => e.Message)
                .IsRequired()
                .HasColumnType("text");
            entity.Property(e => e.Type)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
