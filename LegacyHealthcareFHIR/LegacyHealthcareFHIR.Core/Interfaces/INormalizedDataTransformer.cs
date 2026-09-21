using LegacyHealthcareFHIR.Core.Enums;
using LegacyHealthcareFHIR.Core.Models.Normalized;
using LegacyHealthcareFHIR.Core.Models.Transformed;

namespace LegacyHealthcareFHIR.Core.Interfaces;

public interface INormalizedDataTransformer
{
    List<TransformedData> Transform(FhirResourceType resourceType, List<NormalizedData> normalizedData);
}