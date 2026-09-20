using LegacyHealthcareFHIR.Core.Interfaces;
using LegacyHealthcareFHIR.Infrastructure.Csv;
using LegacyHealthcareFHIR.Infrastructure.Data;
using LegacyHealthcareFHIR.Infrastructure.Repositories;
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
    protected readonly Mock<LocalFileStorageService> _localFileStorageServiceMock;


    protected readonly ResourceTypeDetectionService _resourceTypeDetectionService;
    protected readonly ImportsService _importService;
    protected readonly FieldMappingService _fieldMappingService;
    protected readonly Mock<IBackgroundTaskQueue> _backgroundTaskQueueMock;

    protected readonly IJobRepository _jobRepository;


    protected readonly Mock<ILegacyDataConverter> _legacyDataConverterMock;
    protected readonly Mock<ILegacyDataValidator> _legacyDataValidatorMock;
    protected readonly LegacyCsvReader _csvReader;
    protected readonly string _uploadDirectory;
    protected readonly LocalFileStorageService _fileStorage;
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

        _localFileStorageServiceMock = new Mock<LocalFileStorageService>("test-uploads");
        _backgroundTaskQueueMock = new Mock<IBackgroundTaskQueue>();

        _resourceTypeDetectionService = new ResourceTypeDetectionService(_dbcontext, _resourceTypeAiServiceMock.Object);
        _importService = new ImportsService(_dbcontext, _localFileStorageServiceMock.Object, _backgroundTaskQueueMock.Object, _jobRepositoryMock.Object);
        _fieldMappingService = new FieldMappingService(_dbcontext, _fieldMappingAiServiceMock.Object);

        _jobRepository = new JobRepository(_dbcontext);

        _legacyDataConverterMock = new Mock<ILegacyDataConverter>();
        _legacyDataValidatorMock = new Mock<ILegacyDataValidator>();
        _csvReader = new LegacyCsvReader();

        _uploadDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(_uploadDirectory);

        _fileStorage = new LocalFileStorageService(_uploadDirectory);
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