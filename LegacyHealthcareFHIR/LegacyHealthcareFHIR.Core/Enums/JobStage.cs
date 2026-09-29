namespace LegacyHealthcareFHIR.Core.Enums;

public enum JobStage
{
    Created = 0,
    ResourceTypeDetection = 1,
    FieldMapping = 2,
    DataValidation = 3,
    FhirTransformation = 4,
    Completed = 6
}