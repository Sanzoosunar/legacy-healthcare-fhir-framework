using Hl7.Fhir.Model;
using LegacyHealthcareFHIR.Core.Enums;
using LegacyHealthcareFHIR.Core.Interfaces;
using LegacyHealthcareFHIR.Core.Models;
using LegacyHealthcareFHIR.Core.Models.Transformed;
using LegacyHealthcareFHIR.Core.Transformation;
using LegacyHealthcareFHIR.Core.Validation;
using LegacyHealthcareFHIR.Infrastructure.Csv;
using LegacyHealthcareFHIR.Infrastructure.Data;
using Task = System.Threading.Tasks.Task;

namespace LegacyHealthcareFHIR.Infrastructure.Processors;

public class FhirTransformationProcessor: IJobProcessor
{
    private readonly IJobRepository _jobRepo;
    private readonly IBackgroundTaskQueue _queue;
    private readonly ISignalRNotifier _signalRNotifier;
    private readonly LocalFileStorageService _fileStorage;
    private readonly LegacyCsvReader _csvReader;
    private readonly ILegacyDataConverter _legacyDataConverter;
    private readonly INormalizedDataTransformer _normalizedDataTransformer;
    public JobStage _currentStage => JobStage.FhirTransformation;

    public FhirTransformationProcessor(
        IJobRepository jobRepo,
        LocalFileStorageService fileStorage,
        LegacyCsvReader csvReader,
        ILegacyDataConverter legacyDataConverter,
        INormalizedDataTransformer normalizedDataTransformer,
        IBackgroundTaskQueue queue,
        ISignalRNotifier signalRNotifier)
    {
        _jobRepo = jobRepo;
        _fileStorage = fileStorage;
        _csvReader = csvReader;
        _legacyDataConverter = legacyDataConverter;
        _normalizedDataTransformer = normalizedDataTransformer;
        _queue = queue;
        _signalRNotifier = signalRNotifier;
    }

    public async Task ExecuteAsync(Guid jobId)
    {
        var job = await _jobRepo.GetWithDetails(jobId);
        ValidateJob(job);

        await _jobRepo.UpdateStageAndStatus(jobId,_currentStage,JobStatus.InProgress);
        await _signalRNotifier.SendAsync(jobId, _currentStage, job.Status);
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

            var resourceData = transformedData
                        .Cast<FhirTransformedData>()
                        .Select(x => x.Resource)
                        .ToList();
            ;
            var bundle = FhirSerializationUtility.CreateBundle(resourceData);

            var json = FhirSerializationUtility.Serialize(bundle);

            var outputFileName = $"{job.Id}-fhir.json";

            var bytes = System.Text.Encoding.UTF8.GetBytes(json);
            using var jsonStream = new MemoryStream(bytes);

            await _fileStorage.SaveAsync(jsonStream,outputFileName);

            job.Status = JobStatus.Completed;
            job.OutputFileName = outputFileName;
            await _jobRepo.UpdateAsync(job);

            await _signalRNotifier.SendAsync(jobId, _currentStage, job.Status);
            await _queue.EnqueueAsync(JobStage.Completed, jobId);
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