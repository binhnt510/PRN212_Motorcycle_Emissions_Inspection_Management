using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace App_MotoEmissions.Models;

public partial class PVehicleContext : DbContext
{
    public PVehicleContext()
    {
    }

    public PVehicleContext(DbContextOptions<PVehicleContext> options)
        : base(options)
    {
    }

    public virtual DbSet<EmissionTest> EmissionTests { get; set; }

    public virtual DbSet<Inspection> Inspections { get; set; }

    public virtual DbSet<InspectionCenter> InspectionCenters { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<UserAccount> UserAccounts { get; set; }

    public virtual DbSet<Vehicle> Vehicles { get; set; }

    public virtual DbSet<Violation> Violations { get; set; }

    //    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
    //        => optionsBuilder.UseSqlServer("Server=LAPTOP-82HD84H0;uid=binhnt;password=123;database=P_Vehicle;Encrypt=True;TrustServerCertificate=True;");
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {

        var builder = new ConfigurationBuilder();
        builder.SetBasePath(Directory.GetCurrentDirectory());
        builder.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
        var configuration = builder.Build();
        optionsBuilder.UseSqlServer(configuration.GetConnectionString("Default"));

    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EmissionTest>(entity =>
        {
            entity.HasKey(e => e.TestId).HasName("PK__Emission__8CC33100E55E3BF7");

            entity.ToTable("EmissionTest");

            entity.Property(e => e.TestId).HasColumnName("TestID");
            entity.Property(e => e.CoLevel).HasColumnName("CO_Level");
            entity.Property(e => e.HcLevel).HasColumnName("HC_Level");
            entity.Property(e => e.InspectionId).HasColumnName("InspectionID");
            entity.Property(e => e.NoxLevel).HasColumnName("NOx_Level");
            entity.Property(e => e.Result).HasMaxLength(10);
            entity.Property(e => e.TestDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Inspection).WithMany(p => p.EmissionTests)
                .HasForeignKey(d => d.InspectionId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__EmissionT__Inspe__4F7CD00D");
        });

        modelBuilder.Entity<Inspection>(entity =>
        {
            entity.HasKey(e => e.InspectionId).HasName("PK__Inspecti__30B2DC285021614D");

            entity.ToTable("Inspection");

            entity.Property(e => e.InspectionId).HasColumnName("InspectionID");
            entity.Property(e => e.CenterId).HasColumnName("CenterID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.InspectorId).HasColumnName("InspectorID");
            entity.Property(e => e.ScheduledDate).HasColumnType("datetime");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Pending");
            entity.Property(e => e.VehicleId).HasColumnName("VehicleID");

            entity.HasOne(d => d.Center).WithMany(p => p.Inspections)
                .HasForeignKey(d => d.CenterId)
                .HasConstraintName("FK__Inspectio__Cente__5070F446");

            entity.HasOne(d => d.Inspector).WithMany(p => p.Inspections)
                .HasForeignKey(d => d.InspectorId)
                .HasConstraintName("FK__Inspectio__Inspe__5165187F");

            entity.HasOne(d => d.Vehicle).WithMany(p => p.Inspections)
                .HasForeignKey(d => d.VehicleId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Inspectio__Vehic__52593CB8");
        });

        modelBuilder.Entity<InspectionCenter>(entity =>
        {
            entity.HasKey(e => e.CenterId).HasName("PK__Inspecti__398FC7D7F7B4A826");

            entity.ToTable("InspectionCenter");

            entity.HasIndex(e => e.Email, "UQ__Inspecti__A9D105347B5B089F").IsUnique();

            entity.Property(e => e.CenterId).HasColumnName("CenterID");
            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.PhoneNumber).HasMaxLength(15);
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__Notifica__20CF2E326775F791");

            entity.ToTable("Notification");

            entity.Property(e => e.NotificationId).HasColumnName("NotificationID");
            entity.Property(e => e.Message).HasMaxLength(255);
            entity.Property(e => e.SentDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Notificat__UserI__534D60F1");
        });

        modelBuilder.Entity<UserAccount>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__UserAcco__1788CCAC04085AB2");

            entity.ToTable("UserAccount");

            entity.HasIndex(e => e.Email, "UQ__UserAcco__A9D105347915F558").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.CenterId).HasColumnName("CenterID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.PhoneNumber).HasMaxLength(15);
            entity.Property(e => e.Role).HasMaxLength(50);

            entity.HasOne(d => d.Center).WithMany(p => p.UserAccounts)
                .HasForeignKey(d => d.CenterId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__UserAccou__Cente__5441852A");
        });

        modelBuilder.Entity<Vehicle>(entity =>
        {
            entity.HasKey(e => e.VehicleId).HasName("PK__Vehicle__476B54B28FCB049B");

            entity.ToTable("Vehicle");

            entity.HasIndex(e => e.LicensePlate, "UQ__Vehicle__026BC15C0ACDD367").IsUnique();

            entity.Property(e => e.VehicleId).HasColumnName("VehicleID");
            entity.Property(e => e.Brand).HasMaxLength(50);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FuelType).HasMaxLength(20);
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("isActive");
            entity.Property(e => e.LicensePlate).HasMaxLength(20);
            entity.Property(e => e.Model).HasMaxLength(50);
            entity.Property(e => e.OwnerId).HasColumnName("OwnerID");

            entity.HasOne(d => d.Owner).WithMany(p => p.Vehicles)
                .HasForeignKey(d => d.OwnerId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Vehicle__OwnerID__5535A963");
        });

        modelBuilder.Entity<Violation>(entity =>
        {
            entity.HasKey(e => e.ViolationId).HasName("PK__Violatio__18B6DC2839134BA9");

            entity.ToTable("Violation");

            entity.Property(e => e.ViolationId).HasColumnName("ViolationID");
            entity.Property(e => e.FineAmount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.PoliceId).HasColumnName("PoliceID");
            entity.Property(e => e.Status).HasMaxLength(25);
            entity.Property(e => e.VehicleId).HasColumnName("VehicleID");
            entity.Property(e => e.ViolationDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ViolationDetails).HasMaxLength(255);

            entity.HasOne(d => d.Police).WithMany(p => p.Violations)
                .HasForeignKey(d => d.PoliceId)
                .HasConstraintName("FK__Violation__Polic__5629CD9C");

            entity.HasOne(d => d.Vehicle).WithMany(p => p.Violations)
                .HasForeignKey(d => d.VehicleId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Violation__Vehic__571DF1D5");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
