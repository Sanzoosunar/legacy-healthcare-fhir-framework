using LegacyHealthcareFHIR.Core.Enums;
using LegacyHealthcareFHIR.Core.Models;

namespace LegacyHealthcareFHIR.Core.Interfaces;

public interface IBackgroundTaskQueue
{
    ValueTask EnqueueAsync(JobStage stage, Guid jobId);
    ValueTask<BackgroundTaskMessage> DequeueAsync(CancellationToken cancellationToken);
}