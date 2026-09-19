using LegacyHealthcareFHIR.Core.Enums;

namespace LegacyHealthcareFHIR.Core.Models.Mapping;

public class MappingError
{
    public MappingErrorType ErrorType { get; set; }

    public string? SourceField { get; set; }

    public string? TargetField { get; set; }
}