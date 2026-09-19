using LegacyHealthcareFHIR.Core.Enums;

namespace LegacyHealthcareFHIR.Core.Models.Import;

public class ApproveResourceTypeRequest
{
    public int DetectionId { get; set; }
    public FhirResourceType ResourceType { get; set; }
}