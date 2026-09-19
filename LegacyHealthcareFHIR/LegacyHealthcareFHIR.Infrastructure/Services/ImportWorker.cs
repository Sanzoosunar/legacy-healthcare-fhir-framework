namespace LegacyHealthcareFHIR.Infrastructure.Services;

public class ImportWorker
{
    public async Task ProcessAsync(Guid importJobId)
    {
        await StartJobAsync(importJobId);

        await ReadSourceFileAsync(importJobId);

        await ProcessMappingAsync(importJobId);

        await ValidateDataAsync(importJobId);

        await TransformToFhirAsync(importJobId);

        await ValidateFhirAsync(importJobId);

        await StoreFhirResourcesAsync(importJobId);

        await CompleteJobAsync(importJobId);
    }

    private async Task StartJobAsync(Guid importJobId)
    {
        // TODO
    }

    private async Task ReadSourceFileAsync(Guid importJobId)
    {
        // TODO
    }

    private async Task ProcessMappingAsync(Guid importJobId)
    {
        // TODO
    }

    private async Task ValidateDataAsync(Guid importJobId)
    {
        // TODO
    }

    private async Task TransformToFhirAsync(Guid importJobId)
    {
        // TODO
    }

    private async Task ValidateFhirAsync(Guid importJobId)
    {
        // TODO
    }

    private async Task StoreFhirResourcesAsync(Guid importJobId)
    {
        // TODO
    }

    private async Task CompleteJobAsync(Guid importJobId)
    {
        // TODO
    }
}