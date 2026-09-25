using LegacyHealthcareFHIR.Core.Enums;
using LegacyHealthcareFHIR.Core.Interfaces;
using LegacyHealthcareFHIR.Core.Models;
using LegacyHealthcareFHIR.Core.Models.Import;
using LegacyHealthcareFHIR.Core.Models.Mapping;
using LegacyHealthcareFHIR.Core.Validation;
using LegacyHealthcareFHIR.Infrastructure.Services;

namespace LegacyHealthcareFHIR.Infrastructure.Processors;

public class FieldMappingProcessor : IJobProcessor
{
    private readonly IJobRepository _jobRepo;
    private readonly SourceFileService _sourceFileService;
    private readonly FieldMappingService _fieldMappingService;
    private readonly ISignalRNotifier _notifier;
    public JobStage _currentStage => JobStage.FieldMapping;
    public FieldMappingProcessor(IJobRepository jobRepo, 
        SourceFileService sourceFileService, 
        FieldMappingService fieldMappingService, 
        ISignalRNotifier notifier)
    {
        _jobRepo = jobRepo;
        _sourceFileService = sourceFileService;
        _fieldMappingService = fieldMappingService;
        _notifier = notifier;
    }
    private void ValidateJob(ImportJob job)
    {
        if (job.ResourceTypeDetection == null) throw new Exception("missing resource type");
        if (!job.ResourceTypeDetection.IsApproved) throw new Exception("resource type is not approved");

        var isValidState = JobStateValidator.IsValid(job.JobStage, job.Status, _currentStage);
        if (!isValidState) throw new Exception("Not valid stage or status");
    }

    public async Task ExecuteAsync(Guid jobId)
    {
        var job = await _jobRepo.GetWithDetails(jobId);
        ValidateJob(job);

        try
        {
            await _jobRepo.UpdateStageAndStatus(jobId,_currentStage,JobStatus.InProgress);
            await _notifier.SendAsync(jobId, JobStage.FieldMapping, JobStatus.InProgress);

            var detection = job.ResourceTypeDetection;
            var sourceFileData = await GetHeaderAndSampleDataAsync(job);

            await GetAiSuggestedFieldMappingAsync(job, detection!.ResourceType, sourceFileData);
        }
        catch(Exception ex)
        {
            await _jobRepo.UpdateStageAndStatus(jobId, _currentStage, JobStatus.Failed);
            await _notifier.SendAsync(jobId, _currentStage, JobStatus.Failed,ex.Message);
        }
    }

    private async Task<SourceFileData> GetHeaderAndSampleDataAsync(ImportJob job)
    {
        return await _sourceFileService.GetOrCreate(job.Id, job.StoredFileName);
    }

    private async Task<FieldMappingResult> GetAiSuggestedFieldMappingAsync(ImportJob job, FhirResourceType resourceType ,SourceFileData sourceFileData)
    {
        var aiSuggestedFields =  await _fieldMappingService.GetOrCreateAsync(job.HospitalId, resourceType , sourceFileData);

        job.MappingConfigurationId = aiSuggestedFields.ConfigurationId;
        job.Status = JobStatus.AiSuggested;
        await _jobRepo.UpdateAsync(job);

        await _notifier.SendAsync(job.Id, job.JobStage, job.Status, aiSuggestedFields);

        return aiSuggestedFields;
    }
}