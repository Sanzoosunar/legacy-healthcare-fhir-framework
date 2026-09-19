using LegacyHealthcareFHIR.Core.Enums;
using LegacyHealthcareFHIR.Core.Interfaces;
using LegacyHealthcareFHIR.Core.Models;
using LegacyHealthcareFHIR.Core.Models.Import;
using LegacyHealthcareFHIR.Core.Models.Notifications;
using LegacyHealthcareFHIR.Infrastructure.Csv;
using LegacyHealthcareFHIR.Infrastructure.Data;
using LegacyHealthcareFHIR.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace LegacyHealthcareFHIR.Tests.Workers;

public class ResourceTypeDetectionWorkerTests
{
    private static AppDbContext CreateDbContext()
    {
        var options =
            new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

        return new AppDbContext(options);
    }


    [Fact]
    public async Task ExecuteAsync_ValidJob_PublishesDetectionResult()
    {
        // Arrange
        await using var dbContext = CreateDbContext();

        var tempDirectory = Path.Combine(
            Path.GetTempPath(),
            Guid.NewGuid().ToString());

        Directory.CreateDirectory(tempDirectory);

        try
        {
            var storedFileName = "patients.csv";

            var filePath = Path.Combine(
                tempDirectory,
                storedFileName);

            await File.WriteAllTextAsync(
                filePath,
                """
            P_ID,F_NAME,L_NAME,DOB,SEX
            1001,John,Smith,1980-01-01,M
            1002,Mary,Jones,1992-05-14,F
            """);

            var job = new ImportJob
            {
                Id = Guid.NewGuid(),
                HospitalId = 1,
                OriginalFileName = "patients.csv",
                StoredFileName = storedFileName,
                InputFormat = "CSV",
                Status = JobStatus.Pending
            };

            dbContext.ImportJobs.Add(job);
            await dbContext.SaveChangesAsync();

            // Mock AI
            var aiService =
                new Mock<IResourceTypeAiService>();

            aiService
                .Setup(x => x.DetectAsync(
                    It.IsAny<SourceFileData>()))
                .ReturnsAsync(
                    new ResourceTypeDetectionResult
                    {
                        ResourceType = FhirResourceType.Patient,
                        IsApproved = false,
                        AiConfidence = 0.98m
                    });

            // Mock SignalR
            var signalRNotifier =
                new Mock<ISignalRNotifier>();

            JobNotificationEvent? detectionEvent = null;

            signalRNotifier
                .Setup(x => x.SendAsync(
                    It.Is<JobNotificationEvent>(e =>
                        e.EventType ==
                        JobNotificationEventType.AiSuggested)))
                .Callback<JobNotificationEvent>(e =>
                {
                    detectionEvent = e;
                })
                .Returns(Task.CompletedTask);

            // Real services
            var fileStorageService =
                new LocalFileStorageService(
                    tempDirectory);

            var csvReader =
                new LegacyCsvReader();

            var sourceFileService =
                new SourceFileService(
                    fileStorageService,
                    csvReader);

            var resourceTypeDetectionService =
                new ResourceTypeDetectionService(
                    dbContext,
                    aiService.Object);

            var worker =
                new ResourceTypeDetectionWorker(
                    dbContext,
                    signalRNotifier.Object,
                    sourceFileService,
                    resourceTypeDetectionService);

            // Act
            await worker.ExecuteAsync(job.Id);
            // Assert
            var updatedJob =
                await dbContext.ImportJobs.FindAsync(job.Id);

            Assert.NotNull(updatedJob);

            Assert.Equal(
                JobStatus.Preparing,
                updatedJob.Status);

            Assert.NotNull(
                updatedJob.StartedAtUtc);

            Assert.Null(
                updatedJob.ErrorMessage);

            aiService.Verify(
                x => x.DetectAsync(
                    It.IsAny<SourceFileData>()),
                Times.Once);
        }
        finally
        {
            Directory.Delete(
                tempDirectory,
                true);
        }
    }

    [Fact]
    public async Task ExecuteAsync_JobDoesNotExist_ThrowsInvalidOperationException()
    {
        // Arrange
        await using var dbContext = CreateDbContext();

        var tempDirectory = Path.Combine(
            Path.GetTempPath(),
            Guid.NewGuid().ToString());

        Directory.CreateDirectory(tempDirectory);

        try
        {
            var aiService =
                new Mock<IResourceTypeAiService>();

            var signalRNotifier =
                new Mock<ISignalRNotifier>();

            var fileStorageService =
                new LocalFileStorageService(
                    tempDirectory);

            var sourceFileService =
                new SourceFileService(
                    fileStorageService,
                    new LegacyCsvReader());

            var resourceTypeDetectionService =
                new ResourceTypeDetectionService(
                    dbContext,
                    aiService.Object);

            var worker =
                new ResourceTypeDetectionWorker(
                    dbContext,
                    signalRNotifier.Object,
                    sourceFileService,
                    resourceTypeDetectionService);

            var missingJobId =
                Guid.NewGuid();

            // Act + Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => worker.ExecuteAsync(missingJobId));

            aiService.Verify(
                x => x.DetectAsync(
                    It.IsAny<SourceFileData>()),
                Times.Never);

            signalRNotifier.Verify(
                x => x.SendAsync(
                    It.IsAny<JobNotificationEvent>()),
                Times.Never);
        }
        finally
        {
            Directory.Delete(
                tempDirectory,
                true);
        }
    }

    [Fact]
    public async Task ExecuteAsync_SourceFileDoesNotExist_DoesNotPublishDetectionResult()
    {
        // Arrange
        await using var dbContext = CreateDbContext();

        var tempDirectory = Path.Combine(
            Path.GetTempPath(),
            Guid.NewGuid().ToString());

        Directory.CreateDirectory(tempDirectory);

        try
        {
            var job = new ImportJob
            {
                Id = Guid.NewGuid(),
                HospitalId = 1,
                OriginalFileName = "missing.csv",
                StoredFileName = "missing.csv",
                InputFormat = "CSV",
                Status = JobStatus.Pending
            };

            dbContext.ImportJobs.Add(job);
            await dbContext.SaveChangesAsync();

            var aiService =
                new Mock<IResourceTypeAiService>();

            var signalRNotifier =
                new Mock<ISignalRNotifier>();

            var fileStorageService =
                new LocalFileStorageService(
                    tempDirectory);

            var sourceFileService =
                new SourceFileService(
                    fileStorageService,
                    new LegacyCsvReader());

            var resourceTypeDetectionService =
                new ResourceTypeDetectionService(
                    dbContext,
                    aiService.Object);

            var worker =
                new ResourceTypeDetectionWorker(
                    dbContext,
                    signalRNotifier.Object,
                    sourceFileService,
                    resourceTypeDetectionService);

            // Act + Assert
            await Assert.ThrowsAsync<FileNotFoundException>(
                () => worker.ExecuteAsync(job.Id));

            // Starting notification SHOULD have happened.
            signalRNotifier.Verify(
                x => x.SendAsync(
                    It.Is<JobNotificationEvent>(e =>
                        e.EventType ==
                            JobNotificationEventType.ProgressUpdated)),
                Times.Once);

            // Detection result should NOT have been published.
            signalRNotifier.Verify(
                x => x.SendAsync(
                    It.Is<JobNotificationEvent>(e =>
                        e.EventType ==
                            JobNotificationEventType.AiSuggested)),
                Times.Never);

            // AI should never have been reached.
            aiService.Verify(
                x => x.DetectAsync(
                    It.IsAny<SourceFileData>()),
                Times.Never);
        }
        finally
        {
            Directory.Delete(
                tempDirectory,
                true);
        }
    }
}