using Hl7.Fhir.Model;
using LegacyHealthcareFHIR.Core.Models.Normalized;

namespace LegacyHealthcareFHIR.Core.Transformation;

public static class ObservationFhirTransformer
{
    public static List<Observation> Transform(List<ObservationData> observations)
    {
        return observations.Select(Transform).ToList();
    }

    public static Observation Transform(ObservationData observation)
    {
        return new Observation
        {
            Id = observation.ObservationId,
            Status = ParseStatus(observation.Status),
            Category = CreateCategory(observation),
            Code = CreateCode(observation),
            Subject = CreateReference("Patient", observation.PatientId),
            Encounter = CreateReference("Encounter", observation.EncounterId),
            Effective = CreateEffectiveDateTime(observation.EffectiveDateTime),
            Value = CreateValue(observation),
            ReferenceRange = CreateReferenceRange(observation)
        };
    }

    private static ObservationStatus ParseStatus(string? status)
    {
        return status?.Trim().ToLowerInvariant() switch
        {
            "registered" => ObservationStatus.Registered,
            "preliminary" => ObservationStatus.Preliminary,
            "final" => ObservationStatus.Final,
            "amended" => ObservationStatus.Amended,
            "corrected" => ObservationStatus.Corrected,
            "cancelled" => ObservationStatus.Cancelled,
            "entered-in-error" => ObservationStatus.EnteredInError,
            "unknown" => ObservationStatus.Unknown,
            _ => ObservationStatus.Unknown
        };
    }

    private static List<CodeableConcept> CreateCategory(ObservationData observation)
    {
        if (string.IsNullOrWhiteSpace(observation.CategoryCode))
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
                        Code = observation.CategoryCode,
                        Display = observation.CategoryDisplay
                    }
                ]
            }
        ];
    }

    private static CodeableConcept CreateCode(ObservationData observation)
    {
        return new CodeableConcept
        {
            Coding =
            [
                new Coding
                {
                    System = observation.CodeSystem,
                    Code = observation.Code,
                    Display = observation.CodeDisplay
                }
            ]
        };
    }

    private static ResourceReference? CreateReference(string resourceType, string? id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return null;
        }

        return new ResourceReference($"{resourceType}/{id}");
    }

    private static FhirDateTime? CreateEffectiveDateTime(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return new FhirDateTime(value);
    }

    private static Quantity? CreateValue(ObservationData observation)
    {
        if (string.IsNullOrWhiteSpace(observation.Value))
        {
            return null;
        }

        if (!decimal.TryParse(observation.Value, out var value))
        {
            return null;
        }

        return new Quantity
        {
            Value = value,
            Unit = observation.Unit,
            Code = observation.UnitCode,
            System = observation.UnitSystem
        };
    }

    private static List<Observation.ReferenceRangeComponent> CreateReferenceRange(ObservationData observation)
    {
        var hasLow = decimal.TryParse(observation.ReferenceRangeLow, out var low);
        var hasHigh = decimal.TryParse(observation.ReferenceRangeHigh, out var high);

        if (!hasLow && !hasHigh)
        {
            return [];
        }

        return
        [
            new Observation.ReferenceRangeComponent
            {
                Low = hasLow
                    ? new Quantity
                    {
                        Value = low,
                        Unit = observation.Unit,
                        Code = observation.UnitCode,
                        System = observation.UnitSystem
                    }
                    : null,

                High = hasHigh
                    ? new Quantity
                    {
                        Value = high,
                        Unit = observation.Unit,
                        Code = observation.UnitCode,
                        System = observation.UnitSystem
                    }
                    : null
            }
        ];
    }
}