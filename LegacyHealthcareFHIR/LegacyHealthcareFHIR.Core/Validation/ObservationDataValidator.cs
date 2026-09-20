using LegacyHealthcareFHIR.Core.Models.Normalized;

namespace LegacyHealthcareFHIR.Core.Validation;

public static class ObservationDataValidator
{
    public static LegacyDataValidationResult Validate(List<ObservationData> observations)
    {
        var result = new LegacyDataValidationResult();

        for (var i = 0; i < observations.Count; i++)
        {
            var observation = observations[i];
            var rowNumber = i + 2;
            var rowIsValid = true;

            if (string.IsNullOrWhiteSpace(observation.ObservationId))
            {
                AddError(result, rowNumber, nameof(ObservationData.ObservationId), observation.ObservationId, "Observation ID is required");
                rowIsValid = false;
            }

            if (string.IsNullOrWhiteSpace(observation.PatientId))
            {
                AddError(result, rowNumber, nameof(ObservationData.PatientId), observation.PatientId, "Patient ID is required");
                rowIsValid = false;
            }

            if (string.IsNullOrWhiteSpace(observation.Status))
            {
                AddError(result, rowNumber, nameof(ObservationData.Status), observation.Status, "Status is required");
                rowIsValid = false;
            }

            if (string.IsNullOrWhiteSpace(observation.Code))
            {
                AddError(result, rowNumber, nameof(ObservationData.Code), observation.Code, "Observation code is required");
                rowIsValid = false;
            }

            if (!string.IsNullOrWhiteSpace(observation.EffectiveDateTime) && !DateTime.TryParse(observation.EffectiveDateTime, out _))
            {
                AddError(result, rowNumber, nameof(ObservationData.EffectiveDateTime), observation.EffectiveDateTime, "Invalid effective date and time");
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