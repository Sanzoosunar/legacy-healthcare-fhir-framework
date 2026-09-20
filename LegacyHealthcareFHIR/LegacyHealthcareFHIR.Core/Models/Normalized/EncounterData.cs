namespace LegacyHealthcareFHIR.Core.Models.Normalized;

public class EncounterData:NormalizedData
{
    public string? EncounterId { get; set; }
    public string? PatientId { get; set; }
    public string? Status { get; set; }
    public string? Class { get; set; }
    public string? TypeCode { get; set; }
    public string? TypeDisplay { get; set; }
    public string? StartDateTime { get; set; }
    public string? EndDateTime { get; set; }
    public string? PractitionerId { get; set; }
    public string? LocationId { get; set; }
    public string? ReasonCode { get; set; }
    public string? ReasonDisplay { get; set; }
}