using LegacyHealthcareFHIR.Core.Enums;
using LegacyHealthcareFHIR.Core.Models;
using LegacyHealthcareFHIR.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LegacyHealthcareFHIR.Infrastructure.Repositories;

public class JobRepository : IJobRepository
{
    private readonly AppDbContext _dbContext;

    public JobRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    public async Task<ImportJob> AddNewJob(int hospitalId, string originalFileName, string storedFileName, string format)
    {
        var jobId = Guid.NewGuid();
        var job = new ImportJob
        {
            Id = jobId,
            HospitalId = hospitalId,
            OriginalFileName = originalFileName,
            StoredFileName = storedFileName,
            InputFormat = format,
            Status = JobStatus.Completed,
            JobStage = JobStage.Created
        };

        _dbContext.ImportJobs.Add(job);
        await _dbContext.SaveChangesAsync();

        return job;
    }

    public async Task<ImportJob> GetAsync(Guid jobId)
    {
        return await _dbContext.ImportJobs.FindAsync(jobId)
            ?? throw new Exception("Import job does not exist.");
    }

    public async Task<ImportJob> Get(Guid jobId)
    {
        return await _dbContext.ImportJobs.FindAsync(jobId)
            ?? throw new Exception("Import job does not exist.");
    }

    public async Task<ImportJob> GetWithDetails(Guid jobId)
    {
        return await _dbContext.ImportJobs
            .Include(a=>a.ResourceTypeDetection)
            .Include(a=>a.MappingConfiguration)
            .ThenInclude(x => x!.FieldMappings)
            .FirstOrDefaultAsync(x=>x.Id == jobId)
            ?? throw new Exception("Import job does not exist.");
    }


    public async Task UpdateStageAndStatus(Guid jobId, JobStage stage, JobStatus status)
    {
        await _dbContext.ImportJobs
           .Where(x => x.Id == jobId)
           .ExecuteUpdateAsync(x => x
               .SetProperty(j => j.JobStage, stage)
               .SetProperty(j => j.Status, status));
    }

    public async Task UpdateStatus(Guid jobId, JobStatus status)
    {
        await _dbContext.ImportJobs
           .Where(x => x.Id == jobId)
           .ExecuteUpdateAsync(x => x
               .SetProperty(j => j.Status, status));
    }

    public async Task<ImportJob> Update(ImportJob job)
    {
        _dbContext.ImportJobs.Update(job);
        await _dbContext.SaveChangesAsync();

        return job;
    }

    public async Task<ImportJob> UpdateAsync(ImportJob job)
    {
        _dbContext.ImportJobs.Update(job);
        await _dbContext.SaveChangesAsync();

        return job;
    }

    public async Task<ResourceTypeDetection?> GetResourceTypeDetectionAsync(int resourceTypeId)
    {
        return await _dbContext.ResourceTypeDetections.FindAsync(resourceTypeId);
    }
}