using LegacyHealthcareFHIR.Core.Enums;
using LegacyHealthcareFHIR.Core.Interfaces;
using LegacyHealthcareFHIR.Core.Models.Import;
using LegacyHealthcareFHIR.Core.Models.Mapping;
using LegacyHealthcareFHIR.Core.Models.Normalized;
using System.Text;
using System.Text.Json;

namespace LegacyHealthcareFHIR.Infrastructure.AI;

public class FieldMappingAiService : IFieldMappingAiService
{
    private readonly IAiService _aiService;

    public FieldMappingAiService(IAiService aiService)
    {
        _aiService = aiService;
    }

    public async Task<FieldMappingAIResult> SuggestMappingsAsync(FhirResourceType resourceType, SourceFileData sourceFileData)
    {
        var prompt = BuildPrompt(resourceType, sourceFileData);
        var output = await _aiService.GetResponseAsync(prompt);

        var result = JsonSerializer.Deserialize<FieldMappingAIResult>(
            output,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

        if (result == null)
        {
            throw new InvalidOperationException("AI returned an invalid field mapping response.");
        }

        return result;
    }

    private static string BuildPrompt(FhirResourceType resourceType, SourceFileData sourceFileData)
    {
        var builder = new StringBuilder();

        builder.AppendLine("Analyze this legacy healthcare dataset.");
        builder.AppendLine($"The approved resource type is {resourceType}.");
        builder.AppendLine("Map each source field to the most appropriate normalized field.");

        builder.AppendLine();
        builder.AppendLine("Allowed normalized fields:");

        foreach (var field in GetNormalizedFields(resourceType))
        {
            builder.AppendLine($"- {field}");
        }

        builder.AppendLine();
        builder.AppendLine("Source headers:");

        foreach (var header in sourceFileData.Headers)
        {
            builder.AppendLine($"- {header}");
        }

        builder.AppendLine();
        builder.AppendLine("Sample records:");

        foreach (var record in sourceFileData.SampleRecords)
        {
            builder.AppendLine(JsonSerializer.Serialize(record.Fields));
        }

        builder.AppendLine();
        builder.AppendLine(
            """
            Return ONLY JSON in this format:
            {
              "mappings": [
                {
                  "sourceField": "P_ID",
                  "normalizedField": "PatientId",
                  "aiConfidence": 0.99,
                  "aiExplanation": "P_ID appears to represent the patient identifier."
                }
              ]
            }

            confidence must be between 0 and 1.
            Use only the allowed normalized fields.
            Return one mapping for every source header.
            """);

        return builder.ToString();
    }

    private static List<string> GetNormalizedFields(FhirResourceType resourceType)
    {
        return resourceType switch
        {
            FhirResourceType.Patient =>
       [
           nameof(PatientData.PatientId),
            nameof(PatientData.FirstName),
            nameof(PatientData.LastName),
            nameof(PatientData.DateOfBirth),
            nameof(PatientData.Gender)
       ],
            _ => throw new InvalidOperationException($"Field mapping is not supported for resource type '{resourceType}'.")
        };
    }
}