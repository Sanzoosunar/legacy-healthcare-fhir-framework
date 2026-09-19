using LegacyHealthcareFHIR.Core.Enums;
using LegacyHealthcareFHIR.Core.Interfaces;
using LegacyHealthcareFHIR.Core.Models;
using LegacyHealthcareFHIR.Core.Models.Import;
using LegacyHealthcareFHIR.Core.Models.Notifications;
using LegacyHealthcareFHIR.Infrastructure.Data;

namespace LegacyHealthcareFHIR.Infrastructure.Services;

public class ResourceTypeDetectionWorker
{
    private readonly AppDbContext _dbContext;
    private readonly ISignalRNotifier _signalRNotifier;
    private readonly SourceFileService _sourceFileService;
    private readonly ResourceTypeDetectionService _resourceTypeDetectionService;

    public ResourceTypeDetectionWorker(
        AppDbContext dbContext,
        ISignalRNotifier signalRNotifier,
        SourceFileService sourceFileService,
        ResourceTypeDetectionService resourceTypeDetectionService)
    {
        _dbContext = dbContext;
        _signalRNotifier = signalRNotifier;
        _sourceFileService = sourceFileService;
        _resourceTypeDetectionService = resourceTypeDetectionService;
    }

    public async Task ExecuteAsync(Guid importJobId)
    {
        var job =
            await StartDetectionAsync(importJobId);

        var sourceFileData =
            ReadSourceFile(job);

        var detectionResult =
            await DetectResourceTypeAsync(
                job,
                sourceFileData);

        await PublishDetectionResultAsync(
                job,
                detectionResult);
    }

    private async Task<ImportJob> StartDetectionAsync(
        Guid importJobId)
    {
        var job =
            await _dbContext.ImportJobs.FindAsync(importJobId);

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

        await _signalRNotifier.SendAsync(
            new JobNotificationEvent
            {
                EventType = JobNotificationEventType.ProgressUpdated,
                JobId = job.Id,
                Status = job.Status,
                ProgressPercentage = job.ProgressPercentage,
                Stage = "Starting",
            });

        return job;
    }

    private SourceFileData ReadSourceFile(
        ImportJob job)
    {
        return _sourceFileService.Read(
            job.StoredFileName);
    }

    private async Task<ResourceTypeDetectionResult>
        DetectResourceTypeAsync(
            ImportJob job,
            SourceFileData sourceFileData)
    {
        var result =
            await _resourceTypeDetectionService.DetectAsync(
                job.HospitalId,
                sourceFileData);

        return result;
    }

    private async Task PublishDetectionResultAsync(
    ImportJob job,
    ResourceTypeDetectionResult detectionResult)
    {
        await _signalRNotifier.SendAsync(
            new JobNotificationEvent
            {
                EventType = JobNotificationEventType.ResourceTypeDetected,
                JobId = job.Id,
                Status = job.Status,
                ProgressPercentage = job.ProgressPercentage,
                Stage = "ResourceTypeDetection",
                Data = detectionResult
            });
    }
}
