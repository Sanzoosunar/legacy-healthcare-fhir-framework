using LegacyHealthcareFHIR.Core.Enums;

namespace LegacyHealthcareFHIR.Core.Models.Detection;

public class ResourceTypeDetectionResult
{
    public int DetectionId { get; set; }
    public FhirResourceType ResourceType { get; set; }
    public bool IsApproved { get; set; }
    public decimal AiConfidence { get; set; }
}