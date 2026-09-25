using LegacyHealthcareFHIR.Core.Enums;
using LegacyHealthcareFHIR.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public class BackgroundJobWorker : BackgroundService
{
    private readonly IBackgroundTaskQueue _queue;
    private readonly IServiceScopeFactory _scopeFactory;

    public BackgroundJobWorker(IBackgroundTaskQueue queue, IServiceScopeFactory scopeFactory)
    {
        _queue = queue;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var message = await _queue.DequeueAsync(stoppingToken);

            using var scope = _scopeFactory.CreateScope();

            var processor = scope.ServiceProvider
                                .GetServices<IJobProcessor>()
                                .FirstOrDefault(x => x._currentStage == message.Stage)
                                ?? throw new InvalidOperationException($"No processor registered for stage: {message.Stage}");

            await processor.ExecuteAsync(message.JobId);
        }
    }
}