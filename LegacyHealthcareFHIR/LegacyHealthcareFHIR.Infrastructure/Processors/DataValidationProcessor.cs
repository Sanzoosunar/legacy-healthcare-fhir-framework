using LegacyHealthcareFHIR.Core.Enums;
using LegacyHealthcareFHIR.Core.Interfaces;
using LegacyHealthcareFHIR.Core.Models;
using LegacyHealthcareFHIR.Core.Validation;
using LegacyHealthcareFHIR.Infrastructure.Csv;

namespace LegacyHealthcareFHIR.Infrastructure.Processors;

public class DataValidationProcessor: IJobProcessor
{
    private readonly IJobRepository _jobRepo;
    private readonly LocalFileStorageService _fileStorage;
    private readonly LegacyCsvReader _csvReader;
    private readonly ILegacyDataConverter _legacyDataConverter;
    private readonly ILegacyDataValidator _legacyDataValidator;
    private readonly IBackgroundTaskQueue _queue;
    private readonly ISignalRNotifier _signalRNotifier;
    public JobStage _currentStage => JobStage.DataValidation;
    public DataValidationProcessor(IJobRepository jobRepo, 
        LocalFileStorageService fileStorage, 
        LegacyCsvReader csvReader, 
        ILegacyDataConverter legacyDataConverter,
        ILegacyDataValidator legacyDataValidator, 
        IBackgroundTaskQueue queue, 
        ISignalRNotifier signalRNotifier)
    {
        _jobRepo = jobRepo;
        _fileStorage = fileStorage;
        _csvReader = csvReader;
        _legacyDataConverter = legacyDataConverter;
        _legacyDataValidator = legacyDataValidator;
        _queue = queue;
        _signalRNotifier = signalRNotifier;
    }

    public async Task ExecuteAsync(Guid jobId)
    {
        var job = await _jobRepo.GetWithDetails(jobId);
        ValidateJob(job!);
 
        await _jobRepo.UpdateStageAndStatus(jobId,_currentStage, JobStatus.InProgress);

        var resourceType = job.ResourceTypeDetection!.ResourceType;

        var mappings = job.MappingConfiguration!.FieldMappings
            .ToDictionary(x => x.SourceField, x => x.NormalizedField!);

        try
        {
            using var stream = _fileStorage.OpenRead(job.StoredFileName);

            var csvResult = _csvReader.Read(stream);

            if (!csvResult.IsSuccess) throw new Exception("failed to read source file");

            var legacyData = csvResult.Records;

            var normalizedData = _legacyDataConverter.Convert(resourceType, legacyData, mappings);

            var validationResult = _legacyDataValidator.Validate(resourceType, normalizedData);

            if (!validationResult.IsValid)
            {
                await _jobRepo.UpdateStatus(jobId, JobStatus.Failed);

                await _signalRNotifier.SendAsync(job.Id, job.JobStage, job.Status, validationResult);
                return;
            }

            job.Status = JobStatus.Completed;
            await _jobRepo.UpdateAsync(job);

            await _signalRNotifier.SendAsync(job.Id, job.JobStage, job.Status, validationResult);

            await _queue.EnqueueAsync(JobStage.FhirTransformation, job.Id);
        }
        catch(Exception ex)
        {
            await _jobRepo.UpdateStatus(jobId,JobStatus.Failed);
            await _signalRNotifier.SendAsync(job.Id, job.JobStage, job.Status, ex.Message);
        }
    }
    private void ValidateJob(ImportJob job)
    {
        if (job == null) throw new Exception("job not exists");
        if (job.ResourceTypeDetection == null) throw new Exception("missing resource type");
        if (!job.ResourceTypeDetection.IsApproved) throw new Exception("resource type is not approved");
        if (job.MappingConfiguration == null) throw new Exception("missing mapping configuration");
        if (!job.MappingConfiguration.IsApproved) throw new Exception("mapping configuration is not approved");

        var isValidState = JobStateValidator.IsValid(job.JobStage, job.Status, _currentStage);
        if (!isValidState) throw new Exception("Not valid stage or status");
    }
}