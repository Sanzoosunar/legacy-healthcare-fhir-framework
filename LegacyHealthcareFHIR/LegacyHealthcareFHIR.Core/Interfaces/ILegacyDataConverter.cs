using LegacyHealthcareFHIR.Core.Enums;
using LegacyHealthcareFHIR.Core.Models.Legacy;
using LegacyHealthcareFHIR.Core.Models.Normalized;

namespace LegacyHealthcareFHIR.Core.Interfaces;

public interface ILegacyDataConverter
{
    List<NormalizedData> Convert(FhirResourceType resourceType, List<LegacyRecord> legacyData, Dictionary<string, string> mappings);
}