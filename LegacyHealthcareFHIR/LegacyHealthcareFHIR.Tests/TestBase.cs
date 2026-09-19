using LegacyHealthcareFHIR.Core.Interfaces;
using LegacyHealthcareFHIR.Infrastructure.Data;
using LegacyHealthcareFHIR.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace LegacyHealthcareFHIR.Tests;

public abstract class TestBase : IAsyncLifetime
{
    protected readonly AppDbContext DbContext;
    protected readonly Mock<IResourceTypeAiService> AiService;
    protected readonly Mock<ISignalRNotifier> SignalRNotifier;
    protected readonly ResourceTypeDetectionService ResourceTypeDetectionService;
    protected readonly ImportsService ImportsService;

    protected TestBase()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        DbContext = new AppDbContext(options);
        AiService = new Mock<IResourceTypeAiService>();
        SignalRNotifier = new Mock<ISignalRNotifier>();

        ResourceTypeDetectionService = new ResourceTypeDetectionService(DbContext, AiService.Object);
        ImportsService = new ImportsService(DbContext);
    }

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    public virtual async Task DisposeAsync()
    {
        await DbContext.DisposeAsync();
    }
}