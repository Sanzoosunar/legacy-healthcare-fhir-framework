using System;
using System.Collections.Generic;
using System.Text;
using LegacyHealthcareFHIR.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace LegacyHealthcareFHIR.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Hospital> Hospitals => Set<Hospital>();

    public DbSet<HospitalConfiguration> HospitalConfigurations
        => Set<HospitalConfiguration>();

    public DbSet<ImportJob> ImportJobs => Set<ImportJob>();

    public DbSet<MappingProfile> MappingProfiles => Set<MappingProfile>();

    public DbSet<MappingField> MappingFields => Set<MappingField>();

    public DbSet<FhirResource> FhirResources => Set<FhirResource>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Hospital
        modelBuilder.Entity<Hospital>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.HospitalKey)
                .IsRequired();

            entity.Property(x => x.Name)
                .IsRequired();

            entity.Property(x => x.Code)
                .IsRequired();

            entity.HasIndex(x => x.HospitalKey)
                .IsUnique();

            entity.HasIndex(x => x.Code)
                .IsUnique();
        });

        // Hospital Configuration
        modelBuilder.Entity<HospitalConfiguration>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.HasIndex(x => x.HospitalId)
                .IsUnique();

            entity.HasOne<Hospital>()
                .WithOne()
                .HasForeignKey<HospitalConfiguration>(x => x.HospitalId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Import Job
        modelBuilder.Entity<ImportJob>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.HasOne<Hospital>()
                .WithMany()
                .HasForeignKey(x => x.HospitalId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Mapping Profile
        modelBuilder.Entity<MappingProfile>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.HasOne<Hospital>()
                .WithMany()
                .HasForeignKey(x => x.HospitalId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Mapping Field
        modelBuilder.Entity<MappingField>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.HasOne<MappingProfile>()
                .WithMany()
                .HasForeignKey(x => x.MappingProfileId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // FHIR Resource
        modelBuilder.Entity<FhirResource>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.HasOne<Hospital>()
                .WithMany()
                .HasForeignKey(x => x.HospitalId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne<ImportJob>()
                .WithMany()
                .HasForeignKey(x => x.ImportJobId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
