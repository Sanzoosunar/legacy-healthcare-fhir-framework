using LegacyHealthcareFHIR.Core.Enums;
using LegacyHealthcareFHIR.Core.Models.Normalized;
using LegacyHealthcareFHIR.Core.Validation;

namespace LegacyHealthcareFHIR.Core.Interfaces;
public interface ILegacyDataValidator
{
    LegacyDataValidationResult Validate(FhirResourceType resourceType, List<NormalizedData> normalizedData);
}
