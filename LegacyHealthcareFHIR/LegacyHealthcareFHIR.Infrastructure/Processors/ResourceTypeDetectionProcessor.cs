using LegacyHealthcareFHIR.Core.Enums;
using LegacyHealthcareFHIR.Core.Interfaces;
using LegacyHealthcareFHIR.Core.Models;
using LegacyHealthcareFHIR.Core.Models.Detection;
using LegacyHealthcareFHIR.Core.Models.Import;
using LegacyHealthcareFHIR.Infrastructure.Services;

namespace LegacyHealthcareFHIR.Infrastructure.Processors;

public class ResourceTypeDetectionProcessor
{
    private readonly ISignalRNotifier _signalRNotifier;
    private readonly SourceFileService _sourceFileService;
    private readonly ResourceTypeDetectionService _resourceTypeDetectionService;
    private readonly IJobRepository _jobRepo;

    public ResourceTypeDetectionProcessor(
        ISignalRNotifier signalRNotifier,
        SourceFileService sourceFileService,
        ResourceTypeDetectionService resourceTypeDetectionService,
        IJobRepository jobRepo)
    {
        _signalRNotifier = signalRNotifier;
        _sourceFileService = sourceFileService;
        _resourceTypeDetectionService = resourceTypeDetectionService;
        _jobRepo = jobRepo;
    }

    public async Task ExecuteAsync(Guid jobId)
    {
        var job = await StartDetectionAsync(jobId);

        var sourceFileData = await ReadSourceFileAsync(job);

        await DetectResourceTypeAsync(job, sourceFileData);
    }

    private async Task<ImportJob> StartDetectionAsync(Guid jobId)
    {
        var job = await _jobRepo.GetAsync(jobId);

        job.Status = JobStatus.InProgress;
        job.JobStage = JobStage.ResourceTypeDetection;

        await _jobRepo.UpdateAsync(job);

        await _signalRNotifier.SendAsync(job.Id, job.JobStage, job.Status);

        return job;
    }

    private async Task<SourceFileData> ReadSourceFileAsync(ImportJob job)
    {
        return await _sourceFileService.ReadAndSaveAsync(job.Id, job.StoredFileName);
    }

    private async Task<ResourceTypeDetectionResult> DetectResourceTypeAsync(ImportJob job, SourceFileData sourceFileData)
    {
        await _signalRNotifier.SendAsync(job.Id, JobStage.ResourceTypeDetection, JobStatus.AiSuggestionWaiting);

        var result = await _resourceTypeDetectionService.DetectAsync(job.HospitalId, sourceFileData);

        await _signalRNotifier.SendAsync(job.Id, JobStage.ResourceTypeDetection, JobStatus.AiSuggested, result);

        return result;
    }
}