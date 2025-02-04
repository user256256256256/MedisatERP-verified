using MedisatERP.Areas.NutritionCompanySystem.Models;
using Microsoft.EntityFrameworkCore;
using MedisatERP.Data;

namespace MedisatERP.Data;

public partial class NutritionSystemDbContext : DbContext
{
    private readonly SharedDbContext _sharedDbContext;

    public NutritionSystemDbContext()
    {
    }

    public NutritionSystemDbContext(DbContextOptions<NutritionSystemDbContext> options, SharedDbContext sharedDbContext)
        : base(options)
    {
        _sharedDbContext = sharedDbContext;
    }


    public virtual DbSet<Allergy> Allergies { get; set; }

    public virtual DbSet<Appointment> Appointments { get; set; } 

    public virtual DbSet<Blog> Blogs { get; set; }

    public virtual DbSet<BlogCategory> BlogCategories { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<ClientMembership> ClientMemberships { get; set; }

    public virtual DbSet<ContentCategory> ContentCategories { get; set; }

    public virtual DbSet<ContentManagement> ContentManagements { get; set; }

    public virtual DbSet<DietPlan> DietPlans { get; set; }

    public virtual DbSet<ExercisePlan> ExercisePlans { get; set; }

    public virtual DbSet<ExpenseTracking> ExpenseTrackings { get; set; }

    public virtual DbSet<FoodDatabase> FoodDatabases { get; set; }

    public virtual DbSet<FollowUp> FollowUps { get; set; }

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

    public virtual DbSet<PartnershipAgreement> PartnershipAgreements { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProgressTracking> ProgressTrackings { get; set; }

    public virtual DbSet<Recipe> Recipes { get; set; }

    public virtual DbSet<Report> Reports { get; set; }

    public virtual DbSet<Supplement> Supplements { get; set; }

    public virtual DbSet<WorkplaceLookup> WorkplaceLookups { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=MedisatConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
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
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Appointments_Client");

            entity.HasOne(d => d.Nutritionist).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.NutritionistId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Appointments_Nutritionist");

            entity.HasOne(d => d.Workplace).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.WorkplaceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Appointments_Workplace");

            entity.HasOne(d => d.Company)
                .WithMany(p => p.Appointments)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Appointments_Company");
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

        modelBuilder.Entity<DietPlan>(entity =>
        {
            entity.HasKey(e => e.DietPlanId).HasName("PK__DietPlan__D256E10A3F13B2C9");
            entity.ToTable("DietPlan", "dbo");
            entity.Property(e => e.DietPlanId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedBy).IsRequired().HasMaxLength(100).IsUnicode(false);
            entity.Property(e => e.DietPlanName).IsRequired().HasMaxLength(100).IsUnicode(false);
            entity.Property(e => e.DietType).HasMaxLength(50).IsUnicode(false);
            entity.Property(e => e.EndDate).HasColumnType("date");
            entity.Property(e => e.Goal).HasMaxLength(50).IsUnicode(false);
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

        modelBuilder.Entity<FoodDatabase>(entity =>
        {
            entity.HasKey(e => e.FoodItemId).HasName("PK__FoodData__464DC8124C775F72");
            entity.ToTable("FoodDatabase", "dbo");
            entity.Property(e => e.FoodItemId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Category).HasMaxLength(50).IsUnicode(false);
            entity.Property(e => e.FoodName).IsRequired().HasMaxLength(100).IsUnicode(false);

            entity.HasOne(d => d.Client).WithMany(p => p.FoodDatabases)
                .HasForeignKey(d => d.ClientId)
                .HasConstraintName("FK__FoodDatab__Carbs__0A338187");
        });

        modelBuilder.Entity<HealthMetric>(entity =>
        {
            entity.HasKey(e => e.MetricId).HasName("PK__HealthMe__561056A5AA11C57C");
            entity.ToTable("HealthMetrics", "dbo");
            entity.Property(e => e.MetricId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.BloodPressure).HasMaxLength(20).IsUnicode(false);
            entity.Property(e => e.MeasurementDate).HasColumnType("date");
            entity.Property(e => e.Notes).HasMaxLength(500);

            entity.HasOne(d => d.Client).WithMany(p => p.HealthMetrics)
                .HasForeignKey(d => d.ClientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__HealthMet__Clien__51851410");
        });

        modelBuilder.Entity<ExercisePlan>(entity =>
        {
            entity.HasKey(e => e.ExercisePlanId).HasName("PK__Exercise__8A3C5C19A698591E");
            entity.ToTable("ExercisePlans", "dbo");
            entity.Property(e => e.ExercisePlanId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedDate).HasColumnType("date");
            entity.Property(e => e.ExerciseName).IsRequired().HasMaxLength(100).IsUnicode(false);
            entity.Property(e => e.Frequency).IsRequired().HasMaxLength(50).IsUnicode(false);

            entity.HasOne(d => d.Client).WithMany(p => p.ExercisePlans)
                .HasForeignKey(d => d.ClientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ExerciseP__Clien__451F3D2B");
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
            entity.Property(e => e.ContentTitle).IsRequired().HasMaxLength(255);
            entity.Property(e => e.ContentType).IsRequired().HasMaxLength(50);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())").HasColumnType("datetime");
            entity.Property(e => e.PublishedDate).HasColumnType("datetime");
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Tags).HasMaxLength(255);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.Url).HasMaxLength(255);

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
            entity.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValueSql("((1))");
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

