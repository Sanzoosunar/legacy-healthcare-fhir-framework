using LegacyHealthcareFHIR.Core.Models.Normalized;

namespace LegacyHealthcareFHIR.Core.Validation;

public static class EncounterDataValidator
{
    public static LegacyDataValidationResult Validate(List<EncounterData> encounters)
    {
        var result = new LegacyDataValidationResult();

        for (var i = 0; i < encounters.Count; i++)
        {
            var encounter = encounters[i];
            var rowNumber = i + 2;
            var rowIsValid = true;

            if (string.IsNullOrWhiteSpace(encounter.EncounterId))
            {
                AddError(result, rowNumber, nameof(EncounterData.EncounterId), encounter.EncounterId, "Encounter ID is required");
                rowIsValid = false;
            }

            if (string.IsNullOrWhiteSpace(encounter.PatientId))
            {
                AddError(result, rowNumber, nameof(EncounterData.PatientId), encounter.PatientId, "Patient ID is required");
                rowIsValid = false;
            }

            if (string.IsNullOrWhiteSpace(encounter.Status))
            {
                AddError(result, rowNumber, nameof(EncounterData.Status), encounter.Status, "Status is required");
                rowIsValid = false;
            }

            if (string.IsNullOrWhiteSpace(encounter.Class))
            {
                AddError(result, rowNumber, nameof(EncounterData.Class), encounter.Class, "Class is required");
                rowIsValid = false;
            }

            if (!string.IsNullOrWhiteSpace(encounter.StartDateTime) && !DateTime.TryParse(encounter.StartDateTime, out _))
            {
                AddError(result, rowNumber, nameof(EncounterData.StartDateTime), encounter.StartDateTime, "Invalid start date and time");
                rowIsValid = false;
            }

            if (!string.IsNullOrWhiteSpace(encounter.EndDateTime) && !DateTime.TryParse(encounter.EndDateTime, out _))
            {
                AddError(result, rowNumber, nameof(EncounterData.EndDateTime), encounter.EndDateTime, "Invalid end date and time");
                rowIsValid = false;
            }

            if (rowIsValid) result.TotalValid++;
            else result.TotalInvalid++;
        }

        return result;
    }

    private static void AddError(LegacyDataValidationResult result, int rowNumber, string fieldName, string? value, string error)
    {
        result.Errors.Add(new LegacyDataValidationError
        {
            RowNumber = rowNumber,
            FieldName = fieldName,
            Value = value,
            Error = error
        });
    }
}