namespace LegacyHealthcareFHIR.Core.Validation;

public class LegacyDataValidationResult
{
    public bool IsValid => Errors.Count == 0;
    public int TotalValid { get; set; }
    public int TotalInvalid { get; set; }
    public List<LegacyDataValidationError> Errors { get; set; } = new();
}