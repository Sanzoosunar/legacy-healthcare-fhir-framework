using LegacyHealthcareFHIR.Core.Enums;
using LegacyHealthcareFHIR.Core.Interfaces;
using LegacyHealthcareFHIR.Infrastructure.Processors;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public class JobWorker : BackgroundService
{
    private readonly IBackgroundTaskQueue _queue;
    private readonly IServiceScopeFactory _scopeFactory;

    public JobWorker(IBackgroundTaskQueue queue, IServiceScopeFactory scopeFactory)
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

            await ProcessAsync(scope.ServiceProvider, message.Stage, message.JobId);
        }
    }

    private static async Task ProcessAsync(IServiceProvider serviceProvider, JobStage stage, Guid jobId)
    {
        switch (stage)
        {
            case JobStage.ResourceTypeDetection:
                var detectionProcessor = serviceProvider.GetRequiredService<ResourceTypeDetectionProcessor>();
                await detectionProcessor.ExecuteAsync(jobId);
                break;

            case JobStage.FieldMapping:
                //var mappingProcessor = serviceProvider.GetRequiredService<FieldMappingProcessor>();
                //await mappingProcessor.ExecuteAsync(jobId);
                break;

            default:
                throw new InvalidOperationException($"Unsupported job stage: {stage}");
        }
    }
}