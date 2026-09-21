using Hl7.Fhir.Model;
using LegacyHealthcareFHIR.Core.Enums;
using LegacyHealthcareFHIR.Core.Models.Normalized;

namespace LegacyHealthcareFHIR.Core.Transformation;

public static class PatientFhirTransformer
{
    public static List<Patient> Transform(List<PatientData> patients)
    {
        return patients.Select(Transform).ToList();
    }
    public static Patient Transform(PatientData patient)
    {
        return new Patient
        {
            Id = patient.PatientId,
            BirthDate = patient.DateOfBirth,
            Gender = ParseGender(patient.Gender),
            Name =
            [
                new HumanName
                {
                    Family = patient.LastName,
                    Given = [patient.FirstName]
                }
            ]
        };
    }
    private static AdministrativeGender ParseGender(GenderType? gender)
    {
        return gender switch
        {
            GenderType.Male => AdministrativeGender.Male,
            GenderType.Female => AdministrativeGender.Female,
            GenderType.Other => AdministrativeGender.Other,
            GenderType.Unknown => AdministrativeGender.Unknown,
            _ => AdministrativeGender.Unknown
        };
    }
}