using LegacyHealthcareFHIR.Core.Enums;
using LegacyHealthcareFHIR.Core.Interfaces;
using LegacyHealthcareFHIR.Core.Models;
using LegacyHealthcareFHIR.Core.Models.Import;
using LegacyHealthcareFHIR.Core.Models.Mapping;
using LegacyHealthcareFHIR.Core.Utilities;
using LegacyHealthcareFHIR.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LegacyHealthcareFHIR.Infrastructure.Services;

public class FieldMappingService
{
    private readonly AppDbContext _dbContext;
    private readonly IFieldMappingAiService _aiService;
    public FieldMappingService(
        AppDbContext dbContext,
        IFieldMappingAiService aiService)
    {
        _dbContext = dbContext;
        _aiService = aiService;
    }

    public async Task<FieldMappingResult> GetOrCreateAsync(int hospitalId, FhirResourceType resourceType, SourceFileData sourceFileData)
    {
        var fingerprint = SchemaFingerprintUtility.GenerateFromHeaders(sourceFileData.Headers);

        var configuration = await _dbContext.MappingConfigurations
            .Include(x => x.FieldMappings)
            .FirstOrDefaultAsync(x => x.HospitalId == hospitalId && x.SchemaFingerprint == fingerprint && x.ResourceType == resourceType);

        if (configuration == null)
        {
            configuration = await CreateMappingAsync(hospitalId, resourceType, fingerprint, sourceFileData);
        }

        return CreateResult(configuration);
    }

    private async Task<MappingConfiguration> CreateMappingAsync(int hospitalId, FhirResourceType resourceType, string fingerprint, SourceFileData sourceFileData)
    {
        var aiResult = await _aiService.SuggestMappingsAsync(resourceType, sourceFileData);

        var configuration = new MappingConfiguration
        {
            HospitalId = hospitalId,
            SchemaFingerprint = fingerprint,
            ResourceType = resourceType,
        };

        foreach (var suggestion in aiResult.Mappings)
        {
            configuration.FieldMappings.Add(new FieldMapping
            {
                SourceField = suggestion.SourceField,
                NormalizedField = suggestion.NormalizedField,
                AiConfidence = suggestion.AiConfidence,
                AiExplanation = suggestion.AiExplanation
            });
        }

        _dbContext.MappingConfigurations.Add(configuration);
        await _dbContext.SaveChangesAsync();

        return configuration;
    }

    private static FieldMappingResult CreateResult(MappingConfiguration configuration)
    {
        return new FieldMappingResult
        {
            ConfigurationId = configuration.Id,
            IsApproved = configuration.IsApproved,
            Mappings = configuration.FieldMappings.Select(x => new FieldMappingItemResult
            {
                MappingId = x.Id,
                SourceField = x.SourceField,
                NormalizedField = x.NormalizedField,
                AiConfidence = x.AiConfidence,
                AiExplanation = x.AiExplanation
            }).ToList()
        };
    }
}