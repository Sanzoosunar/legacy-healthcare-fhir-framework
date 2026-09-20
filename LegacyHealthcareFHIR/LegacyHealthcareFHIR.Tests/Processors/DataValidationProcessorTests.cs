using LegacyHealthcareFHIR.Core.Enums;
using LegacyHealthcareFHIR.Core.Interfaces;
using LegacyHealthcareFHIR.Core.Models;
using LegacyHealthcareFHIR.Core.Models.Legacy;
using LegacyHealthcareFHIR.Core.Models.Normalized;
using LegacyHealthcareFHIR.Core.Validation;
using LegacyHealthcareFHIR.Infrastructure.Csv;
using LegacyHealthcareFHIR.Infrastructure.Processors;
using Moq;

namespace LegacyHealthcareFHIR.Tests.Processors;

public class DataValidationProcessorTests : TestBase
{
    private readonly DataValidationProcessor _processor;
    public DataValidationProcessorTests()
    {
        _processor = new DataValidationProcessor(
            _jobRepositoryMock.Object,
            _dbcontext,
            _fileStorage,
            _csvReader,
            _legacyDataConverterMock.Object,
            _legacyDataValidatorMock.Object,
            _backgroundTaskQueueMock.Object,
            _signalRNotifierMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_JobNotFound_ThrowsException()
    {
        await Assert.ThrowsAsync<Exception>(() => _processor.ExecuteAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task ExecuteAsync_MissingResourceType_ThrowsException()
    {
        var job = CreateValidJob();
        job.ResourceTypeDetection = null;
        job.ResourceTypeDetectionId = null;

        _dbcontext.ImportJobs.Add(job);
        await _dbcontext.SaveChangesAsync();

        await Assert.ThrowsAsync<Exception>(() => _processor.ExecuteAsync(job.Id));
    }

    [Fact]
    public async Task ExecuteAsync_ResourceTypeNotApproved_ThrowsException()
    {
        var job = CreateValidJob();
        job.ResourceTypeDetection!.IsApproved = false;

        _dbcontext.ImportJobs.Add(job);
        await _dbcontext.SaveChangesAsync();

        await Assert.ThrowsAsync<Exception>(() => _processor.ExecuteAsync(job.Id));
    }

    [Fact]
    public async Task ExecuteAsync_MissingMappingConfiguration_ThrowsException()
    {
        var job = CreateValidJob();
        job.MappingConfiguration = null;
        job.MappingConfigurationId = null;

        _dbcontext.ImportJobs.Add(job);
        await _dbcontext.SaveChangesAsync();

        await Assert.ThrowsAsync<Exception>(() => _processor.ExecuteAsync(job.Id));
    }

    [Fact]
    public async Task ExecuteAsync_MappingConfigurationNotApproved_ThrowsException()
    {
        var job = CreateValidJob();
        job.MappingConfiguration!.IsApproved = false;

        _dbcontext.ImportJobs.Add(job);
        await _dbcontext.SaveChangesAsync();

        await Assert.ThrowsAsync<Exception>(() => _processor.ExecuteAsync(job.Id));
    }

    [Fact]
    public async Task ExecuteAsync_MissingSourceFile_SetsStatusFailed()
    {
        var job = CreateValidJob();

        _dbcontext.ImportJobs.Add(job);
        await _dbcontext.SaveChangesAsync();

        await _processor.ExecuteAsync(job.Id);

        Assert.Equal(JobStatus.Failed, job.Status);
        Assert.Equal(JobStage.DataValidation, job.JobStage);
    }

    [Fact]
    public async Task ExecuteAsync_ValidationFailed_SetsStatusFailed()
    {
        var job = CreateValidJob();

        await CreateCsvAsync(job.StoredFileName);

        _dbcontext.ImportJobs.Add(job);
        await _dbcontext.SaveChangesAsync();

        _legacyDataConverterMock
            .Setup(x => x.Convert(It.IsAny<FhirResourceType>(), It.IsAny<List<LegacyRecord>>(), It.IsAny<Dictionary<string, string>>()))
            .Returns(new List<NormalizedData> { new PatientData() });

        _legacyDataValidatorMock
            .Setup(x => x.Validate(It.IsAny<FhirResourceType>(), It.IsAny<List<NormalizedData>>()))
            .Returns(new LegacyDataValidationResult
            {
                Errors =
                [
                    new LegacyDataValidationError
                    {
                        RowNumber = 2,
                        FieldName = "PatientId",
                        Error = "Patient ID is required"
                    }
                ]
            });

        await _processor.ExecuteAsync(job.Id);

        Assert.Equal(JobStatus.Failed, job.Status);
        Assert.Equal(JobStage.DataValidation, job.JobStage);
    }

    [Fact]
    public async Task ExecuteAsync_ValidationSucceeded_SetsStatusCompleted()
    {
        var job = CreateValidJob();

        await CreateCsvAsync(job.StoredFileName);

        _dbcontext.ImportJobs.Add(job);
        await _dbcontext.SaveChangesAsync();

        _legacyDataConverterMock
            .Setup(x => x.Convert(It.IsAny<FhirResourceType>(), It.IsAny<List<LegacyRecord>>(), It.IsAny<Dictionary<string, string>>()))
            .Returns(new List<NormalizedData> { new PatientData() });

        _legacyDataValidatorMock
            .Setup(x => x.Validate(It.IsAny<FhirResourceType>(), It.IsAny<List<NormalizedData>>()))
            .Returns(new LegacyDataValidationResult());

        await _processor.ExecuteAsync(job.Id);

        Assert.Equal(JobStatus.Completed, job.Status);
        Assert.Equal(JobStage.DataValidation, job.JobStage);
    }

    private ImportJob CreateValidJob()
    {
        return new ImportJob
        {
            Id = Guid.NewGuid(),
            HospitalId = 1,
            OriginalFileName = "patients.csv",
            StoredFileName = $"{Guid.NewGuid()}.csv",
            InputFormat = "CSV",
            JobStage = JobStage.FieldMapping,
            Status = JobStatus.Completed,
            ResourceTypeDetection = new ResourceTypeDetection
            {
                HospitalId = 1,
                SchemaFingerprint = Guid.NewGuid().ToString(),
                ResourceType = FhirResourceType.Patient,
                IsApproved = true
            },
            MappingConfiguration = new MappingConfiguration
            {
                HospitalId = 1,
                SchemaFingerprint = Guid.NewGuid().ToString(),
                ResourceType = FhirResourceType.Patient,
                IsApproved = true,
                FieldMappings =
                [
                    new FieldMapping
                    {
                        SourceField = "P_ID",
                        NormalizedField = nameof(PatientData.PatientId)
                    }
                ]
            }
        };
    }

    private async Task CreateCsvAsync(string fileName)
    {
        var path = Path.Combine(_uploadDirectory, fileName);
        await File.WriteAllTextAsync(path, "P_ID\n123");
    }

    public override async Task DisposeAsync()
    {
        await base.DisposeAsync();

        if (Directory.Exists(_uploadDirectory))
        {
            Directory.Delete(_uploadDirectory, true);
        }
    }
}