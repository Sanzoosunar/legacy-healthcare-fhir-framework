using LegacyHealthcareFHIR.Core.Enums;

public class ResourceTypeDetectionResult
{
    public FhirResourceType ResourceType { get; set; }

    public bool IsApproved { get; set; }

    public decimal? AiConfidence { get; set; }
}