namespace LegacyHealthcareFHIR.Core.Models.Mapping;

public class FieldMappingResult
{
    public int ConfigurationId { get; set; }
    public bool IsApproved { get; set; }
    public List<FieldMappingItemResult> Mappings { get; set; } = new();
}

public class FieldMappingItemResult
{
    public int MappingId { get; set; }
    public required string SourceField { get; set; }
    public required string NormalizedField { get; set; }
    public decimal? AiConfidence { get; set; }
    public string? AiExplanation { get; set; }
}