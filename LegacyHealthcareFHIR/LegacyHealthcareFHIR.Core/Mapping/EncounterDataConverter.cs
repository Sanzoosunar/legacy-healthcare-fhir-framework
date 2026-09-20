using LegacyHealthcareFHIR.Core.Models.Legacy;
using LegacyHealthcareFHIR.Core.Models.Normalized;

namespace LegacyHealthcareFHIR.Core.Mapping;

public static class EncounterDataConverter
{
    public static List<EncounterData> Convert(List<LegacyRecord> legacyData, Dictionary<string, string> mappings)
    {
        var encounters = new List<EncounterData>();

        foreach (var record in legacyData)
        {
            encounters.Add(ConvertRecord(record, mappings));
        }

        return encounters;
    }

    public static EncounterData ConvertRecord(LegacyRecord record, Dictionary<string, string> mappings)
    {
        var encounter = new EncounterData();

        foreach (var mapping in mappings)
        {
            record.Fields.TryGetValue(mapping.Key, out var value);

            switch (mapping.Value)
            {
                case nameof(EncounterData.EncounterId):
                    encounter.EncounterId = value;
                    break;

                case nameof(EncounterData.PatientId):
                    encounter.PatientId = value;
                    break;

                case nameof(EncounterData.Status):
                    encounter.Status = value;
                    break;

                case nameof(EncounterData.Class):
                    encounter.Class = value;
                    break;

                case nameof(EncounterData.TypeCode):
                    encounter.TypeCode = value;
                    break;

                case nameof(EncounterData.TypeDisplay):
                    encounter.TypeDisplay = value;
                    break;

                case nameof(EncounterData.StartDateTime):
                    encounter.StartDateTime = value;
                    break;

                case nameof(EncounterData.EndDateTime):
                    encounter.EndDateTime = value;
                    break;

                case nameof(EncounterData.PractitionerId):
                    encounter.PractitionerId = value;
                    break;

                case nameof(EncounterData.LocationId):
                    encounter.LocationId = value;
                    break;

                case nameof(EncounterData.ReasonCode):
                    encounter.ReasonCode = value;
                    break;

                case nameof(EncounterData.ReasonDisplay):
                    encounter.ReasonDisplay = value;
                    break;
            }
        }

        return encounter;
    }
}