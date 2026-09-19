#pragma warning disable OPENAI001

using LegacyHealthcareFHIR.Core.Enums;
using LegacyHealthcareFHIR.Core.Interfaces;
using LegacyHealthcareFHIR.Core.Models.Detection;
using LegacyHealthcareFHIR.Core.Models.Import;
using Microsoft.Extensions.Configuration;
using OpenAI.Responses;
using System.Text;
using System.Text.Json;

namespace LegacyHealthcareFHIR.Infrastructure.AI;

public class ResourceTypeAiService : IResourceTypeAiService
{
    private readonly ResponsesClient _client;
    private readonly string _model;

    public ResourceTypeAiService(IConfiguration configuration)
    {
        var apiKey = configuration["OpenAI:ApiKey"]
            ?? throw new InvalidOperationException(
                "OpenAI API key is not configured.");

        _model = configuration["OpenAI:Model"]
            ?? throw new InvalidOperationException(
                "OpenAI model is not configured.");

        _client = new ResponsesClient(apiKey);
    }

    public async Task<ResourceTypeDetectionAiResult> DetectAsync(
        SourceFileData sourceFileData)
    {
        var prompt = BuildPrompt(sourceFileData);

        var response = await _client.CreateResponseAsync(
            _model,
            prompt);

        var output = response.Value.GetOutputText();

        var result =
            JsonSerializer.Deserialize<AiDetectionResponse>(
                output,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

        if (result == null)
        {
            throw new InvalidOperationException(
                "AI returned an invalid response.");
        }

        if (!Enum.TryParse<FhirResourceType>(
                result.ResourceType,
                true,
                out var resourceType))
        {
            throw new InvalidOperationException(
                $"Unsupported FHIR resource type: {result.ResourceType}");
        }

        return new ResourceTypeDetectionAiResult
        {
            ResourceType = resourceType,
            AiConfidence = result.Confidence
        };
    }

    private static string BuildPrompt(
        SourceFileData sourceFileData)
    {
        var builder = new StringBuilder();

        builder.AppendLine(
            "Analyze this legacy healthcare dataset.");

        builder.AppendLine(
            "Determine which supported FHIR resource type best represents it.");

        builder.AppendLine();
        builder.AppendLine("Supported resource types:");
        builder.AppendLine("- Patient");
        builder.AppendLine("- Encounter");
        builder.AppendLine("- Observation");

        builder.AppendLine();
        builder.AppendLine("Headers:");

        foreach (var header in sourceFileData.Headers)
        {
            builder.AppendLine($"- {header}");
        }

        builder.AppendLine();
        builder.AppendLine("Sample records:");

        foreach (var record in sourceFileData.SampleRecords)
        {
            builder.AppendLine(
                JsonSerializer.Serialize(record.Fields));
        }

        builder.AppendLine();
        builder.AppendLine(
            """
            Return ONLY JSON in this format:
            {
              "resourceType": "Patient",
              "confidence": 0.98
            }

            confidence must be between 0 and 1.
            """);

        return builder.ToString();
    }

    private class AiDetectionResponse
    {
        public string ResourceType { get; set; } = string.Empty;

        public decimal Confidence { get; set; }
    }
}