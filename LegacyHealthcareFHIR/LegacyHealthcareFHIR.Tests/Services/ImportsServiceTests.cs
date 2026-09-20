using LegacyHealthcareFHIR.Core.Enums;
using LegacyHealthcareFHIR.Core.Models;

namespace LegacyHealthcareFHIR.Tests.Services;

public class ImportsServiceTests : TestBase
{
    private readonly ImportJob _job;
    private readonly ResourceTypeDetection _detection;

    public ImportsServiceTests()
    {
        _detection = new ResourceTypeDetection
        {
            HospitalId = 1,
            SchemaFingerprint = "patient-schema",
            ResourceType = FhirResourceType.Observation,
            AiConfidence = 0.85m,
            IsApproved = false
        };

        _job = new ImportJob
        {
            Id = Guid.NewGuid(),
            HospitalId = 1,
            OriginalFileName = "patients.csv",
            StoredFileName = "stored_patients.csv",
            InputFormat = "CSV",
            Status = JobStatus.Started,
            ResourceTypeDetection = _detection
        };
    }

    private async Task SaveJobAndDetectionAsync()
    {
        _dbcontext.ResourceTypeDetections.Add(_detection);
        _dbcontext.ImportJobs.Add(_job);
        await _dbcontext.SaveChangesAsync();
    }

    [Fact]
    public async Task ApproveResourceTypeAsync_ValidRequest_ApprovesDetection()
    {
        await SaveJobAndDetectionAsync();

        var result = await _importService.ApproveResourceTypeAsync(_job.Id, FhirResourceType.Patient);

        Assert.True(result);
        Assert.True(_detection.IsApproved);
        Assert.Equal(FhirResourceType.Patient, _detection.ResourceType);
    }

    [Fact]
    public async Task ApproveResourceTypeAsync_JobDoesNotExist_ThrowsException()
    {
        await Assert.ThrowsAsync<Exception>(() => _importService.ApproveResourceTypeAsync(Guid.NewGuid(), FhirResourceType.Patient));
    }

    [Fact]
    public async Task ApproveResourceTypeAsync_JobHasNoDetection_ThrowsException()
    {
        _job.ResourceTypeDetection = null;
        _job.ResourceTypeDetectionId = null;

        _dbcontext.ImportJobs.Add(_job);
        await _dbcontext.SaveChangesAsync();

        await Assert.ThrowsAsync<Exception>(() => _importService.ApproveResourceTypeAsync(_job.Id, FhirResourceType.Patient));
    }
}