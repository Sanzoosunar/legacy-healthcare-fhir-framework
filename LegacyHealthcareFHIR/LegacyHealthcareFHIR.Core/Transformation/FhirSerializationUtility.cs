using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using System.Text.Json;

namespace LegacyHealthcareFHIR.Core.Transformation;

public static class FhirSerializationUtility
{
    public static Bundle CreateBundle(IEnumerable<Resource> resources)
    {
        return new Bundle
        {
            Type = Bundle.BundleType.Collection,
            Entry = resources.Select(resource => new Bundle.EntryComponent
            {
                Resource = resource
            }).ToList()
        };
    }

    public static string Serialize(Resource resource)
    {
        var serializer = new FhirJsonSerializer();
        var json = serializer.SerializeToString(resource);

        using var document = JsonDocument.Parse(json);

        return JsonSerializer.Serialize(document, new JsonSerializerOptions
        {
            WriteIndented = true
        });
    }
}