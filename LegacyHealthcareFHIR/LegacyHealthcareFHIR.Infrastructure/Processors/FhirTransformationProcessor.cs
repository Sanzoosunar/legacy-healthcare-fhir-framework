using LegacyHealthcareFHIR.Core.Enums;
using LegacyHealthcareFHIR.Core.Interfaces;
using LegacyHealthcareFHIR.Core.Models;
using LegacyHealthcareFHIR.Core.Validation;
using LegacyHealthcareFHIR.Infrastructure.Csv;
using LegacyHealthcareFHIR.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LegacyHealthcareFHIR.Infrastructure.Processors;

public class FhirTransformationProcessor
{
    private readonly IJobRepository _jobRepo;
    private readonly AppDbContext _dbContext;
    private readonly IBackgroundTaskQueue _queue;
    private readonly ISignalRNotifier _signalRNotifier;
    private readonly LocalFileStorageService _fileStorage;
    private readonly LegacyCsvReader _csvReader;
    private readonly ILegacyDataConverter _legacyDataConverter;
    private readonly INormalizedDataTransformer _normalizedDataTransformer;
    private readonly JobStage _currentStage = JobStage.FhirTransformation;

    public FhirTransformationProcessor(
        IJobRepository jobRepo,
        AppDbContext dbContext,
        LocalFileStorageService fileStorage,
        LegacyCsvReader csvReader,
        ILegacyDataConverter legacyDataConverter,
        INormalizedDataTransformer normalizedDataTransformer,
        IBackgroundTaskQueue queue,
        ISignalRNotifier signalRNotifier)
    {
        _jobRepo = jobRepo;
        _dbContext = dbContext;
        _fileStorage = fileStorage;
        _csvReader = csvReader;
        _legacyDataConverter = legacyDataConverter;
        _normalizedDataTransformer = normalizedDataTransformer;
        _queue = queue;
        _signalRNotifier = signalRNotifier;
    }

    public async Task ExecuteAsync(Guid jobId)
    {
        var job = await _dbContext.ImportJobs
            .Include(x => x.ResourceTypeDetection)
            .Include(x => x.MappingConfiguration)
            .ThenInclude(x => x!.FieldMappings)
            .FirstOrDefaultAsync(x => x.Id == jobId) ?? throw new Exception("job not found");

        ValidateJob(job);

        job.JobStage = _currentStage;
        job.Status = JobStatus.InProgress;
        await _jobRepo.UpdateAsync(job);

        try
        {
            using var stream = _fileStorage.OpenRead(job.StoredFileName);

            var csvResult = _csvReader.Read(stream);

            if (!csvResult.IsSuccess) throw new Exception("failed to read source file");

            var resourceType = job.ResourceTypeDetection!.ResourceType;

            var mappings = job.MappingConfiguration!.FieldMappings
                .Where(x => !string.IsNullOrWhiteSpace(x.NormalizedField))
                .ToDictionary(x => x.SourceField, x => x.NormalizedField!);

            var normalizedData = _legacyDataConverter.Convert(resourceType, csvResult.Records, mappings);

            var transformedData = _normalizedDataTransformer.Transform(resourceType, normalizedData);
        }
        catch (Exception ex)
        {
            job.Status = JobStatus.Failed;
            await _jobRepo.UpdateAsync(job);

            await _signalRNotifier.SendAsync(job.Id, job.JobStage, job.Status, ex.Message);
        }
    }

    private void ValidateJob(ImportJob job)
    {
        if (job.ResourceTypeDetection == null) throw new Exception("missing resource type");
        if (!job.ResourceTypeDetection.IsApproved) throw new Exception("resource type is not approved");
        if (job.MappingConfiguration == null) throw new Exception("missing mapping configuration");
        if (!job.MappingConfiguration.IsApproved) throw new Exception("mapping configuration is not approved");

        var isValidState = JobStateValidator.IsValid(job.JobStage, job.Status, _currentStage);

        if (!isValidState) throw new Exception("Not valid stage or status");
    }
}