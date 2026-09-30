using LegacyHealthcareFHIR.Core.Enums;
using LegacyHealthcareFHIR.Core.Models;

public interface IJobRepository
{
    Task<ImportJob> AddNewJob(int hospitalId, int userId, string originalFileName, string storedFileName, string format);
    Task<ImportJob> Get(Guid jobId);
    Task<ImportJob> GetWithDetails(Guid jobId);
    Task UpdateStageAndStatus(Guid jobId, JobStage stage, JobStatus status);
    Task UpdateStatus(Guid jobId, JobStatus status);

    Task<ImportJob> Update(ImportJob job);
    Task<ImportJob> GetAsync(Guid jobId);
    Task<ResourceTypeDetection?> GetResourceTypeDetectionAsync(int resourceTypeId);
}