using LegacyHealthcareFHIR.Core.Enums;
using LegacyHealthcareFHIR.Core.Interfaces;
using LegacyHealthcareFHIR.Core.Models;
using LegacyHealthcareFHIR.Core.Models.Detection;
using LegacyHealthcareFHIR.Core.Models.Import;
using LegacyHealthcareFHIR.Core.Validation;
using LegacyHealthcareFHIR.Infrastructure.Services;

namespace LegacyHealthcareFHIR.Infrastructure.Processors;

public class ResourceTypeDetectionProcessor: IJobProcessor
{
    private readonly ISignalRNotifier _signalRNotifier;
    private readonly SourceFileService _sourceFileService;
    private readonly ResourceTypeDetectionService _resourceTypeDetectionService;
    private readonly IJobRepository _jobRepo;
    public JobStage _currentStage => JobStage.DataValidation;
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

    public void Validate(ImportJob job)
    {
        var isValid = JobStateValidator.IsValid(job.JobStage, job.Status, _currentStage);
        if (!isValid) throw new Exception("Not in valid state");
    }

    public async Task ExecuteAsync(Guid jobId)
    {
        var job = await _jobRepo.Get(jobId);
        await StartDetectionAsync(job);
        var sourceFileData = await ReadSourceFileAsync(job);
        await DetectResourceTypeAsync(job, sourceFileData);
    }

    private async Task StartDetectionAsync(ImportJob job)
    {
        await _jobRepo.UpdateStageAndStatus(job.Id,_currentStage,JobStatus.InProgress);
        await _signalRNotifier.SendAsync(job.Id, job.JobStage, job.Status);
    }

    private async Task<SourceFileData> ReadSourceFileAsync(ImportJob job)
    {
        return await _sourceFileService.ReadAndSaveAsync(job.Id, job.StoredFileName);
    }

    private async Task<ResourceTypeDetectionResult> DetectResourceTypeAsync(ImportJob job, SourceFileData sourceFileData)
    {
        try
        {
            var result = await _resourceTypeDetectionService.DetectAsync(job.HospitalId, sourceFileData);

            job.Status = JobStatus.AiSuggested;
            job.ResourceTypeDetectionId = result.DetectionId;
            await _jobRepo.UpdateAsync(job);

            await _signalRNotifier.SendAsync(job.Id, _currentStage, job.Status, result);

            return result;
        }
        catch (Exception ex)
        {
            await _jobRepo.UpdateStatus(job.Id,JobStatus.Failed);
            await _signalRNotifier.SendAsync(job.Id, _currentStage, JobStatus.Failed, ex.Message);
            throw;
        }
    }
}