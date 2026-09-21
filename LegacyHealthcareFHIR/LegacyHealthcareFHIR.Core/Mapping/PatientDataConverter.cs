using LegacyHealthcareFHIR.Core.Enums;
using LegacyHealthcareFHIR.Core.Models.Legacy;
using LegacyHealthcareFHIR.Core.Models.Normalized;

namespace LegacyHealthcareFHIR.Core.Mapping;

public static class PatientDataConverter
{
    public static List<NormalizedData> Convert(List<LegacyRecord> legacyData, Dictionary<string, string> mappings)
    {
        var patients = new List<NormalizedData>();

        foreach (var record in legacyData)
        {
            patients.Add(ConvertRecord(record, mappings));
        }

        return patients;
    }

    public static PatientData ConvertRecord(LegacyRecord record, Dictionary<string, string> mappings)
    {
        var patient = new PatientData();

        foreach (var mapping in mappings)
        {
            record.Fields.TryGetValue(mapping.Key, out var value);

            switch (mapping.Value)
            {
                case nameof(PatientData.PatientId):
                    patient.PatientId = value;
                    break;

                case nameof(PatientData.FirstName):
                    patient.FirstName = value;
                    break;

                case nameof(PatientData.LastName):
                    patient.LastName = value;
                    break;

                case nameof(PatientData.DateOfBirth):
                    patient.DateOfBirth = value;
                    break;

                case nameof(PatientData.Gender):
                    patient.Gender = ParseGender(value);
                    break;
            }
        }

        return patient;
    }

    private static GenderType? ParseGender(string? gender)
    {
        return gender?.Trim().ToLowerInvariant() switch
        {
            "m" => GenderType.Male,
            "male" => GenderType.Male,
            "f" => GenderType.Female,
            "female" => GenderType.Female,
            "o" => GenderType.Other,
            "other" => GenderType.Other,
            "u" => GenderType.Unknown,
            "unknown" => GenderType.Unknown,
            _ => null
        };
    }
}