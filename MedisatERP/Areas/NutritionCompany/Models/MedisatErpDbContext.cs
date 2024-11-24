//using System;
//using System.Collections.Generic;
//using Microsoft.EntityFrameworkCore;

//namespace MedisatERP.Areas.NutritionCompany.Models;

//public partial class MedisatErpDbContext : DbContext
//{
//    public MedisatErpDbContext()
//    {
//    }

//    //public MedisatErpDbContext(DbContextOptions<MedisatErpDbContext> options)
//        : base(options)
//    {
//    }

//    public virtual DbSet<Allergy> Allergies { get; set; }

//    public virtual DbSet<CompanyClient> CompanyClients { get; set; }

//    public virtual DbSet<DietPlan> DietPlans { get; set; }

//    public virtual DbSet<FoodDatabase> FoodDatabases { get; set; }

//    public virtual DbSet<MealLogging> MealLoggings { get; set; }

//    public virtual DbSet<MealPlan> MealPlans { get; set; }

//    public virtual DbSet<MedicalCondition> MedicalConditions { get; set; }

//    public virtual DbSet<NutritionalProfile> NutritionalProfiles { get; set; }

//    public virtual DbSet<Report> Reports { get; set; }

//    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
//        => optionsBuilder.UseSqlServer("Data Source=medisaterp.lyfexafrica.com;uid=adminMedisatERP;pwd=Planchinobo256;TrustServerCertificate=True;");

//    protected override void OnModelCreating(ModelBuilder modelBuilder)
//    {
//        modelBuilder.HasDefaultSchema("adminMedisatERP");

//        modelBuilder.Entity<Allergy>(entity =>
//        {
//            entity.HasKey(e => e.AllergyId).HasName("PK__Allergie__A49EBE42E4BFCC36");

//            entity.ToTable("Allergies", "dbo");

//            entity.Property(e => e.AllergyId).HasDefaultValueSql("(newid())");
//            entity.Property(e => e.AllergyName)
//                .IsRequired()
//                .HasMaxLength(100)
//                .IsUnicode(false);
//            entity.Property(e => e.Severity)
//                .HasMaxLength(50)
//                .IsUnicode(false);

//            entity.HasOne(d => d.Client).WithMany(p => p.Allergies)
//                .HasForeignKey(d => d.ClientId)
//                .HasConstraintName("FK__Allergies__Clien__55BFB948");
//        });

//        modelBuilder.Entity<CompanyClient>(entity =>
//        {
//            entity.HasKey(e => e.ClientId);

//            entity.ToTable("CompanyClients", "dbo");

//            entity.Property(e => e.ClientId).HasDefaultValueSql("(newid())");
//            entity.Property(e => e.ClientName)
//                .IsRequired()
//                .HasMaxLength(200);
//            entity.Property(e => e.CreatedAt)
//                .HasDefaultValueSql("(getdate())")
//                .HasColumnType("datetime");
//            entity.Property(e => e.DateOfBirth).HasColumnType("datetime");
//            entity.Property(e => e.Email)
//                .IsRequired()
//                .HasMaxLength(256);
//            entity.Property(e => e.EmergencyContactName).HasMaxLength(256);
//            entity.Property(e => e.EmergencyContactPhone).HasMaxLength(15);
//            entity.Property(e => e.Gender)
//                .IsRequired()
//                .HasMaxLength(10);
//            entity.Property(e => e.MaritalStatus)
//                .IsRequired()
//                .HasMaxLength(50);
//            entity.Property(e => e.Nationality)
//                .IsRequired()
//                .HasMaxLength(100);
//            entity.Property(e => e.PhoneNumber)
//                .IsRequired()
//                .HasMaxLength(15);
//            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
//        });

//        modelBuilder.Entity<DietPlan>(entity =>
//        {
//            entity.HasKey(e => e.DietPlanId).HasName("PK__DietPlan__D256E10A3F13B2C9");

//            entity.ToTable("DietPlan", "dbo");

//            entity.Property(e => e.DietPlanId).HasDefaultValueSql("(newid())");
//            entity.Property(e => e.CreatedBy)
//                .IsRequired()
//                .HasMaxLength(100)
//                .IsUnicode(false);
//            entity.Property(e => e.DietPlanName)
//                .IsRequired()
//                .HasMaxLength(100)
//                .IsUnicode(false);
//            entity.Property(e => e.EndDate).HasColumnType("date");
//            entity.Property(e => e.LastUpdated).HasColumnType("datetime");
//            entity.Property(e => e.StartDate).HasColumnType("date");

//            entity.HasOne(d => d.Allergy).WithMany(p => p.DietPlans)
//                .HasForeignKey(d => d.AllergyId)
//                .HasConstraintName("FK__DietPlan__Allerg__7167D3BD");

//            entity.HasOne(d => d.Client).WithMany(p => p.DietPlans)
//                .HasForeignKey(d => d.ClientId)
//                .HasConstraintName("FK__DietPlan__Client__7073AF84");

//            entity.HasOne(d => d.Condition).WithMany(p => p.DietPlans)
//                .HasForeignKey(d => d.ConditionId)
//                .HasConstraintName("FK__DietPlan__Condit__725BF7F6");
//        });

//        modelBuilder.Entity<FoodDatabase>(entity =>
//        {
//            entity.HasKey(e => e.FoodItemId).HasName("PK__FoodData__464DC8124C775F72");

//            entity.ToTable("FoodDatabase", "dbo");

//            entity.Property(e => e.FoodItemId).HasDefaultValueSql("(newid())");
//            entity.Property(e => e.Category)
//                .HasMaxLength(50)
//                .IsUnicode(false);
//            entity.Property(e => e.FoodName)
//                .IsRequired()
//                .HasMaxLength(100)
//                .IsUnicode(false);

//            entity.HasOne(d => d.Client).WithMany(p => p.FoodDatabases)
//                .HasForeignKey(d => d.ClientId)
//                .HasConstraintName("FK__FoodDatab__Carbs__0A338187");
//        });

//        modelBuilder.Entity<MealLogging>(entity =>
//        {
//            entity.HasKey(e => e.LogId).HasName("PK__MealLogg__5E548648CA2986E7");

//            entity.ToTable("MealLogging", "dbo");

//            entity.Property(e => e.LogId).HasDefaultValueSql("(newid())");
//            entity.Property(e => e.LogDate).HasColumnType("datetime");

//            entity.HasOne(d => d.Client).WithMany(p => p.MealLoggings)
//                .HasForeignKey(d => d.ClientId)
//                .HasConstraintName("FK__MealLoggi__Clien__15A53433");

//            entity.HasOne(d => d.FoodItem).WithMany(p => p.MealLoggings)
//                .HasForeignKey(d => d.FoodItemId)
//                .HasConstraintName("FK__MealLoggi__FoodI__178D7CA5");

//            entity.HasOne(d => d.MealPlan).WithMany(p => p.MealLoggings)
//                .HasForeignKey(d => d.MealPlanId)
//                .HasConstraintName("FK__MealLoggi__MealP__1699586C");
//        });

//        modelBuilder.Entity<MealPlan>(entity =>
//        {
//            entity.HasKey(e => e.MealPlanId).HasName("PK__MealPlan__0620DB7677F15438");

//            entity.ToTable("MealPlans", "dbo");

//            entity.Property(e => e.MealPlanId).HasDefaultValueSql("(newid())");
//            entity.Property(e => e.FoodCategory)
//                .HasMaxLength(50)
//                .IsUnicode(false);
//            entity.Property(e => e.MealName)
//                .IsRequired()
//                .HasMaxLength(100)
//                .IsUnicode(false);

//            entity.HasOne(d => d.DietPlan).WithMany(p => p.MealPlans)
//                .HasForeignKey(d => d.DietPlanId)
//                .HasConstraintName("FK__MealPlans__DietP__11D4A34F");
//        });

//        modelBuilder.Entity<MedicalCondition>(entity =>
//        {
//            entity.HasKey(e => e.ConditionId).HasName("PK__MedicalC__37F5C0CF98AE7599");

//            entity.ToTable("MedicalConditions", "dbo");

//            entity.Property(e => e.ConditionId).HasDefaultValueSql("(newid())");
//            entity.Property(e => e.ConditionName)
//                .IsRequired()
//                .HasMaxLength(100)
//                .IsUnicode(false);
//            entity.Property(e => e.DiagnosisDate).HasColumnType("date");

//            entity.HasOne(d => d.Client).WithMany(p => p.MedicalConditions)
//                .HasForeignKey(d => d.ClientId)
//                .HasConstraintName("FK__MedicalCo__Clien__59904A2C");
//        });

//        modelBuilder.Entity<NutritionalProfile>(entity =>
//        {
//            entity.HasKey(e => e.ProfileId).HasName("PK__Nutritio__290C88E46E0FE18E");

//            entity.ToTable("NutritionalProfile", "dbo");

//            entity.Property(e => e.ProfileId).HasDefaultValueSql("(newid())");
//            entity.Property(e => e.Height)
//                .HasColumnType("decimal(5, 2)")
//                .HasColumnName("height");
//            entity.Property(e => e.Weight)
//                .HasColumnType("decimal(5, 2)")
//                .HasColumnName("weight");

//            entity.HasOne(d => d.Allergy).WithMany(p => p.NutritionalProfiles)
//                .HasForeignKey(d => d.AllergyId)
//                .HasConstraintName("FK__Nutrition__Aller__047AA831");

//            entity.HasOne(d => d.Client).WithMany(p => p.NutritionalProfiles)
//                .HasForeignKey(d => d.ClientId)
//                .HasConstraintName("FK__Nutrition__Clien__038683F8");

//            entity.HasOne(d => d.Condition).WithMany(p => p.NutritionalProfiles)
//                .HasForeignKey(d => d.ConditionId)
//                .HasConstraintName("FK__Nutrition__Condi__056ECC6A");

//            entity.HasOne(d => d.DietPlan).WithMany(p => p.NutritionalProfiles)
//                .HasForeignKey(d => d.DietPlanId)
//                .HasConstraintName("FK__Nutrition__DietP__0662F0A3");
//        });

//        modelBuilder.Entity<Report>(entity =>
//        {
//            entity.HasKey(e => e.ReportId).HasName("PK__Report__D5BD48054B37C2EE");

//            entity.ToTable("Report", "dbo");

//            entity.Property(e => e.ReportId).HasDefaultValueSql("(newid())");
//            entity.Property(e => e.ReportContent).HasColumnType("text");
//            entity.Property(e => e.ReportDate).HasColumnType("datetime");
//            entity.Property(e => e.ReportType)
//                .IsRequired()
//                .HasMaxLength(100)
//                .IsUnicode(false);

//            entity.HasOne(d => d.Client).WithMany(p => p.Reports)
//                .HasForeignKey(d => d.ClientId)
//                .HasConstraintName("FK__Report__ClientId__6CA31EA0");
//        });

//        OnModelCreatingPartial(modelBuilder);
//    }

//    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
//}
