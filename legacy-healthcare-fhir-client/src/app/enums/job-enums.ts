export enum JobStage {
  Created = 0,
  ResourceTypeDetection = 1,
  FieldMapping = 2,
  DataValidation = 3,
  FhirTransformation = 4,
  FhirValidation = 5,
  Completed = 6
}

export enum JobStatus {
  Started = 0,
  InProgress = 1,
  AiSuggested = 2,
  Completed = 3,
  Failed = 4
}