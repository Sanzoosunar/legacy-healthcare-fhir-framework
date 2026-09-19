using LegacyHealthcareFHIR.Core.Interfaces;
using LegacyHealthcareFHIR.Core.Models.Import;
using LegacyHealthcareFHIR.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace LegacyHealthcareFHIR.Infrastructure.Services;

public class ResourceTypeDetectionService
{
    private readonly AppDbContext _dbContext;
    private readonly IResourceTypeAiService _aiService;

    public ResourceTypeDetectionService(
        AppDbContext dbContext,
        IResourceTypeAiService aiService)
    {
        _dbContext = dbContext;
        _aiService = aiService;
    }
    public string GenerateSchemaFingerprint(
        List<string> headers)
    {
        var normalizedHeaders = headers
            .Select(x => x.Trim().ToLowerInvariant())
            .OrderBy(x => x);

        var schema = string.Join("|", normalizedHeaders);

        var bytes = SHA256.HashData(
            Encoding.UTF8.GetBytes(schema));

        return Convert.ToHexString(bytes);
    }
    public async Task<ResourceTypeDetectionResult> DetectAsync(int hospitalId,
        SourceFileData sourceFileData)
    {
        var fingerprint =
       GenerateSchemaFingerprint(sourceFileData.Headers);

        var existingDetection =
            await _dbContext.ResourceTypeDetections
                .FirstOrDefaultAsync(x =>
                    x.HospitalId == hospitalId &&
                    x.SchemaFingerprint == fingerprint &&
                    x.IsApproved);

        if (existingDetection != null)
        {
            return new ResourceTypeDetectionResult
            {
                ResourceType = existingDetection.ResourceType,
                IsApproved = true,
                AiConfidence = null
            };
        }

        var aiSuggestion = await _aiService.DetectAsync(sourceFileData);
        return aiSuggestion;
    }
}