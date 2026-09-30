using LegacyHealthcareFHIR.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LegacyHealthcareFHIR.Infrastructure.Data;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(AppDbContext dbContext)
    {
        await SeedHospitalsAsync(dbContext);
        await SeedUsersAsync(dbContext);
    }

    private static async Task SeedHospitalsAsync(AppDbContext dbContext)
    {
        var hospitals = new[]
        {
            new Hospital
            {
                HospitalKey = "hosp_demo_001",
                Name = "Pinecrest Medical Center Demo",
                Code = "DEMO-RURAL",
                IsActive = true
            },
            new Hospital
            {
                HospitalKey = "hosp_demo_002",
                Name = "Riverside Community Hospital Demo",
                Code = "DEMO-COMMUNITY",
                IsActive = true
            },
            new Hospital
            {
                HospitalKey = "hosp_demo_003",
                Name = "Summit Regional Hospital Demo",
                Code = "DEMO-REGIONAL",
                IsActive = true
            }
        };

        foreach (var hospital in hospitals)
        {
            var exists = await dbContext.Hospitals.AnyAsync(
                h => h.HospitalKey == hospital.HospitalKey);

            if (!exists)
            {
                dbContext.Hospitals.Add(hospital);
            }
        }

        await dbContext.SaveChangesAsync();
    }

    private static async Task SeedUsersAsync(AppDbContext dbContext)
    {
        var demoUsers = new[]
        {
            new
            {
                Username = "demo1",
                Password = "demo1",
                HospitalKey = "hosp_demo_001"
            },
            new
            {
                Username = "demo2",
                Password = "demo2",
                HospitalKey = "hosp_demo_002"
            },
            new
            {
                Username = "demo3",
                Password = "demo3",
                HospitalKey = "hosp_demo_003"
            }
        };

        var passwordHasher = new PasswordHasher<AppUser>();

        foreach (var demo in demoUsers)
        {
            var exists = await dbContext.Users.AnyAsync(
                u => u.Username == demo.Username);

            if (exists)
            {
                continue;
            }

            var hospital = await dbContext.Hospitals.SingleAsync(
                h => h.HospitalKey == demo.HospitalKey);

            var user = new AppUser
            {
                Username = demo.Username,
                HospitalId = hospital.Id
            };

            user.PasswordHash = passwordHasher.HashPassword(
                user,
                demo.Password);

            dbContext.Users.Add(user);
        }

        await dbContext.SaveChangesAsync();
    }
}