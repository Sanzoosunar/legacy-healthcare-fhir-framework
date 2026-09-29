using LegacyHealthcareFHIR.Core.Enums;
using LegacyHealthcareFHIR.Core.Interfaces;
using LegacyHealthcareFHIR.Core.Models;
using LegacyHealthcareFHIR.Core.Models.Import;

namespace LegacyHealthcareFHIR.Infrastructure.Services;

public class ImportsService
{
    private readonly LocalFileStorageService _fileStorage;
    private readonly IBackgroundTaskQueue _queue;
    private readonly IJobRepository _jobRepository;
    public ImportsService(LocalFileStorageService fileStorage, IBackgroundTaskQueue queue, IJobRepository jobRepository)
    {
        _fileStorage = fileStorage;
        _queue = queue;
        _jobRepository = jobRepository;
    }
    public async Task<ImportJob> CreateImportJob(Stream fileStream, string originalFileName, int hospitalId, string inputFormat)
    {
        var extension = Path.GetExtension(originalFileName);
        var storedFileName = $"{DateTime.UtcNow:yyyyMMdd_HHmmssfff}{extension}";

        var job = await _jobRepository.AddNewJob(hospitalId, originalFileName, storedFileName, inputFormat);

        await _fileStorage.SaveAsync(fileStream, storedFileName);
        await _queue.EnqueueAsync(JobStage.ResourceTypeDetection, job.Id);

        return job;
    }
    public async Task<bool> ApproveResourceTypeAsync(Guid jobId, FhirResourceType resourceType)
    {
        var job = await _jobRepository.GetWithDetails(jobId);
        if (job.JobStage != JobStage.ResourceTypeDetection) throw new Exception("not valid stage");

        if (job.ResourceTypeDetectionId == null) throw new Exception("Resource not exists");

        var detection = job.ResourceTypeDetection!;

        detection.ResourceType = resourceType;
        detection.IsApproved = true;

        job.Status = JobStatus.Completed;

        await _jobRepository.Update(job);

        await _queue.EnqueueAsync(JobStage.FieldMapping, job.Id);
        return true;
    }

    public async Task<bool> ApproveFieldMappingAsync(Guid jobId , ApproveFieldMappingRequest request)
    {
        var job = await _jobRepository.GetWithDetails(jobId);

        if (job.JobStage != JobStage.FieldMapping) throw new Exception("Invalid job stage");
        if (job.MappingConfigurationId == null) throw new Exception("Mapping configuration not exists");
       
        var configuration = job.MappingConfiguration!;
        if (request.Mappings.Count != configuration.FieldMappings.Count) throw new Exception("All field mappings are required");

        var fieldMappings = configuration.FieldMappings;
        foreach (var mappingRequest in request.Mappings)
        {
            var mapping = fieldMappings.FirstOrDefault(x => x.Id == mappingRequest.MappingId)
                ?? throw new Exception("Field mapping not exists");

            mapping.NormalizedField = mappingRequest.NormalizedField!;
        }

        configuration.IsApproved = true;
        job.Status = JobStatus.Completed;

        await _jobRepository.Update(job);

        await _queue.EnqueueAsync(JobStage.DataValidation, job.Id);
        return true;
    }
}