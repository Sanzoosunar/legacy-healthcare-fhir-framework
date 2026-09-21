using LegacyHealthcareFHIR.Core.Enums;
using LegacyHealthcareFHIR.Core.Interfaces;
using LegacyHealthcareFHIR.Core.Models.Normalized;
using LegacyHealthcareFHIR.Core.Models.Transformed;

namespace LegacyHealthcareFHIR.Core.Transformation;

public class FhirDataTransformer : INormalizedDataTransformer
{
    public List<TransformedData> Transform(FhirResourceType resourceType, List<NormalizedData> normalizedData)
    {
        return resourceType switch
        {
            FhirResourceType.Patient => PatientFhirTransformer.Transform(normalizedData.Cast<PatientData>().ToList())
                .Select(x => new FhirTransformedData { Resource = x })
                .Cast<TransformedData>()
                .ToList(),

            _ => throw new Exception("unsupported resource type")
        };
    }
}