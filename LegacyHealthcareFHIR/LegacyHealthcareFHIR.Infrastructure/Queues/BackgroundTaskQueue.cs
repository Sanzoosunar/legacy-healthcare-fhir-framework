using System.Threading.Channels;
using LegacyHealthcareFHIR.Core.Enums;
using LegacyHealthcareFHIR.Core.Interfaces;
using LegacyHealthcareFHIR.Core.Models;

namespace LegacyHealthcareFHIR.Infrastructure.Queues;

public class BackgroundTaskQueue : IBackgroundTaskQueue
{
    private readonly Channel<BackgroundTaskMessage> _queue = Channel.CreateUnbounded<BackgroundTaskMessage>();

    public ValueTask EnqueueAsync(JobStage stage, Guid jobId)
    {
        return _queue.Writer.WriteAsync(new BackgroundTaskMessage
        {
            JobId = jobId,
            Stage = stage
        });
    }

    public ValueTask<BackgroundTaskMessage> DequeueAsync(CancellationToken cancellationToken)
    {
        return _queue.Reader.ReadAsync(cancellationToken);
    }
}