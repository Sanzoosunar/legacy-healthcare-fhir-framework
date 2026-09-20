using LegacyHealthcareFHIR.Core.Enums;

namespace LegacyHealthcareFHIR.Core.Models.Import;

public class ApproveResourceTypeRequest
{
    public FhirResourceType ResourceType { get; set; }
}