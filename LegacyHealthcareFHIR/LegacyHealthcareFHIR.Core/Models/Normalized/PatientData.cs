using LegacyHealthcareFHIR.Core.Enums;

namespace LegacyHealthcareFHIR.Core.Models.Normalized;

public class PatientData: NormalizedData
{
    public string? PatientId { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? DateOfBirth { get; set; }

    public GenderType? Gender { get; set; }
}