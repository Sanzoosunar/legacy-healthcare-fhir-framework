using LegacyHealthcareFHIR.Core.Interfaces;
using LegacyHealthcareFHIR.Core.Models;
using LegacyHealthcareFHIR.Core.Models.Detection;
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
        var fingerprint = GenerateSchemaFingerprint(sourceFileData.Headers);

        var detection = await _dbContext.ResourceTypeDetections
                                   .FirstOrDefaultAsync(x =>
                                       x.HospitalId == hospitalId &&
                                       x.SchemaFingerprint == fingerprint);

        if (detection == null)
        {
            var aiResult = await _aiService.DetectAsync(sourceFileData);

            detection = new ResourceTypeDetection
            {
                HospitalId = hospitalId,
                SchemaFingerprint = fingerprint,
                ResourceType = aiResult.ResourceType,
                AiConfidence = aiResult.AiConfidence,
                IsApproved = false
            };

            _dbContext.ResourceTypeDetections.Add(detection);

            await _dbContext.SaveChangesAsync();
        }

        return new ResourceTypeDetectionResult
        {
            DetectionId = detection.Id,
            ResourceType = detection.ResourceType,
            AiConfidence = detection.AiConfidence,
            IsApproved = detection.IsApproved
        };
    }
}