using LegacyHealthcareFHIR.Core.Enums;
using LegacyHealthcareFHIR.Core.Models;
using LegacyHealthcareFHIR.Core.Models.Import;
using LegacyHealthcareFHIR.Core.Models.Mapping;
using LegacyHealthcareFHIR.Core.Utilities;
using Moq;

namespace LegacyHealthcareFHIR.Tests.Services;

public class FieldMappingServiceTests : TestBase
{
    private readonly int _hospitalId = 1;
    private readonly FhirResourceType _resourceType = FhirResourceType.Patient;
    private readonly SourceFileData _sourceData;
    private readonly MappingConfiguration _config;
    public FieldMappingServiceTests()
    {
        _sourceData = new SourceFileData { Headers = ["P_ID", "F_NAME", "L_NAME", "DOB", "SEX"] };
        _config = new MappingConfiguration
        {
            Id = 1,
            HospitalId = _hospitalId,
            SchemaFingerprint = SchemaFingerprintUtility.GenerateFromHeaders(_sourceData.Headers),
            ResourceType = _resourceType,
        };
    }

    [Fact]
    public async Task GenerateAsync_MappingDoesNotExist_CallsAiAndSavesMapping()
    {
        SetupAiResult();

        var result = await _fieldMappingService.GetOrCreateAsync(_hospitalId, _resourceType, _sourceData);
        var configuration = Assert.Single(_dbcontext.MappingConfigurations);

        Assert.Equivalent(_config, configuration);

        _fieldMappingAiServiceMock.Verify(x => x.SuggestMappingsAsync(FhirResourceType.Patient, It.IsAny<SourceFileData>()), Times.Once);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GenerateAsync_MappingExists_ReturnsExistingMapping(bool isApproved)
    {
        _config.IsApproved = isApproved;
        _config.FieldMappings = [new FieldMapping { SourceField = "P_ID", NormalizedField = "PatientId", AiConfidence = 0.99m }];

        _dbcontext.MappingConfigurations.Add(_config);
        await _dbcontext.SaveChangesAsync();

        var result = await _fieldMappingService.GetOrCreateAsync(_hospitalId, _resourceType, _sourceData);

        Assert.Equal(_config.Id, result.ConfigurationId);
        Assert.Equal(isApproved, result.IsApproved);

        _fieldMappingAiServiceMock.Verify(x => x.SuggestMappingsAsync(It.IsAny<FhirResourceType>(), It.IsAny<SourceFileData>()), Times.Never);
    }


    [Theory]
    [InlineData(1, FhirResourceType.Observation)]
    [InlineData(2, FhirResourceType.Patient)]
    public async Task GetOrCreateAsync_DifferentCacheKey_CreatesNewMapping(int hospitalId, FhirResourceType resourceType)
    {
        _dbcontext.MappingConfigurations.Add(_config);
        await _dbcontext.SaveChangesAsync();

        var existingId = _config.Id;

        SetupAiResult();

        var result = await _fieldMappingService.GetOrCreateAsync(hospitalId, resourceType, _sourceData);

        Assert.NotEqual(existingId, result.ConfigurationId);

        _fieldMappingAiServiceMock.Verify(x => x.SuggestMappingsAsync(resourceType, It.IsAny<SourceFileData>()), Times.Once);
    }

    private void SetupAiResult()
    {
        var aiResult = new FieldMappingAIResult
        {
            Mappings =
            [
                new FieldMappingAISuggestion { SourceField = "P_ID", NormalizedField = "PatientId", AiConfidence = 0.99m },
                new FieldMappingAISuggestion { SourceField = "F_NAME", NormalizedField = "FirstName", AiConfidence = 0.98m }
            ]
        };
        _fieldMappingAiServiceMock.Setup(x => x.SuggestMappingsAsync(It.IsAny<FhirResourceType>(), It.IsAny<SourceFileData>())).ReturnsAsync(aiResult);
    }
}