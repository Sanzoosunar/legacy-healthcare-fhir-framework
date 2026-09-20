namespace LegacyHealthcareFHIR.Core.Enums;

public enum JobStage
{
    Created,
    ResourceTypeDetection,
    FieldMapping,
    DataValidation,
    FhirTransformation,
    FhirValidation,
    Completed
}