using LegacyHealthcareFHIR.Core.Enums;
using LegacyHealthcareFHIR.Core.Interfaces;
using LegacyHealthcareFHIR.Core.Models.Legacy;
using LegacyHealthcareFHIR.Core.Models.Normalized;

namespace LegacyHealthcareFHIR.Core.Mapping;

public class LegacyDataConverter : ILegacyDataConverter
{
    public List<NormalizedData> Convert(FhirResourceType resourceType, List<LegacyRecord> legacyData, Dictionary<string, string> mappings)
    {
        return resourceType switch
        {
            FhirResourceType.Patient => PatientDataConverter.Convert(legacyData, mappings).Cast<NormalizedData>().ToList(),
            FhirResourceType.Encounter => EncounterDataConverter.Convert(legacyData, mappings).Cast<NormalizedData>().ToList(),
            FhirResourceType.Observation => ObservationDataConverter.Convert(legacyData, mappings).Cast<NormalizedData>().ToList(),
            _ => throw new Exception("unsupported resource type")
        };
    }
}