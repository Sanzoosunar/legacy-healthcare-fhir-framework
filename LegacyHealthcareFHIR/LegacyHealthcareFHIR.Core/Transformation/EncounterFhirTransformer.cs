using Hl7.Fhir.Model;
using LegacyHealthcareFHIR.Core.Models.Normalized;

namespace LegacyHealthcareFHIR.Core.Transformation;

public static class EncounterFhirTransformer
{
    public static List<Encounter> Transform(List<EncounterData> encounters)
    {
        return encounters.Select(Transform).ToList();
    }

    public static Encounter Transform(EncounterData encounter)
    {
        return new Encounter
        {
            Id = encounter.EncounterId,
            Status = ParseStatus(encounter.Status),
            Class = CreateClass(encounter.Class),
            Type = CreateType(encounter),
            Subject = CreateReference("Patient", encounter.PatientId),
            Period = CreatePeriod(encounter),
            Participant = CreateParticipant(encounter.PractitionerId),
            Location = CreateLocation(encounter.LocationId),
            ReasonCode = CreateReasonCode(encounter)
        };
    }

    private static Encounter.EncounterStatus ParseStatus(string? status)
    {
        return status?.Trim().ToLowerInvariant() switch
        {
            "planned" => Encounter.EncounterStatus.Planned,
            "arrived" => Encounter.EncounterStatus.Arrived,
            "triaged" => Encounter.EncounterStatus.Triaged,
            "in-progress" => Encounter.EncounterStatus.InProgress,
            "onleave" => Encounter.EncounterStatus.Onleave,
            "finished" => Encounter.EncounterStatus.Finished,
            "cancelled" => Encounter.EncounterStatus.Cancelled,
            "entered-in-error" => Encounter.EncounterStatus.EnteredInError,
            "unknown" => Encounter.EncounterStatus.Unknown,
            _ => Encounter.EncounterStatus.Unknown
        };
    }

    private static Coding CreateClass(string? encounterClass)
    {
        return new Coding
        {
            System = "http://terminology.hl7.org/CodeSystem/v3-ActCode",
            Code = encounterClass
        };
    }

    private static List<CodeableConcept> CreateType(EncounterData encounter)
    {
        if (string.IsNullOrWhiteSpace(encounter.TypeCode))
        {
            return [];
        }

        return
        [
            new CodeableConcept
            {
                Coding =
                [
                    new Coding
                    {
                        Code = encounter.TypeCode,
                        Display = encounter.TypeDisplay
                    }
                ]
            }
        ];
    }

    private static ResourceReference? CreateReference(string resourceType, string? id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return null;
        }

        return new ResourceReference($"{resourceType}/{id}");
    }

    private static Period? CreatePeriod(EncounterData encounter)
    {
        if (string.IsNullOrWhiteSpace(encounter.StartDateTime) &&
            string.IsNullOrWhiteSpace(encounter.EndDateTime))
        {
            return null;
        }

        return new Period
        {
            Start = encounter.StartDateTime,
            End = encounter.EndDateTime
        };
    }

    private static List<Encounter.ParticipantComponent> CreateParticipant(string? practitionerId)
    {
        if (string.IsNullOrWhiteSpace(practitionerId))
        {
            return [];
        }

        return
        [
            new Encounter.ParticipantComponent
            {
                Individual = new ResourceReference($"Practitioner/{practitionerId}")
            }
        ];
    }

    private static List<Encounter.LocationComponent> CreateLocation(string? locationId)
    {
        if (string.IsNullOrWhiteSpace(locationId))
        {
            return [];
        }

        return
        [
            new Encounter.LocationComponent
            {
                Location = new ResourceReference($"Location/{locationId}")
            }
        ];
    }

    private static List<CodeableConcept> CreateReasonCode(EncounterData encounter)
    {
        if (string.IsNullOrWhiteSpace(encounter.ReasonCode))
        {
            return [];
        }

        return
        [
            new CodeableConcept
            {
                Coding =
                [
                    new Coding
                    {
                        Code = encounter.ReasonCode,
                        Display = encounter.ReasonDisplay
                    }
                ]
            }
        ];
    }
}