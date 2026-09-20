namespace LegacyHealthcareFHIR.Core.Models.Mapping;

public class FieldMappingAIResult
{
    public List<FieldMappingAISuggestion> Mappings { get; set; } = new();
}

public class FieldMappingAISuggestion
{
    public required string SourceField { get; set; }
    public required string NormalizedField { get; set; }
    public decimal AiConfidence { get; set; }
    public string? AiExplanation { get; set; }
}