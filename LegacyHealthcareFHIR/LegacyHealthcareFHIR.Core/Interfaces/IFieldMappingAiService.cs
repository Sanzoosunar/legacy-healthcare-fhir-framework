using LegacyHealthcareFHIR.Core.Enums;
using LegacyHealthcareFHIR.Core.Models.Import;
using LegacyHealthcareFHIR.Core.Models.Mapping;

namespace LegacyHealthcareFHIR.Core.Interfaces;

public interface IFieldMappingAiService
{
    Task<FieldMappingAIResult> SuggestMappingsAsync(FhirResourceType resourceType, SourceFileData sourceFileData);
}