using LegacyHealthcareFHIR.Core.Enums;
using LegacyHealthcareFHIR.Core.Models;
using LegacyHealthcareFHIR.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LegacyHealthcareFHIR.Infrastructure.Services;

public class ImportsService
{
    private readonly AppDbContext _dbContext;
    public ImportsService(
        AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> ApproveResourceTypeAsync(Guid jobId, int detectionId, FhirResourceType resourceType)
    {
        var job = await _dbContext.ImportJobs.FindAsync(jobId) ?? throw new Exception("Job not exists");

        var detection = await _dbContext.ResourceTypeDetections.FindAsync(detectionId) ?? throw new Exception("Detection not exists"); ;

        if (detection.HospitalId != job.HospitalId)
        {
            throw new Exception("Job and Detection don't match");
        }

        detection.ResourceType = resourceType;
        detection.IsApproved = true;

        job.ResourceType = resourceType;

        await _dbContext.SaveChangesAsync();

        return true;
    }
}