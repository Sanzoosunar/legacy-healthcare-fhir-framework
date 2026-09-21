using Hl7.Fhir.Model;

namespace LegacyHealthcareFHIR.Core.Models.Transformed;

public class FhirTransformedData : TransformedData
{
    public required Resource Resource { get; set; }
}