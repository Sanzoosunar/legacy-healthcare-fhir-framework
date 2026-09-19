using LegacyHealthcareFHIR.Core.Enums;
using LegacyHealthcareFHIR.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LegacyHealthcareFHIR.Infrastructure.Services;

public class ImportPreparationWorker
{
    private readonly AppDbContext _dbContext;
    public ImportPreparationWorker(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task PrepareAsync(Guid importJobId)
    {
        await StartPreparationAsync(importJobId);

        await ReadSourceFileAsync(importJobId);

        await DetectResourceTypeAsync(importJobId);

        await ResolveFieldMappingsAsync(importJobId);

        await CompletePreparationAsync(importJobId);
    }

    private async Task StartPreparationAsync(Guid importJobId)
    {
        var job = await _dbContext.ImportJobs.FindAsync(importJobId);

        if (job == null)
        {
            throw new InvalidOperationException(
                $"Import job '{importJobId}' was not found.");
        }

        job.Status = JobStatus.Preparing;
        job.ProgressPercentage = 0;
        job.StartedAtUtc = DateTime.UtcNow;
        job.ErrorMessage = null;

        await _dbContext.SaveChangesAsync();
    }

    private async Task ReadSourceFileAsync(Guid importJobId)
    {
        // TODO
    }

    private async Task DetectResourceTypeAsync(Guid importJobId)
    {
        // TODO
    }

    private async Task ResolveFieldMappingsAsync(Guid importJobId)
    {
        // TODO
    }

    private async Task CompletePreparationAsync(Guid importJobId)
    {
        // TODO
    }
}