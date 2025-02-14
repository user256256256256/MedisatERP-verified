﻿using System;
using System.Collections.Generic;
using MedisatERP.Areas.AdministratorSystem.Models;
using MedisatERP.Areas.NutritionCompanySystem.Models;
using MedisatERP.Models;
using Microsoft.EntityFrameworkCore;

namespace MedisatERP.Data;

public partial class AdministratorSystemDbContext : DbContext
{
    private readonly SharedDbContext _sharedDbContext;

    public AdministratorSystemDbContext()
    {
    }

    public AdministratorSystemDbContext(DbContextOptions<AdministratorSystemDbContext> options, SharedDbContext sharedDbContext )
        : base(options)
    {
        _sharedDbContext = sharedDbContext;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=MedisatConnection");



    public virtual DbSet<AspNetRole> AspNetRoles { get; set; }

    public virtual DbSet<AspNetRoleClaim> AspNetRoleClaims { get; set; }

    public virtual DbSet<AspNetUser> AspNetUsers { get; set; }

    public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }

    public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }

    public virtual DbSet<AspNetUserToken> AspNetUserTokens { get; set; }

    public virtual DbSet<AspNetUserRoles> AspNetUserRoles { get; set; }

    public virtual DbSet<AuditLog> AuditLogs { get; set; }

    public virtual DbSet<BillingCycleLookup> BillingCycleLookups { get; set; }

    public virtual DbSet<CompanyClient> CompanyClients { get; set; }

    public virtual DbSet<ClientAddress> ClientAddresses { get; set; }

    public virtual DbSet<Company> Companies { get; set; }

    public virtual DbSet<CompanyAddress> CompanyAddresses { get; set; }

    public virtual DbSet<CompanyStatusLookup> CompanyStatusLookup { get; set; }

    public virtual DbSet<DataMigration> DataMigrations { get; set; }

    public virtual DbSet<Feedback> Feedbacks { get; set; }
 
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


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("adminMedisatERP");

        // Configure AspNetRole
        modelBuilder.Entity<AspNetRole>(entity =>
        {
            entity.HasIndex(e => e.NormalizedName, "RoleNameIndex")
                .IsUnique()
                .HasFilter("([NormalizedName] IS NOT NULL)");

            entity.Property(e => e.Name).HasMaxLength(256);
            entity.Property(e => e.NormalizedName).HasMaxLength(256);
        });

        // Configure AspNetRoleClaim
        modelBuilder.Entity<AspNetRoleClaim>(entity =>
        {
            entity.HasIndex(e => e.RoleId, "IX_AspNetRoleClaims_RoleId");

            entity.Property(e => e.RoleId).IsRequired();

            entity.HasOne(d => d.Role).WithMany(p => p.AspNetRoleClaims).HasForeignKey(d => d.RoleId);
        });

        // Configure AspNetUser
        modelBuilder.Entity<AspNetUser>(entity =>
        {
            entity.HasIndex(e => e.NormalizedEmail, "EmailIndex");

            entity.HasIndex(e => e.NormalizedUserName, "UserNameIndex")
                .IsUnique()
                .HasFilter("([NormalizedUserName] IS NOT NULL)");

            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.NormalizedEmail).HasMaxLength(256);
            entity.Property(e => e.NormalizedUserName).HasMaxLength(256);
            entity.Property(e => e.ProfileImagePath).HasMaxLength(255);
            entity.Property(e => e.UserName).HasMaxLength(256);

            // Add the CompanyId property and configure the relationship
            entity.Property(e => e.CompanyId).IsRequired(false);

            // Configure ProfileImagePath property
            entity.Property(e => e.ProfileImagePath)
                .HasMaxLength(255)
                .IsRequired(false);

            // Configure BioData property
            entity.Property(e => e.BioData)
                .HasMaxLength(500)  // Adjust the max length as needed
                .IsRequired(false);
        });

        // Configure AspNetUserRoles
        modelBuilder.Entity<AspNetUserRoles>(entity =>
        {
            entity.HasKey(ur => new { ur.UserId, ur.RoleId });

            entity.HasOne(ur => ur.User)
                .WithMany(u => u.AspNetUserRoles)
                .HasForeignKey(ur => ur.UserId);

            entity.HasOne(ur => ur.Role)
                .WithMany(r => r.AspNetUserRoles)
                .HasForeignKey(ur => ur.RoleId);
        });

        // Configure AspNetUserClaim
        modelBuilder.Entity<AspNetUserClaim>(entity =>
        {
            entity.HasIndex(e => e.UserId, "IX_AspNetUserClaims_UserId");

            entity.Property(e => e.UserId).IsRequired();

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserClaims).HasForeignKey(d => d.UserId);
        });

        // Configure AspNetUserLogin
        modelBuilder.Entity<AspNetUserLogin>(entity =>
        {
            entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });

            entity.HasIndex(e => e.UserId, "IX_AspNetUserLogins_UserId");

            entity.Property(e => e.UserId).IsRequired();

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserLogins).HasForeignKey(d => d.UserId);
        });

        // Configure AspNetUserToken
        modelBuilder.Entity<AspNetUserToken>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserTokens).HasForeignKey(d => d.UserId);
        });

        // Configure AuditLog
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.AuditLogId).HasName("PK__AuditLog__EB5F6CBD52DD8874");

            entity.ToTable("AuditLogs", "dbo");

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

        // Configure BillingCycleLookup
        modelBuilder.Entity<BillingCycleLookup>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__BillingC__3214EC07271F0E00");

            entity.ToTable("BillingCycleLookup", "dbo");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CycleName)
                .IsRequired()
                .HasMaxLength(50);
        });


        modelBuilder.Entity<ClientAddress>(entity =>
        {
            entity.HasKey(e => e.AddressId);

            entity.ToTable("ClientAddress", "dbo");

            entity.Property(e => e.AddressId).HasDefaultValueSql("(newid())");
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

        modelBuilder.Entity<ClientMembership>(entity =>
        {
            entity.HasKey(e => e.ClientMembershipId).HasName("PK__ClientMe__D13287670A7BEE58");

            entity.ToTable("ClientMemberships", "dbo");

            entity.Property(e => e.ClientMembershipId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValueSql("((1))");
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
            entity.Property(e => e.CompanyEmail).HasMaxLength(256);
            entity.Property(e => e.CompanyInitials).HasMaxLength(10);
            entity.Property(e => e.CompanyLogoFilePath).HasMaxLength(511);
            entity.Property(e => e.CompanyName).HasMaxLength(256);
            entity.Property(e => e.CompanyPhone).HasMaxLength(15);
            entity.Property(e => e.CompanyType).HasMaxLength(100);
            entity.Property(e => e.ContactPerson).HasMaxLength(256);
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.Motto).HasMaxLength(512);

            entity.HasOne(d => d.Address).WithMany(p => p.Companies)
                .HasForeignKey(d => d.AddressId)
                .HasConstraintName("FK_Companies_Address");

            entity.HasOne(d => d.CompanyStatus).WithMany(p => p.Companies)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK_CompanyStatus");
        });

        modelBuilder.Entity<CompanyAddress>(entity =>
        {
            entity.HasKey(e => e.AddressId);

            entity.ToTable("CompanyAddress", "dbo");

            entity.Property(e => e.AddressId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.Country).HasMaxLength(100);
            entity.Property(e => e.PostalCode).HasMaxLength(20);
            entity.Property(e => e.State).HasMaxLength(100);
            entity.Property(e => e.Street).HasMaxLength(256);
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

            entity.HasOne(d => d.Address).WithMany(p => p.CompanyClients)
                .HasForeignKey(d => d.AddressId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Client_Address");

            entity.HasOne(d => d.Company).WithMany(p => p.CompanyClients)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Company_Client");

            // Explicitly configure the FollowUp relationships
            entity.HasMany(d => d.FollowUpClients).WithOne(p => p.Client)
                .HasForeignKey(p => p.ClientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FollowUps_Client");

            entity.HasMany(d => d.FollowUpPractitioners).WithOne(p => p.Practitioner)
                .HasForeignKey(p => p.PractitionerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FollowUps_Practitioner");

            // Configure relationship to Appointments
            entity.HasMany(d => d.Appointments).WithOne(p => p.Client)
                .HasForeignKey(p => p.ClientId)
                .OnDelete(DeleteBehavior.Cascade) // Use Cascade delete to avoid foreign key conflict
                .HasConstraintName("FK_Appointments_Client");
        });



        modelBuilder.Entity<CompanyStatusLookup>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("PK__CompanyS__C8EE20635B13B506");

            entity.ToTable("CompanyStatusLookup", "dbo");

            entity.Property(e => e.StatusName)
                .IsRequired()
                .HasMaxLength(50);
        });



        modelBuilder.Entity<DataMigration>(entity =>
        {
            entity.HasKey(e => e.MigrationId).HasName("PK__DataMigr__E5D3573B63B85DFE");

            entity.ToTable("DataMigration", "dbo");

            entity.Property(e => e.MigrationId).HasDefaultValueSql("(newid())");
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





        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.HasKey(e => e.FeedbackId).HasName("PK__Feedback__6A4BEDD6D92678A5");

            entity.ToTable("Feedback", "dbo");

            entity.Property(e => e.FeedbackId).HasDefaultValueSql("(newid())");
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





