using LegacyHealthcareFHIR.Core.Enums;
using LegacyHealthcareFHIR.Core.Interfaces;
using LegacyHealthcareFHIR.Core.Models;
using LegacyHealthcareFHIR.Core.Models.Detection;
using LegacyHealthcareFHIR.Core.Models.Import;
using LegacyHealthcareFHIR.Core.Utilities;
using LegacyHealthcareFHIR.Infrastructure.Data;
using LegacyHealthcareFHIR.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace LegacyHealthcareFHIR.Tests.Services;

public class ResourceTypeDetectionServiceTests : IAsyncLifetime
{
    private readonly AppDbContext _dbContext;
    private readonly Mock<IResourceTypeAiService> _aiService;
    private readonly ResourceTypeDetectionService _service;
    private readonly SourceFileData _sourceData;
    private readonly ResourceTypeDetection _detection;

    public ResourceTypeDetectionServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _dbContext = new AppDbContext(options);
        _aiService = new Mock<IResourceTypeAiService>();
        _service = new ResourceTypeDetectionService(_dbContext, _aiService.Object);

        _sourceData = new SourceFileData
        {
            Headers = ["P_ID", "F_NAME", "L_NAME", "DOB", "SEX"]
        };

        _detection = new ResourceTypeDetection
        {
            HospitalId = 1,
            ResourceType = FhirResourceType.Patient,
            AiConfidence = 0.95m,
            IsApproved = false
        };
    }

    private async Task SaveDetectionAsync(ResourceTypeDetection detection)
    {
        detection.SchemaFingerprint = SchemaFingerprintUtility.GenerateFromHeaders(_sourceData.Headers);

        _dbContext.ResourceTypeDetections.Add(detection);
        await _dbContext.SaveChangesAsync();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task DetectAsync_DetectionExists_ReturnsCachedDetection(bool isApproved)
    {
        // Arrange
        _detection.IsApproved = isApproved;

        await SaveDetectionAsync(_detection);

        // Act
        var result = await _service.DetectAsync(1, _sourceData);

        // Assert
        Assert.Equal(_detection.Id, result.DetectionId);
        Assert.Equal(_detection.ResourceType, result.ResourceType);
        Assert.Equal(_detection.IsApproved, result.IsApproved);
        Assert.Equal(_detection.AiConfidence, result.AiConfidence);

        _aiService.Verify(x => x.DetectAsync(It.IsAny<SourceFileData>()), Times.Never);
    }

    [Fact]
    public async Task DetectAsync_NoDetectionExists_CallsAiAndSavesDetection()
    {
        // Arrange
        _aiService
            .Setup(x => x.DetectAsync(It.IsAny<SourceFileData>()))
            .ReturnsAsync(new ResourceTypeDetectionAiResult
            {
                ResourceType = FhirResourceType.Patient,
                AiConfidence = 0.98m
            });

        // Act
        var result = await _service.DetectAsync(1, _sourceData);

        // Assert
        Assert.Equal(FhirResourceType.Patient, result.ResourceType);
        Assert.Equal(0.98m, result.AiConfidence);
        Assert.False(result.IsApproved);
        Assert.True(result.DetectionId > 0);

        _aiService.Verify(x => x.DetectAsync(It.IsAny<SourceFileData>()), Times.Once);

        var savedDetection = await _dbContext.ResourceTypeDetections.SingleAsync();

        Assert.Equal(result.DetectionId, savedDetection.Id);
        Assert.Equal(1, savedDetection.HospitalId);
        Assert.Equal(FhirResourceType.Patient, savedDetection.ResourceType);
        Assert.Equal(0.98m, savedDetection.AiConfidence);
        Assert.False(savedDetection.IsApproved);
    }

    [Fact]
    public async Task DetectAsync_SameHeadersDifferentOrder_UsesExistingDetection()
    {
        // Arrange
        await SaveDetectionAsync(_detection);

        var reorderedSourceData = new SourceFileData
        {
            Headers = ["SEX", "DOB", "L_NAME", "P_ID", "F_NAME"]
        };

        // Act
        var result = await _service.DetectAsync(1, reorderedSourceData);

        // Assert
        Assert.Equal(_detection.Id, result.DetectionId);
        Assert.Equal(_detection.ResourceType, result.ResourceType);
        Assert.Equal(_detection.IsApproved, result.IsApproved);

        _aiService.Verify(x => x.DetectAsync(It.IsAny<SourceFileData>()), Times.Never);
    }

    [Fact]
    public async Task DetectAsync_SameSchemaDifferentHospital_DoesNotUseOtherHospitalDetection()
    {
        // Arrange
        _detection.IsApproved = true;

        await SaveDetectionAsync(_detection);

        _aiService
            .Setup(x => x.DetectAsync(It.IsAny<SourceFileData>()))
            .ReturnsAsync(new ResourceTypeDetectionAiResult
            {
                ResourceType = FhirResourceType.Patient,
                AiConfidence = 0.97m
            });

        // Act
        var result = await _service.DetectAsync(2, _sourceData);

        // Assert
        Assert.Equal(FhirResourceType.Patient, result.ResourceType);
        Assert.Equal(0.97m, result.AiConfidence);
        Assert.False(result.IsApproved);

        _aiService.Verify(x => x.DetectAsync(It.IsAny<SourceFileData>()), Times.Once);

        var hospital2Detection = await _dbContext.ResourceTypeDetections.SingleAsync(x => x.HospitalId == 2);

        Assert.Equal(result.DetectionId, hospital2Detection.Id);
        Assert.Equal(2, hospital2Detection.HospitalId);
        Assert.False(hospital2Detection.IsApproved);
    }

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        await _dbContext.DisposeAsync();
    }
}