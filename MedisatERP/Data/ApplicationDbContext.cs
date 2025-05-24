using System;
using System.Collections.Generic;
using MedisatERP.Models;
using Microsoft.EntityFrameworkCore;

namespace MedisatERP.Data;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Allergy> Allergies { get; set; }

    public virtual DbSet<Appointment> Appointments { get; set; }

    public virtual DbSet<AspNetRole> AspNetRoles { get; set; }

    public virtual DbSet<AspNetRoleClaim> AspNetRoleClaims { get; set; }

    public virtual DbSet<AspNetUser> AspNetUsers { get; set; }

    public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }

    public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }

    public virtual DbSet<AspNetUserToken> AspNetUserTokens { get; set; }

    public virtual DbSet<AuditLog> AuditLogs { get; set; }

    public virtual DbSet<BillingCycleLookup> BillingCycleLookups { get; set; }

    public virtual DbSet<Blog> Blogs { get; set; }

    public virtual DbSet<BlogCategory> BlogCategories { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<ClientMembership> ClientMemberships { get; set; }

    public virtual DbSet<Company> Companies { get; set; }

    public virtual DbSet<CompanyClient> CompanyClients { get; set; }

    public virtual DbSet<ContentCategory> ContentCategories { get; set; }

    public virtual DbSet<ContentManagement> ContentManagements { get; set; }

    public virtual DbSet<DataMigration> DataMigrations { get; set; }

    public virtual DbSet<DietPlan> DietPlans { get; set; }

    public virtual DbSet<ExercisePlan> ExercisePlans { get; set; }

    public virtual DbSet<ExpenseTracking> ExpenseTrackings { get; set; }

    public virtual DbSet<Feedback> Feedbacks { get; set; }

    public virtual DbSet<FollowUp> FollowUps { get; set; }

    public virtual DbSet<FoodDatabase> FoodDatabases { get; set; }

    public virtual DbSet<Goal> Goals { get; set; }

    public virtual DbSet<HealthMetric> HealthMetrics { get; set; }

    public virtual DbSet<Hospital> Hospitals { get; set; }

    public virtual DbSet<HospitalReferral> HospitalReferrals { get; set; }

    public virtual DbSet<HospitalSchedule> HospitalSchedules { get; set; }

    public virtual DbSet<Image> Images { get; set; }

    public virtual DbSet<MealLogging> MealLoggings { get; set; }

    public virtual DbSet<MealPlan> MealPlans { get; set; }

    public virtual DbSet<MealPreference> MealPreferences { get; set; }

    public virtual DbSet<MedicalCondition> MedicalConditions { get; set; }

    public virtual DbSet<MembershipLevel> MembershipLevels { get; set; }

    public virtual DbSet<NutritionCompanySubscription> NutritionCompanySubscriptions { get; set; }

    public virtual DbSet<NutritionalProfile> NutritionalProfiles { get; set; }

    public virtual DbSet<OnlineApplicant> OnlineApplicants { get; set; }

    public virtual DbSet<PartnershipAgreement> PartnershipAgreements { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProgressTracking> ProgressTrackings { get; set; }

    public virtual DbSet<Recipe> Recipes { get; set; }

    public virtual DbSet<Report> Reports { get; set; }

    public virtual DbSet<Subscription> Subscriptions { get; set; }

    public virtual DbSet<Subscription1> Subscriptions1 { get; set; }

    public virtual DbSet<SubscriptionLog> SubscriptionLogs { get; set; }

    public virtual DbSet<SubscriptionPayment> SubscriptionPayments { get; set; }

    public virtual DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }

    public virtual DbSet<SubscriptionPlanNameLookup> SubscriptionPlanNameLookups { get; set; }

    public virtual DbSet<Supplement> Supplements { get; set; }

    public virtual DbSet<TrialNotification> TrialNotifications { get; set; }

    public virtual DbSet<TrialNotificationLookup> TrialNotificationLookups { get; set; }

    public virtual DbSet<WorkplaceLookup> WorkplaceLookups { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=MedisatConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("adminMedisatERP");

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

        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(e => e.AppointmentId).HasName("PK__Appointm__8ECDFCC28DFA0A5B");

            entity.ToTable("Appointments", "dbo");

            entity.Property(e => e.AppointmentId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Notes).HasMaxLength(500);
            entity.Property(e => e.NutritionistId)
                .IsRequired()
                .HasMaxLength(450);
            entity.Property(e => e.Priority).HasMaxLength(10);
            entity.Property(e => e.ReminderSentAt).HasColumnType("datetime");
            entity.Property(e => e.ScheduledDate).HasColumnType("datetime");
            entity.Property(e => e.Status)
                .IsRequired()
                .HasMaxLength(20);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.Client).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.ClientId)
                .HasConstraintName("FK_Appointments_Client");

            entity.HasOne(d => d.Company).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.CompanyId)
                .HasConstraintName("FK_Appointments_Company");

            entity.HasOne(d => d.Nutritionist).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.NutritionistId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Appointments_Nutritionist");

            entity.HasOne(d => d.Workplace).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.WorkplaceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Appointments_Workplace");
        });

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

        modelBuilder.Entity<AspNetUser>(entity =>
        {
            entity.HasIndex(e => e.NormalizedEmail, "EmailIndex");

            entity.HasIndex(e => e.NormalizedUserName, "UserNameIndex")
                .IsUnique()
                .HasFilter("([NormalizedUserName] IS NOT NULL)");

            entity.Property(e => e.Country).HasMaxLength(100);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Dob)
                .HasColumnType("date")
                .HasColumnName("DOB");
            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.Gender).HasMaxLength(10);
            entity.Property(e => e.Name).HasMaxLength(256);
            entity.Property(e => e.NormalizedEmail).HasMaxLength(256);
            entity.Property(e => e.NormalizedUserName).HasMaxLength(256);
            entity.Property(e => e.ProfileImagePath).HasMaxLength(255);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
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
            entity.HasKey(e => e.Id).HasName("PK__AuditLog__EB5F6CBD52DD8874");

            entity.ToTable("AuditLogs", "dbo");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
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
            entity.Property(e => e.UserId).HasMaxLength(450);

            entity.HasOne(d => d.Company).WithMany(p => p.AuditLogs)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_AuditLogs_Company");

            entity.HasOne(d => d.User).WithMany(p => p.AuditLogs)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_AuditLogs_User");
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

        modelBuilder.Entity<Blog>(entity =>
        {
            entity.HasKey(e => e.BlogId).HasName("PK__Blogs__54379E307C912ED2");

            entity.ToTable("Blogs", "dbo");

            entity.Property(e => e.BlogId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Content).IsRequired();
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PublishedDate).HasColumnType("datetime");
            entity.Property(e => e.Status)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.Tags).HasMaxLength(255);
            entity.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<BlogCategory>(entity =>
        {
            entity.HasKey(e => e.BlogCategoryId).HasName("PK__BlogCate__6BD2DA019F5FD70A");

            entity.ToTable("BlogCategories", "dbo");

            entity.Property(e => e.BlogCategoryId).HasDefaultValueSql("(newid())");

            entity.HasOne(d => d.Blog).WithMany(p => p.BlogCategories)
                .HasForeignKey(d => d.BlogId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BlogCateg__BlogI__04CFADEC");

            entity.HasOne(d => d.Category).WithMany(p => p.BlogCategories)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BlogCateg__Categ__05C3D225");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Categori__19093A0B527121DA");

            entity.ToTable("Categories", "dbo");

            entity.Property(e => e.CategoryId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CategoryName)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<ClientMembership>(entity =>
        {
            entity.HasKey(e => e.ClientMembershipId).HasName("PK__ClientMe__D13287670A7BEE58");

            entity.ToTable("ClientMemberships", "dbo");

            entity.Property(e => e.ClientMembershipId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.StartDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.Client).WithMany(p => p.ClientMemberships)
                .HasForeignKey(d => d.ClientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ClientMem__Clien__09C96D33");

            entity.HasOne(d => d.MembershipLevel).WithMany(p => p.ClientMemberships)
                .HasForeignKey(d => d.MembershipLevelId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ClientMem__Membe__0ABD916C");
        });

        modelBuilder.Entity<Company>(entity =>
        {
            entity.ToTable("Companies", "dbo");

            entity.Property(e => e.CompanyId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.ApiCode).HasMaxLength(256);
            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.CompanyEmail).HasMaxLength(256);
            entity.Property(e => e.CompanyInitials).HasMaxLength(10);
            entity.Property(e => e.CompanyLogoFilePath).HasMaxLength(511);
            entity.Property(e => e.CompanyName).HasMaxLength(256);
            entity.Property(e => e.CompanyPhone).HasMaxLength(15);
            entity.Property(e => e.CompanyType).HasMaxLength(100);
            entity.Property(e => e.CompanyWebsite).HasMaxLength(256);
            entity.Property(e => e.ContactPerson).HasMaxLength(256);
            entity.Property(e => e.ContactPersonPhone).HasMaxLength(15);
            entity.Property(e => e.Country).HasMaxLength(100);
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.Motto).HasMaxLength(512);
            entity.Property(e => e.PostalCode).HasMaxLength(20);
            entity.Property(e => e.State).HasMaxLength(100);
            entity.Property(e => e.Status)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValueSql("('InActive')");
            entity.Property(e => e.Street).HasMaxLength(256);
        });

        modelBuilder.Entity<CompanyClient>(entity =>
        {
            entity.ToTable("CompanyClients", "dbo");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.ClientName)
                .IsRequired()
                .HasMaxLength(200);
            entity.Property(e => e.Country).HasMaxLength(100);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DateOfBirth).HasColumnType("datetime");
            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(256);
            entity.Property(e => e.EmergencyContactName).HasMaxLength(256);
            entity.Property(e => e.EmergencyContactPhone).HasMaxLength(15);
            entity.Property(e => e.EmergencyContactRelationship).HasMaxLength(100);
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
            entity.Property(e => e.PostalCode).HasMaxLength(20);
            entity.Property(e => e.State).HasMaxLength(100);
            entity.Property(e => e.Street).HasMaxLength(256);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<ContentCategory>(entity =>
        {
            entity.HasKey(e => e.ContentCategoryId).HasName("PK__ContentC__7742238C0BF9621F");

            entity.ToTable("ContentCategories", "dbo");

            entity.Property(e => e.ContentCategoryId).HasDefaultValueSql("(newid())");

            entity.HasOne(d => d.Category).WithMany(p => p.ContentCategories)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ContentCa__Categ__00FF1D08");

            entity.HasOne(d => d.Content).WithMany(p => p.ContentCategories)
                .HasForeignKey(d => d.ContentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ContentCa__Conte__000AF8CF");
        });

        modelBuilder.Entity<ContentManagement>(entity =>
        {
            entity.HasKey(e => e.ContentId).HasName("PK__ContentM__2907A81E8AF43054");

            entity.ToTable("ContentManagement", "dbo");

            entity.Property(e => e.ContentId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.ContentTitle)
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(e => e.ContentType)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PublishedDate).HasColumnType("datetime");
            entity.Property(e => e.Status)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.Tags).HasMaxLength(255);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.Url).HasMaxLength(255);
        });

        modelBuilder.Entity<DataMigration>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__DataMigr__E5D3573B63B85DFE");

            entity.ToTable("DataMigration", "dbo");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CompanyId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.DestinationSystem)
                .IsRequired()
                .HasMaxLength(256);
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.ErrorCount).HasDefaultValueSql("((0))");
            entity.Property(e => e.RecordsMigrated).HasDefaultValueSql("((0))");
            entity.Property(e => e.SourceSystem)
                .IsRequired()
                .HasMaxLength(256);
            entity.Property(e => e.StartDate).HasColumnType("datetime");
            entity.Property(e => e.Status)
                .IsRequired()
                .HasMaxLength(50);

            entity.HasOne(d => d.Company).WithMany(p => p.DataMigrations)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_DataMigration_Company");
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
            entity.Property(e => e.DietType)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.EndDate).HasColumnType("date");
            entity.Property(e => e.Goal)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.LastUpdated).HasColumnType("datetime");
            entity.Property(e => e.StartDate).HasColumnType("date");

            entity.HasOne(d => d.Allergy).WithMany(p => p.DietPlans)
                .HasForeignKey(d => d.AllergyId)
                .HasConstraintName("FK__DietPlan__Allerg__7167D3BD");

            entity.HasOne(d => d.Client).WithMany(p => p.DietPlans)
                .HasForeignKey(d => d.ClientId)
                .HasConstraintName("FK__DietPlan__Client__7073AF84");

            entity.HasOne(d => d.Condition).WithMany(p => p.DietPlans)
                .HasForeignKey(d => d.ConditionId)
                .HasConstraintName("FK__DietPlan__Condit__725BF7F6");
        });

        modelBuilder.Entity<ExercisePlan>(entity =>
        {
            entity.HasKey(e => e.ExercisePlanId).HasName("PK__Exercise__8A3C5C19A698591E");

            entity.ToTable("ExercisePlans", "dbo");

            entity.Property(e => e.ExercisePlanId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedDate).HasColumnType("date");
            entity.Property(e => e.ExerciseName)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Frequency)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Client).WithMany(p => p.ExercisePlans)
                .HasForeignKey(d => d.ClientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ExerciseP__Clien__451F3D2B");
        });

        modelBuilder.Entity<ExpenseTracking>(entity =>
        {
            entity.HasKey(e => e.ExpenseId).HasName("PK__ExpenseT__1445CFD31448C52A");

            entity.ToTable("ExpenseTracking", "dbo");

            entity.Property(e => e.ExpenseId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Category)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.ExpenseDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Feedback__6A4BEDD6D92678A5");

            entity.ToTable("Feedback", "dbo");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Category).HasMaxLength(100);
            entity.Property(e => e.CompanyId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.FeedbackText).IsRequired();
            entity.Property(e => e.SubmittedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UserId).HasMaxLength(450);

            entity.HasOne(d => d.Company).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Feedback_Company");

            entity.HasOne(d => d.User).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Feedback_User");
        });

        modelBuilder.Entity<FollowUp>(entity =>
        {
            entity.HasKey(e => e.FollowUpId).HasName("PK__FollowUp__D507D6383BF365DD");

            entity.ToTable("FollowUps", "dbo");

            entity.Property(e => e.FollowUpId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CurrentStatus)
                .IsRequired()
                .HasMaxLength(20);
            entity.Property(e => e.FollowUpDate).HasColumnType("datetime");
            entity.Property(e => e.FollowUpOutcome).HasMaxLength(100);
            entity.Property(e => e.FollowUpType)
                .IsRequired()
                .HasMaxLength(20);
            entity.Property(e => e.Notes).HasMaxLength(500);
            entity.Property(e => e.ReminderSentAt).HasColumnType("datetime");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.Client).WithMany(p => p.FollowUpClients)
                .HasForeignKey(d => d.ClientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FollowUps_Client");

            entity.HasOne(d => d.Practitioner).WithMany(p => p.FollowUpPractitioners)
                .HasForeignKey(d => d.PractitionerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FollowUps_Practitioner");
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

        modelBuilder.Entity<Goal>(entity =>
        {
            entity.HasKey(e => e.GoalId).HasName("PK__Goals__8A4FFFD16035092B");

            entity.ToTable("Goals", "dbo");

            entity.Property(e => e.GoalId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.GoalDescription)
                .IsRequired()
                .HasMaxLength(500);
            entity.Property(e => e.TargetDate).HasColumnType("date");

            entity.HasOne(d => d.Client).WithMany(p => p.Goals)
                .HasForeignKey(d => d.ClientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Goals__ClientId__4DB4832C");
        });

        modelBuilder.Entity<HealthMetric>(entity =>
        {
            entity.HasKey(e => e.MetricId).HasName("PK__HealthMe__561056A5AA11C57C");

            entity.ToTable("HealthMetrics", "dbo");

            entity.Property(e => e.MetricId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.BloodPressure)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.MeasurementDate).HasColumnType("date");
            entity.Property(e => e.Notes).HasMaxLength(500);

            entity.HasOne(d => d.Client).WithMany(p => p.HealthMetrics)
                .HasForeignKey(d => d.ClientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__HealthMet__Clien__51851410");
        });

        modelBuilder.Entity<Hospital>(entity =>
        {
            entity.HasKey(e => e.HospitalId).HasName("PK__Hospital__38C2E5AF931DD991");

            entity.ToTable("Hospitals", "dbo");

            entity.Property(e => e.HospitalId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.AccreditationStatus).HasMaxLength(50);
            entity.Property(e => e.Address)
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(e => e.City)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.ContactEmail).HasMaxLength(255);
            entity.Property(e => e.ContactName)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.ContactNotes).HasMaxLength(500);
            entity.Property(e => e.ContactPhoneNumber)
                .IsRequired()
                .HasMaxLength(15);
            entity.Property(e => e.ContactPosition).HasMaxLength(100);
            entity.Property(e => e.Country)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.HospitalType).HasMaxLength(100);
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.PhoneNumber)
                .IsRequired()
                .HasMaxLength(15);
            entity.Property(e => e.PostalCode).HasMaxLength(20);
            entity.Property(e => e.Specialties).HasMaxLength(255);
            entity.Property(e => e.State).HasMaxLength(100);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.Website).HasMaxLength(255);
        });

        modelBuilder.Entity<HospitalReferral>(entity =>
        {
            entity.HasKey(e => e.ReferralId).HasName("PK__Hospital__A2C4A96631FD01FF");

            entity.ToTable("HospitalReferrals", "dbo");

            entity.Property(e => e.ReferralId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FollowUpDate).HasColumnType("datetime");
            entity.Property(e => e.Notes).HasMaxLength(500);
            entity.Property(e => e.NutritionistId)
                .IsRequired()
                .HasMaxLength(450);
            entity.Property(e => e.Outcome).HasMaxLength(255);
            entity.Property(e => e.Reason)
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(e => e.ReferralDate).HasColumnType("datetime");
            entity.Property(e => e.ReferralStatus).HasMaxLength(50);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.Client).WithMany(p => p.HospitalReferrals)
                .HasForeignKey(d => d.ClientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__HospitalR__Clien__75C27486");

            entity.HasOne(d => d.Hospital).WithMany(p => p.HospitalReferrals)
                .HasForeignKey(d => d.HospitalId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__HospitalR__Hospi__73DA2C14");

            entity.HasOne(d => d.Nutritionist).WithMany(p => p.HospitalReferrals)
                .HasForeignKey(d => d.NutritionistId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__HospitalR__Nutri__74CE504D");
        });

        modelBuilder.Entity<HospitalSchedule>(entity =>
        {
            entity.HasKey(e => e.ScheduleId).HasName("PK__Hospital__9C8A5B49C8D54C13");

            entity.ToTable("HospitalSchedules", "dbo");

            entity.Property(e => e.ScheduleId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DayOfWeek)
                .IsRequired()
                .HasMaxLength(20);
            entity.Property(e => e.Notes).HasMaxLength(500);
            entity.Property(e => e.NutritionistId)
                .IsRequired()
                .HasMaxLength(450);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.Hospital).WithMany(p => p.HospitalSchedules)
                .HasForeignKey(d => d.HospitalId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__HospitalS__Hospi__68687968");

            entity.HasOne(d => d.Nutritionist).WithMany(p => p.HospitalSchedules)
                .HasForeignKey(d => d.NutritionistId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__HospitalS__Nutri__695C9DA1");
        });

        modelBuilder.Entity<Image>(entity =>
        {
            entity.HasKey(e => e.ImageId).HasName("PK__Images__7516F70CAB53348C");

            entity.ToTable("Images", "dbo");

            entity.Property(e => e.ImageId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.AltText).HasMaxLength(255);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.ImageUrl)
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(e => e.Tags).HasMaxLength(255);
            entity.Property(e => e.Title).HasMaxLength(100);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.UploadDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<MealLogging>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("PK__MealLogg__5E548648CA2986E7");

            entity.ToTable("MealLogging", "dbo");

            entity.Property(e => e.LogId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.LogDate).HasColumnType("datetime");
            entity.Property(e => e.MealTime)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Client).WithMany(p => p.MealLoggings)
                .HasForeignKey(d => d.ClientId)
                .HasConstraintName("FK__MealLoggi__Clien__15A53433");

            entity.HasOne(d => d.FoodItem).WithMany(p => p.MealLoggings)
                .HasForeignKey(d => d.FoodItemId)
                .HasConstraintName("FK__MealLoggi__FoodI__178D7CA5");

            entity.HasOne(d => d.MealPlan).WithMany(p => p.MealLoggings)
                .HasForeignKey(d => d.MealPlanId)
                .HasConstraintName("FK__MealLoggi__MealP__1699586C");
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

        modelBuilder.Entity<MealPreference>(entity =>
        {
            entity.HasKey(e => e.PreferenceId).HasName("PK__MealPref__E228496FF57B116D");

            entity.ToTable("MealPreferences", "dbo");

            entity.Property(e => e.PreferenceId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Notes).HasMaxLength(500);
            entity.Property(e => e.Preference)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.Restriction).HasMaxLength(100);

            entity.HasOne(d => d.Client).WithMany(p => p.MealPreferences)
                .HasForeignKey(d => d.ClientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MealPrefe__Clien__5832119F");
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
            entity.Property(e => e.Severity)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Client).WithMany(p => p.MedicalConditions)
                .HasForeignKey(d => d.ClientId)
                .HasConstraintName("FK__MedicalCo__Clien__59904A2C");
        });

        modelBuilder.Entity<MembershipLevel>(entity =>
        {
            entity.HasKey(e => e.MembershipLevelId).HasName("PK__Membersh__347FCC89DC5E6C72");

            entity.ToTable("MembershipLevels", "dbo");

            entity.Property(e => e.MembershipLevelId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.LevelName)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<NutritionCompanySubscription>(entity =>
        {
            entity.HasKey(e => e.SubscriptionId).HasName("PK__Nutritio__9A2B249D88040296");

            entity.ToTable("NutritionCompanySubscriptions", "dbo");

            entity.Property(e => e.SubscriptionId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.RenewalDate).HasColumnType("datetime");
            entity.Property(e => e.SubscriptionEndDate).HasColumnType("datetime");
            entity.Property(e => e.SubscriptionStartDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.Client).WithMany(p => p.NutritionCompanySubscriptions)
                .HasForeignKey(d => d.ClientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Nutrition__Clien__23893F36");

            entity.HasOne(d => d.Product).WithMany(p => p.NutritionCompanySubscriptions)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Nutrition__Produ__247D636F");
        });

        modelBuilder.Entity<NutritionalProfile>(entity =>
        {
            entity.HasKey(e => e.ProfileId).HasName("PK__Nutritio__290C88E46E0FE18E");

            entity.ToTable("NutritionalProfile", "dbo");

            entity.Property(e => e.ProfileId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.ActivityLevel)
                .HasMaxLength(50)
                .IsUnicode(false);
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

            entity.HasOne(d => d.Condition).WithMany(p => p.NutritionalProfiles)
                .HasForeignKey(d => d.ConditionId)
                .HasConstraintName("FK__Nutrition__Condi__056ECC6A");

            entity.HasOne(d => d.DietPlan).WithMany(p => p.NutritionalProfiles)
                .HasForeignKey(d => d.DietPlanId)
                .HasConstraintName("FK__Nutrition__DietP__0662F0A3");
        });

        modelBuilder.Entity<OnlineApplicant>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__OnlineAp__3214EC0764F7774D");

            entity.ToTable("OnlineApplicants", "dbo");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.FirstName)
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(e => e.HowDidYouHearAboutUs).HasMaxLength(255);
            entity.Property(e => e.LastName)
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(e => e.MobilePhoneNo).HasMaxLength(20);
            entity.Property(e => e.PreferredSchedule).HasColumnType("date");
            entity.Property(e => e.Reason).HasMaxLength(500);
        });

        modelBuilder.Entity<PartnershipAgreement>(entity =>
        {
            entity.HasKey(e => e.AgreementId).HasName("PK__Partners__0A3082C358D637AD");

            entity.ToTable("PartnershipAgreements", "dbo");

            entity.Property(e => e.AgreementId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.AgreementStatus).HasMaxLength(50);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.EndDate).HasColumnType("date");
            entity.Property(e => e.NutritionistId)
                .IsRequired()
                .HasMaxLength(450);
            entity.Property(e => e.RenewalDate).HasColumnType("date");
            entity.Property(e => e.StartDate).HasColumnType("date");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.Hospital).WithMany(p => p.PartnershipAgreements)
                .HasForeignKey(d => d.HospitalId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Partnersh__Hospi__6E2152BE");

            entity.HasOne(d => d.Nutritionist).WithMany(p => p.PartnershipAgreements)
                .HasForeignKey(d => d.NutritionistId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Partnersh__Nutri__6F1576F7");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK__Products__B40CC6CD3F42B71F");

            entity.ToTable("Products", "dbo");

            entity.Property(e => e.ProductId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Category).HasMaxLength(50);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ProductName)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<ProgressTracking>(entity =>
        {
            entity.HasKey(e => e.ProgressId).HasName("PK__Progress__BAE29CA5A5EE9C73");

            entity.ToTable("ProgressTracking", "dbo");

            entity.Property(e => e.ProgressId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Bmi)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("BMI");
            entity.Property(e => e.BodyFatPercentage).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.DateRecorded).HasColumnType("date");
            entity.Property(e => e.Notes).HasMaxLength(500);
            entity.Property(e => e.Weight).HasColumnType("decimal(5, 2)");

            entity.HasOne(d => d.Client).WithMany(p => p.ProgressTrackings)
                .HasForeignKey(d => d.ClientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ProgressT__Clien__414EAC47");
        });

        modelBuilder.Entity<Recipe>(entity =>
        {
            entity.HasKey(e => e.RecipeId).HasName("PK__Recipes__FDD988B014557C6A");

            entity.ToTable("Recipes", "dbo");

            entity.Property(e => e.RecipeId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Ingredients).IsRequired();
            entity.Property(e => e.Instructions).IsRequired();
            entity.Property(e => e.RecipeName)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false);
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
                .HasConstraintName("FK_Company_Subscription");

            entity.HasOne(d => d.SubscriptionPlan).WithMany(p => p.Subscriptions)
                .HasForeignKey(d => d.SubscriptionPlanId)
                .HasConstraintName("FK_SubscriptionPlan_Subscription");
        });

        modelBuilder.Entity<Subscription1>(entity =>
        {
            entity.HasKey(e => e.SubscriptionId).HasName("PK__Subscrip__9A2B249D3A2BC160");

            entity.ToTable("Subscriptions", "dbo");

            entity.Property(e => e.SubscriptionId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.RenewalDate).HasColumnType("datetime");
            entity.Property(e => e.SubscriptionEndDate).HasColumnType("datetime");
            entity.Property(e => e.SubscriptionStartDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.Client).WithMany(p => p.Subscription1s)
                .HasForeignKey(d => d.ClientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Subscript__Clien__7F4BDEC0");

            entity.HasOne(d => d.Product).WithMany(p => p.Subscription1s)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Subscript__Produ__004002F9");
        });

        modelBuilder.Entity<SubscriptionLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Subscrip__3214EC070EA444F0");

            entity.ToTable("SubscriptionLog", "dbo");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Activity).HasMaxLength(100);
            entity.Property(e => e.LogDate).HasColumnType("datetime");

            entity.HasOne(d => d.Subscription).WithMany(p => p.SubscriptionLogs)
                .HasForeignKey(d => d.SubscriptionId)
                .HasConstraintName("FK__Subscript__Subsc__04459E07");
        });

        modelBuilder.Entity<SubscriptionPayment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Payment__3214EC072C033188");

            entity.ToTable("SubscriptionPayments", "dbo");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Method).HasMaxLength(50);
            entity.Property(e => e.PaymentDate).HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.TransactionId).HasMaxLength(255);

            entity.HasOne(d => d.Subscription).WithMany(p => p.SubscriptionPayments)
                .HasForeignKey(d => d.SubscriptionId)
                .HasConstraintName("FK_Subscription_SubscriptionPayments");
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

        modelBuilder.Entity<Supplement>(entity =>
        {
            entity.HasKey(e => e.SupplementId).HasName("PK__Suppleme__15E90EDE5F537E3F");

            entity.ToTable("Supplements", "dbo");

            entity.Property(e => e.SupplementId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Dosage)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.EndDate).HasColumnType("date");
            entity.Property(e => e.Frequency)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.StartDate).HasColumnType("date");
            entity.Property(e => e.SupplementName)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.Client).WithMany(p => p.Supplements)
                .HasForeignKey(d => d.ClientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Supplemen__Clien__48EFCE0F");
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

        modelBuilder.Entity<WorkplaceLookup>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Workplac__3214EC07A626F657");

            entity.ToTable("WorkplaceLookup", "dbo");

            entity.Property(e => e.Workplace)
                .IsRequired()
                .HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
