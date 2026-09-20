namespace LegacyHealthcareFHIR.Core.Models.Normalized;
public class ObservationData: NormalizedData
{
    public string? ObservationId { get; set; }
    public string? PatientId { get; set; }
    public string? EncounterId { get; set; }
    public string? Status { get; set; }
    public string? CategoryCode { get; set; }
    public string? CategoryDisplay { get; set; }
    public string? Code { get; set; }
    public string? CodeDisplay { get; set; }
    public string? CodeSystem { get; set; }
    public string? EffectiveDateTime { get; set; }
    public string? Value { get; set; }
    public string? Unit { get; set; }
    public string? UnitCode { get; set; }
    public string? UnitSystem { get; set; }
    public string? ReferenceRangeLow { get; set; }
    public string? ReferenceRangeHigh { get; set; }
}