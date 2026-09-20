namespace LegacyHealthcareFHIR.Core.Validation;

public class LegacyDataValidationError
{
    public int RowNumber { get; set; }
    public required string FieldName { get; set; }
    public string? Value { get; set; }
    public required string Error { get; set; }
}