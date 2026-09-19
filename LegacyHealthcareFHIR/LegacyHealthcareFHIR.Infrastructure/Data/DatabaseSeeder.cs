using LegacyHealthcareFHIR.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace LegacyHealthcareFHIR.Infrastructure.Data;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(AppDbContext dbContext)
    {
        if (await dbContext.Hospitals.AnyAsync())
        {
            return;
        }

        var hospital = new Hospital
        {
            HospitalKey = "hosp_demo_001",
            Name = "Demo Rural Hospital",
            Code = "DEMO-RURAL",
            IsActive = true
        };

        dbContext.Hospitals.Add(hospital);
        await dbContext.SaveChangesAsync();

        var configuration = new HospitalConfiguration
        {
            HospitalId = hospital.Id,
            SourceSystemName = "Demo Legacy EHR",
            InputFormat = "CSV",
            EnableAiValidation = true,
            EnableAiMapping = true,
            EnableNotifications = false
        };

        dbContext.HospitalConfigurations.Add(configuration);

        await dbContext.SaveChangesAsync();
    }
}