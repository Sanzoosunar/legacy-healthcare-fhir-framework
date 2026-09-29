using LegacyHealthcareFHIR.Core.Enums;

namespace LegacyHealthcareFHIR.Core.Interfaces;

public interface IJobProcessor
{
    public JobStage _currentStage { get; }
    public Task ExecuteAsync(Guid jobId);
}