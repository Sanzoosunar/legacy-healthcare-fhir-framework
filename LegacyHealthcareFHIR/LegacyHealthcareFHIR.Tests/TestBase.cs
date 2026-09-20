using LegacyHealthcareFHIR.Core.Interfaces;
using LegacyHealthcareFHIR.Infrastructure.Data;
using LegacyHealthcareFHIR.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace LegacyHealthcareFHIR.Tests;

public abstract class TestBase : IAsyncLifetime
{
    protected readonly AppDbContext _dbcontext;

    protected readonly Mock<IJobRepository> _jobRepositoryMock;


    protected readonly Mock<IResourceTypeAiService> _resourceTypeAiServiceMock;
    protected readonly Mock<ISignalRNotifier> _signalRNotifierMock;
    protected readonly Mock<IFieldMappingAiService> _fieldMappingAiServiceMock;


    protected readonly ResourceTypeDetectionService _resourceTypeDetectionService;
    protected readonly ImportsService _importService;
    protected readonly FieldMappingService _fieldMappingService;
    protected TestBase()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _dbcontext = new AppDbContext(options);

        _jobRepositoryMock = new Mock<IJobRepository>();

        _resourceTypeAiServiceMock = new Mock<IResourceTypeAiService>();
        _signalRNotifierMock = new Mock<ISignalRNotifier>();
        _fieldMappingAiServiceMock = new Mock<IFieldMappingAiService>();

        _resourceTypeDetectionService = new ResourceTypeDetectionService(_dbcontext, _resourceTypeAiServiceMock.Object);
        _importService = new ImportsService(_dbcontext);
        _fieldMappingService = new FieldMappingService(_dbcontext, _fieldMappingAiServiceMock.Object);
    }

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    public virtual async Task DisposeAsync()
    {
        await _dbcontext.DisposeAsync();
    }
}