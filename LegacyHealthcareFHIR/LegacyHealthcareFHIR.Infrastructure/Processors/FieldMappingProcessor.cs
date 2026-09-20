using LegacyHealthcareFHIR.Core.Enums;
using LegacyHealthcareFHIR.Core.Interfaces;
using LegacyHealthcareFHIR.Core.Models;
using LegacyHealthcareFHIR.Core.Models.Import;
using LegacyHealthcareFHIR.Core.Models.Mapping;
using LegacyHealthcareFHIR.Infrastructure.Data;
using LegacyHealthcareFHIR.Infrastructure.Services;

namespace LegacyHealthcareFHIR.Infrastructure.Processors;

public class FieldMappingProcessor
{
    private readonly AppDbContext _dbContext;
    private readonly SourceFileService _sourceFileService;
    private readonly FieldMappingService _fieldMappingService;
    private readonly ISignalRNotifier _notifier;

    public FieldMappingProcessor(AppDbContext dbContext, SourceFileService sourceFileService, FieldMappingService fieldMappingService, ISignalRNotifier notifier)
    {
        _dbContext = dbContext;
        _sourceFileService = sourceFileService;
        _fieldMappingService = fieldMappingService;
        _notifier = notifier;
    }

    public async Task ExecuteAsync(Guid jobId)
    {
        var job = await _dbContext.ImportJobs.FindAsync(jobId)
            ?? throw new InvalidOperationException("Import job does not exist.");

        if (job.ResourceType == null)
        {
            throw new InvalidOperationException("Resource type has not been approved.");
        }

        await _notifier.SendAsync(jobId, JobStage.FieldMapping, JobStatus.InProgress);

        var sourceFileData = await GetHeaderAndSampleDataAsync(job);

        await GetAiSuggestedFieldMappingAsync(job, sourceFileData);
    }

    private async Task<SourceFileData> GetHeaderAndSampleDataAsync(ImportJob job)
    {
        return await _sourceFileService.GetOrCreate(job.Id, job.StoredFileName);
    }

    private async Task<FieldMappingResult> GetAiSuggestedFieldMappingAsync(ImportJob job, SourceFileData sourceFileData)
    {
        await _notifier.SendAsync(sourceFileData.ImportJobId, JobStage.FieldMapping, JobStatus.AiSuggestionWaiting);
        var aiSuggestedFields =  await _fieldMappingService.GetOrCreateAsync(job.HospitalId, job.ResourceType!.Value, sourceFileData);
        await _notifier.SendAsync(job.Id, JobStage.FieldMapping, JobStatus.AiSuggested, aiSuggestedFields);

        return aiSuggestedFields;
    }
}