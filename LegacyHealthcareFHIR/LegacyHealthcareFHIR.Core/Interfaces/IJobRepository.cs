using LegacyHealthcareFHIR.Core.Enums;
using LegacyHealthcareFHIR.Core.Models;

public interface IJobRepository
{
    Task<ImportJob> GetAsync(Guid jobId);
    Task<ImportJob> UpdateAsync(ImportJob job);
    Task<ResourceTypeDetection?> GetResourceTypeDetectionAsync(int resourceTypeId);
}