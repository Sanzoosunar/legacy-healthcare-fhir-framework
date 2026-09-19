using LegacyHealthcareFHIR.Core.Enums;
using LegacyHealthcareFHIR.Core.Models;
using LegacyHealthcareFHIR.Infrastructure.Services;


namespace LegacyHealthcareFHIR.Tests.Services;

public class ImportsServiceTests : TestBase
{
    private readonly ImportJob _job;
    private readonly ResourceTypeDetection _detection;

    public ImportsServiceTests()
    {
        _job = new ImportJob
        {
            Id = Guid.NewGuid(),
            HospitalId = 1,
            OriginalFileName = "patients.csv",
            StoredFileName = "stored_patients.csv",
            InputFormat = "CSV",
            Status = JobStatus.Pending
        };

        _detection = new ResourceTypeDetection
        {
            HospitalId = 1,
            SchemaFingerprint = "patient-schema",
            ResourceType = FhirResourceType.Observation,
            AiConfidence = 0.85m,
            IsApproved = false
        };
    }

    private async Task SaveJobAndDetectionAsync()
    {
        DbContext.ImportJobs.Add(_job);
        DbContext.ResourceTypeDetections.Add(_detection);
        await DbContext.SaveChangesAsync();
    }

    [Fact]
    public async Task ApproveResourceTypeAsync_ValidRequest_ApprovesDetectionAndUpdatesJob()
    {
        await SaveJobAndDetectionAsync();

        var result = await ImportsService.ApproveResourceTypeAsync(_job.Id, _detection.Id, FhirResourceType.Patient);

        Assert.True(result);
        Assert.True(_detection.IsApproved);
        Assert.Equal(FhirResourceType.Patient, _detection.ResourceType);
        Assert.Equal(FhirResourceType.Patient, _job.ResourceType);
    }

    [Fact]
    public async Task ApproveResourceTypeAsync_JobDoesNotExist_ThrowsException()
    {
        await Assert.ThrowsAsync<Exception>(() =>
            ImportsService.ApproveResourceTypeAsync(Guid.NewGuid(), 1, FhirResourceType.Patient));
    }

    [Fact]
    public async Task ApproveResourceTypeAsync_DetectionDoesNotExist_ThrowsException()
    {
        DbContext.ImportJobs.Add(_job);
        await DbContext.SaveChangesAsync();

        await Assert.ThrowsAsync<Exception>(() =>
            ImportsService.ApproveResourceTypeAsync(_job.Id, 999, FhirResourceType.Patient));
    }

    [Fact]
    public async Task ApproveResourceTypeAsync_DetectionBelongsToDifferentHospital_ThrowsException()
    {
        _detection.HospitalId = 2;

        await SaveJobAndDetectionAsync();

        await Assert.ThrowsAsync<Exception>(() =>
            ImportsService.ApproveResourceTypeAsync(_job.Id, _detection.Id, FhirResourceType.Patient));
    }
}