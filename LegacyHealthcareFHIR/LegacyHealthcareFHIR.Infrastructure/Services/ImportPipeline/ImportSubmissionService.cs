using LegacyHealthcareFHIR.Core.Enums;
using LegacyHealthcareFHIR.Core.Models;
using LegacyHealthcareFHIR.Infrastructure.Data;

namespace LegacyHealthcareFHIR.Infrastructure.Services.ImportPipeline;

public class ImportSubmissionService
{
    private readonly AppDbContext _dbContext;
    private readonly LocalFileStorageService _fileStorage;

    public ImportSubmissionService(
        AppDbContext dbContext,
        LocalFileStorageService fileStorage)
    {
        _dbContext = dbContext;
        _fileStorage = fileStorage;
    }

    public async Task<ImportJob> SubmitAsync(
    Stream fileStream,
    string originalFileName,
    int hospitalId,
    string inputFormat)
    {
        var jobId = Guid.NewGuid();

        var extension = Path.GetExtension(originalFileName);

        var storedFileName =
            $"{DateTime.UtcNow:yyyyMMdd_HHmmssfff}_{jobId}{extension}";

        var job = new ImportJob
        {
            Id = jobId,
            HospitalId = hospitalId,
            OriginalFileName = originalFileName,
            StoredFileName = storedFileName,
            InputFormat = inputFormat,
            Status = JobStatus.Started,
            JobStage=JobStage.ResourceTypeDetection
        };

        await _fileStorage.SaveAsync(
            fileStream,
            storedFileName);

        _dbContext.ImportJobs.Add(job);

        await _dbContext.SaveChangesAsync();

        return job;
    }
}