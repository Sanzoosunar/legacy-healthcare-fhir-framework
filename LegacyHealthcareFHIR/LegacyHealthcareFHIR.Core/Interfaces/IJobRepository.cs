using LegacyHealthcareFHIR.Core.Models;

public interface IJobRepository
{
    Task<ImportJob> GetAsync(Guid jobId);
    Task<ImportJob> UpdateAsync(ImportJob job);
}