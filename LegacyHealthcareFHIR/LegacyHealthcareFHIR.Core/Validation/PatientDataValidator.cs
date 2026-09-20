using LegacyHealthcareFHIR.Core.Models.Normalized;

namespace LegacyHealthcareFHIR.Core.Validation;

public static class PatientDataValidator
{
    public static LegacyDataValidationResult Validate(List<PatientData> patients)
    {
        var result = new LegacyDataValidationResult();

        for (var i = 0; i < patients.Count; i++)
        {
            var patient = patients[i];
            var rowNumber = i + 2;
            var rowIsValid = true;

            if (string.IsNullOrWhiteSpace(patient.PatientId))
            {
                AddError(result, rowNumber, nameof(PatientData.PatientId), patient.PatientId, "Patient ID is required");
                rowIsValid = false;
            }

            if (string.IsNullOrWhiteSpace(patient.FirstName))
            {
                AddError(result, rowNumber, nameof(PatientData.FirstName), patient.FirstName, "First name is required");
                rowIsValid = false;
            }

            if (string.IsNullOrWhiteSpace(patient.LastName))
            {
                AddError(result, rowNumber, nameof(PatientData.LastName), patient.LastName, "Last name is required");
                rowIsValid = false;
            }

            if (string.IsNullOrWhiteSpace(patient.DateOfBirth))
            {
                AddError(result, rowNumber, nameof(PatientData.DateOfBirth), patient.DateOfBirth, "Date of birth is required");
                rowIsValid = false;
            }
            else if (!DateTime.TryParse(patient.DateOfBirth, out var dateOfBirth))
            {
                AddError(result, rowNumber, nameof(PatientData.DateOfBirth), patient.DateOfBirth, "Invalid date of birth");
                rowIsValid = false;
            }
            else if (dateOfBirth.Date > DateTime.UtcNow.Date)
            {
                AddError(result, rowNumber, nameof(PatientData.DateOfBirth), patient.DateOfBirth, "Date of birth cannot be in the future");
                rowIsValid = false;
            }

            if (string.IsNullOrWhiteSpace(patient.Gender))
            {
                AddError(result, rowNumber, nameof(PatientData.Gender), patient.Gender, "Gender is required");
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