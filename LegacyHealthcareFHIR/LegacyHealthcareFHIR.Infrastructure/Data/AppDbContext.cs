using LegacyHealthcareFHIR.Core.Models;
using LegacyHealthcareFHIR.Core.Models.Import;
using LegacyHealthcareFHIR.Core.Models.Legacy;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

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

    public DbSet<MappingConfiguration> MappingConfigurations => Set<MappingConfiguration>();

    public DbSet<FieldMapping> FieldMappings => Set<FieldMapping>();
    public DbSet<FhirResource> FhirResources => Set<FhirResource>();
    public DbSet<ResourceTypeDetection> ResourceTypeDetections
    => Set<ResourceTypeDetection>();
    public DbSet<SourceFileData> SourceFileData => Set<SourceFileData>();
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

        modelBuilder.Entity<MappingConfiguration>()
         .HasIndex(x => new
         {
             x.HospitalId,
             x.SchemaFingerprint,
             x.ResourceType
         })
         .IsUnique();

        modelBuilder.Entity<FieldMapping>()
            .HasOne(x => x.MappingConfiguration)
            .WithMany(x => x.FieldMappings)
            .HasForeignKey(x => x.ConfigId)
            .OnDelete(DeleteBehavior.Cascade);

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

        modelBuilder.Entity<ResourceTypeDetection>()
            .HasIndex(x => new
            {
                x.HospitalId,
                x.SchemaFingerprint
            })
            .IsUnique();

        modelBuilder.Entity<SourceFileData>()
            .Property(x => x.Headers)
            .HasConversion(
                x => JsonSerializer.Serialize(x, (JsonSerializerOptions?)null),
                x => JsonSerializer.Deserialize<List<string>>(x, (JsonSerializerOptions?)null) ?? new());

        modelBuilder.Entity<SourceFileData>()
            .Property(x => x.SampleRecords)
            .HasConversion(
                x => JsonSerializer.Serialize(x, (JsonSerializerOptions?)null),
                x => JsonSerializer.Deserialize<List<LegacyRecord>>(x, (JsonSerializerOptions?)null) ?? new());

        modelBuilder.Entity<SourceFileData>()
            .HasIndex(x => x.ImportJobId)
            .IsUnique();
    }
}
