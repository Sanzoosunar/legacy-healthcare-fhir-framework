using LegacyHealthcareFHIR.Core.Enums;
using LegacyHealthcareFHIR.Core.Models;

public class MappingConfiguration
{
    public int Id { get; set; }
    public int HospitalId { get; set; }
    public string SchemaFingerprint { get; set; } = null!;
    public FhirResourceType ResourceType { get; set; }
    public bool IsApproved { get; set; }
    public List<FieldMapping> FieldMappings { get; set; } = new();
}