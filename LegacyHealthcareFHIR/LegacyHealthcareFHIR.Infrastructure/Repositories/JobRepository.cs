using LegacyHealthcareFHIR.Core.Enums;
using LegacyHealthcareFHIR.Core.Interfaces;
using LegacyHealthcareFHIR.Core.Models;
using LegacyHealthcareFHIR.Infrastructure.Data;

namespace LegacyHealthcareFHIR.Infrastructure.Repositories;

public class JobRepository : IJobRepository
{
    private readonly AppDbContext _dbContext;

    public JobRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ImportJob> GetAsync(Guid jobId)
    {
        return await _dbContext.ImportJobs.FindAsync(jobId)
            ?? throw new Exception("Import job does not exist.");
    }

    public async Task<ImportJob> UpdateAsync(ImportJob job)
    {
        _dbContext.ImportJobs.Update(job);
        await _dbContext.SaveChangesAsync();

        return job;
    }
}