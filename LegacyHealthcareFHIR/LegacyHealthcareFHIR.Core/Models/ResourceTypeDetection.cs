using LegacyHealthcareFHIR.Core.Enums;

namespace LegacyHealthcareFHIR.Core.Models;

public class ResourceTypeDetection
{
    public int Id { get; set; }

    public int HospitalId { get; set; }

    public string SchemaFingerprint { get; set; } = null!;

    public FhirResourceType ResourceType { get; set; }
    public decimal AiConfidence { get; set; }

    public bool IsApproved { get; set; }
}