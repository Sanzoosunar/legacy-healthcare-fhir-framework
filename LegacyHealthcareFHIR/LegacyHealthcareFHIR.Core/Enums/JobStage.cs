namespace LegacyHealthcareFHIR.Core.Enums;

public enum JobStage
{
    ResourceTypeDetection,
    FieldMapping,
    DataValidation,
    FhirTransformation,
    FhirValidation,
    Completed
}