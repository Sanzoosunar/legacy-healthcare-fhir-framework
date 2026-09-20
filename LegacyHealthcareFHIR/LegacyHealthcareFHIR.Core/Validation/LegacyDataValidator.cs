using LegacyHealthcareFHIR.Core.Enums;
using LegacyHealthcareFHIR.Core.Interfaces;
using LegacyHealthcareFHIR.Core.Models.Normalized;
using LegacyHealthcareFHIR.Core.Validation;

public class LegacyDataValidator : ILegacyDataValidator
{
    public LegacyDataValidationResult Validate(FhirResourceType resourceType, List<NormalizedData> normalizedData)
    {
        return resourceType switch
        {
            FhirResourceType.Patient => PatientDataValidator.Validate(normalizedData.Cast<PatientData>().ToList()),
            FhirResourceType.Encounter => EncounterDataValidator.Validate(normalizedData.Cast<EncounterData>().ToList()),
            FhirResourceType.Observation => ObservationDataValidator.Validate(normalizedData.Cast<ObservationData>().ToList()),
            _ => throw new Exception("unsupported resource type")
        };
    }
}