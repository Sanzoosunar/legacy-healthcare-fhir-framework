
using LegacyHealthcareFHIR.Core.Enums;

namespace LegacyHealthcareFHIR.Core.Models;

public class FieldMapping
{
    public int Id { get; set; }

    public int ConfigId { get; set; }

    public required string SourceField { get; set; }

    public required string NormalizedField { get; set; }

    public decimal? AiConfidence { get; set; }

    public string? AiExplanation { get; set; }

    public MappingStatus Status { get; set; } = MappingStatus.Draft;

}
