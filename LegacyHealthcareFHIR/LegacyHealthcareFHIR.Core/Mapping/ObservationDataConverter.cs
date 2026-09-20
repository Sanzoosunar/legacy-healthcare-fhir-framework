using LegacyHealthcareFHIR.Core.Models.Legacy;
using LegacyHealthcareFHIR.Core.Models.Normalized;

namespace LegacyHealthcareFHIR.Core.Mapping;

public static class ObservationDataConverter
{
    public static List<ObservationData> Convert(List<LegacyRecord> legacyData, Dictionary<string, string> mappings)
    {
        var observations = new List<ObservationData>();

        foreach (var record in legacyData)
        {
            observations.Add(ConvertRecord(record, mappings));
        }

        return observations;
    }

    public static ObservationData ConvertRecord(LegacyRecord record, Dictionary<string, string> mappings)
    {
        var observation = new ObservationData();

        foreach (var mapping in mappings)
        {
            record.Fields.TryGetValue(mapping.Key, out var value);

            switch (mapping.Value)
            {
                case nameof(ObservationData.ObservationId):
                    observation.ObservationId = value;
                    break;

                case nameof(ObservationData.PatientId):
                    observation.PatientId = value;
                    break;

                case nameof(ObservationData.EncounterId):
                    observation.EncounterId = value;
                    break;

                case nameof(ObservationData.Status):
                    observation.Status = value;
                    break;

                case nameof(ObservationData.CategoryCode):
                    observation.CategoryCode = value;
                    break;

                case nameof(ObservationData.CategoryDisplay):
                    observation.CategoryDisplay = value;
                    break;

                case nameof(ObservationData.Code):
                    observation.Code = value;
                    break;

                case nameof(ObservationData.CodeDisplay):
                    observation.CodeDisplay = value;
                    break;

                case nameof(ObservationData.CodeSystem):
                    observation.CodeSystem = value;
                    break;

                case nameof(ObservationData.EffectiveDateTime):
                    observation.EffectiveDateTime = value;
                    break;

                case nameof(ObservationData.Value):
                    observation.Value = value;
                    break;

                case nameof(ObservationData.Unit):
                    observation.Unit = value;
                    break;

                case nameof(ObservationData.UnitCode):
                    observation.UnitCode = value;
                    break;

                case nameof(ObservationData.UnitSystem):
                    observation.UnitSystem = value;
                    break;

                case nameof(ObservationData.ReferenceRangeLow):
                    observation.ReferenceRangeLow = value;
                    break;

                case nameof(ObservationData.ReferenceRangeHigh):
                    observation.ReferenceRangeHigh = value;
                    break;
            }
        }

        return observation;
    }
}