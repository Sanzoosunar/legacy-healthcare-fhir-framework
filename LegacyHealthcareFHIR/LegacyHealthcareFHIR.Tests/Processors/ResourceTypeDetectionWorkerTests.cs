using LegacyHealthcareFHIR.Core.Enums;
using LegacyHealthcareFHIR.Core.Models;
using LegacyHealthcareFHIR.Core.Models.Detection;
using LegacyHealthcareFHIR.Core.Models.Import;
using LegacyHealthcareFHIR.Infrastructure.Csv;
using LegacyHealthcareFHIR.Infrastructure.Processors;
using LegacyHealthcareFHIR.Infrastructure.Services;
using Moq;

namespace LegacyHealthcareFHIR.Tests.Workers;

public class ResourceTypeDetectionWorkerTests : TestBase
{
    private readonly ResourceTypeDetectionProcessor _worker;
    private readonly string _tempDirectory;

    public ResourceTypeDetectionWorkerTests()
    {
        _tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(_tempDirectory);

        var fileStorageService = new LocalFileStorageService(_tempDirectory);
        var csvReader = new LegacyCsvReader();
        var sourceFileService = new SourceFileService(fileStorageService, csvReader, _dbcontext);

        _worker = new ResourceTypeDetectionProcessor(
            _signalRNotifierMock.Object,
            sourceFileService,
            _resourceTypeDetectionService,_jobRepository);
    }

    [Fact]
    public async Task ExecuteAsync_ValidJob_StartsDetectionAndCallsAi()
    {
        var storedFileName = "patients.csv";
        var filePath = Path.Combine(_tempDirectory, storedFileName);

        await File.WriteAllTextAsync(
            filePath,
            """
            P_ID,F_NAME,L_NAME,DOB,SEX
            1001,John,Smith,1980-01-01,M
            1002,Mary,Jones,1992-05-14,F
            """);

        var job = CreateJob(storedFileName);

        _dbcontext.ImportJobs.Add(job);
        await _dbcontext.SaveChangesAsync();

        _resourceTypeAiServiceMock
            .Setup(x => x.DetectAsync(It.IsAny<SourceFileData>()))
            .ReturnsAsync(new ResourceTypeDetectionAiResult
            {
                ResourceType = FhirResourceType.Patient,
                AiConfidence = 0.98m
            });

        await _worker.ExecuteAsync(job.Id);

        var updatedJob = await _dbcontext.ImportJobs.FindAsync(job.Id);

        Assert.NotNull(updatedJob);
       
        _resourceTypeAiServiceMock.Verify(x => x.DetectAsync(It.IsAny<SourceFileData>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_JobDoesNotExist_ThrowsInvalidOperationException()
    {
        await Assert.ThrowsAsync<Exception>(() => _worker.ExecuteAsync(Guid.NewGuid()));

        _resourceTypeAiServiceMock.Verify(x => x.DetectAsync(It.IsAny<SourceFileData>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_SourceFileDoesNotExist_ThrowsFileNotFoundException()
    {
        var job = CreateJob("missing.csv");

        _dbcontext.ImportJobs.Add(job);
        await _dbcontext.SaveChangesAsync();

        await Assert.ThrowsAsync<FileNotFoundException>(() => _worker.ExecuteAsync(job.Id));

        _resourceTypeAiServiceMock.Verify(x => x.DetectAsync(It.IsAny<SourceFileData>()), Times.Never);
    }

    private static ImportJob CreateJob(string storedFileName)
    {
        return new ImportJob
        {
            Id = Guid.NewGuid(),
            HospitalId = 1,
            OriginalFileName = storedFileName,
            StoredFileName = storedFileName,
            InputFormat = "CSV",
            Status = JobStatus.Started
        };
    }

    public override async Task DisposeAsync()
    {
        await base.DisposeAsync();

        if (Directory.Exists(_tempDirectory))
        {
            Directory.Delete(_tempDirectory, true);
        }
    }
}