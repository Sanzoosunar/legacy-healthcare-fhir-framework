using LegacyHealthcareFHIR.Core.Enums;
using LegacyHealthcareFHIR.Core.Interfaces;
using LegacyHealthcareFHIR.Core.Models;
using LegacyHealthcareFHIR.Core.Models.Import;
using LegacyHealthcareFHIR.Infrastructure.Data;
using LegacyHealthcareFHIR.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace LegacyHealthcareFHIR.Tests.Services;

public class ResourceTypeDetectionServiceTests
{
    private static AppDbContext CreateDbContext()
    {
        var options =
            new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

        return new AppDbContext(options);
    }

    private static SourceFileData CreatePatientSourceData()
    {
        return new SourceFileData
        {
            Headers =
            [
                "P_ID",
                "F_NAME",
                "L_NAME",
                "DOB",
                "SEX"
            ]
        };
    }

    [Fact]
    public async Task DetectAsync_ApprovedDetectionExists_ReturnsCachedDetection()
    {
        // Arrange
        await using var dbContext = CreateDbContext();

        var aiService =
            new Mock<IResourceTypeAiService>();

        var service =
            new ResourceTypeDetectionService(
                dbContext,
                aiService.Object);

        var sourceData =
            CreatePatientSourceData();

        var fingerprint =
            service.GenerateSchemaFingerprint(
                sourceData.Headers);

        dbContext.ResourceTypeDetections.Add(
            new ResourceTypeDetection
            {
                HospitalId = 1,
                SchemaFingerprint = fingerprint,
                ResourceType = FhirResourceType.Patient,
                IsApproved = true
            });

        await dbContext.SaveChangesAsync();

        // Act
        var result =
            await service.DetectAsync(
                1,
                sourceData);

        // Assert
        Assert.Equal(
            FhirResourceType.Patient,
            result.ResourceType);

        Assert.True(result.IsApproved);

        Assert.Null(result.AiConfidence);

        aiService.Verify(
            x => x.DetectAsync(
                It.IsAny<SourceFileData>()),
            Times.Never);
    }

    [Fact]
    public async Task DetectAsync_NoApprovedDetection_CallsAi()
    {
        // Arrange
        await using var dbContext = CreateDbContext();

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

        var service =
            new ResourceTypeDetectionService(
                dbContext,
                aiService.Object);

        var sourceData =
            CreatePatientSourceData();

        // Act
        var result =
            await service.DetectAsync(
                1,
                sourceData);

        // Assert
        Assert.Equal(
            FhirResourceType.Patient,
            result.ResourceType);

        Assert.False(result.IsApproved);

        Assert.Equal(
            0.98m,
            result.AiConfidence);

        aiService.Verify(
            x => x.DetectAsync(
                It.IsAny<SourceFileData>()),
            Times.Once);
    }

    [Fact]
    public async Task DetectAsync_UnapprovedDetectionExists_CallsAi()
    {
        // Arrange
        await using var dbContext = CreateDbContext();

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
                    AiConfidence = 0.95m
                });

        var service =
            new ResourceTypeDetectionService(
                dbContext,
                aiService.Object);

        var sourceData =
            CreatePatientSourceData();

        var fingerprint =
            service.GenerateSchemaFingerprint(
                sourceData.Headers);

        dbContext.ResourceTypeDetections.Add(
            new ResourceTypeDetection
            {
                HospitalId = 1,
                SchemaFingerprint = fingerprint,
                ResourceType = FhirResourceType.Patient,
                IsApproved = false
            });

        await dbContext.SaveChangesAsync();

        // Act
        var result =
            await service.DetectAsync(
                1,
                sourceData);

        // Assert
        Assert.False(result.IsApproved);

        Assert.Equal(
            0.95m,
            result.AiConfidence);

        aiService.Verify(
            x => x.DetectAsync(
                It.IsAny<SourceFileData>()),
            Times.Once);
    }

    [Fact]
    public async Task DetectAsync_SameHeadersDifferentOrder_UsesApprovedDetection()
    {
        // Arrange
        await using var dbContext = CreateDbContext();

        var aiService =
            new Mock<IResourceTypeAiService>();

        var service =
            new ResourceTypeDetectionService(
                dbContext,
                aiService.Object);

        var originalSourceData =
            CreatePatientSourceData();

        var reorderedSourceData =
            new SourceFileData
            {
                Headers =
                [
                    "SEX",
                    "DOB",
                    "L_NAME",
                    "P_ID",
                    "F_NAME"
                ]
            };

        var fingerprint =
            service.GenerateSchemaFingerprint(
                originalSourceData.Headers);

        dbContext.ResourceTypeDetections.Add(
            new ResourceTypeDetection
            {
                HospitalId = 1,
                SchemaFingerprint = fingerprint,
                ResourceType = FhirResourceType.Patient,
                IsApproved = true
            });

        await dbContext.SaveChangesAsync();

        // Act
        var result =
            await service.DetectAsync(
                1,
                reorderedSourceData);

        // Assert
        Assert.Equal(
            FhirResourceType.Patient,
            result.ResourceType);

        Assert.True(result.IsApproved);

        aiService.Verify(
            x => x.DetectAsync(
                It.IsAny<SourceFileData>()),
            Times.Never);
    }

    [Fact]
    public async Task DetectAsync_SameSchemaDifferentHospital_DoesNotUseOtherHospitalDetection()
    {
        // Arrange
        await using var dbContext = CreateDbContext();

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
                    AiConfidence = 0.97m
                });

        var service =
            new ResourceTypeDetectionService(
                dbContext,
                aiService.Object);

        var sourceData =
            CreatePatientSourceData();

        var fingerprint =
            service.GenerateSchemaFingerprint(
                sourceData.Headers);

        // Hospital 1 approved this schema.
        dbContext.ResourceTypeDetections.Add(
            new ResourceTypeDetection
            {
                HospitalId = 1,
                SchemaFingerprint = fingerprint,
                ResourceType = FhirResourceType.Patient,
                IsApproved = true
            });

        await dbContext.SaveChangesAsync();

        // Act
        // Same schema, but Hospital 2.
        var result =
            await service.DetectAsync(
                2,
                sourceData);

        // Assert
        Assert.False(result.IsApproved);

        Assert.Equal(
            0.97m,
            result.AiConfidence);

        aiService.Verify(
            x => x.DetectAsync(
                It.IsAny<SourceFileData>()),
            Times.Once);
    }
}