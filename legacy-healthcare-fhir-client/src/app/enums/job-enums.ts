import { LabelStyleMap } from "./job-type";

export enum JobStage {
  Created = 0,
  ResourceTypeDetection = 1,
  FieldMapping = 2,
  DataValidation = 3,
  FhirTransformation = 4,
  Completed = 6
}

export enum JobStatus {
  Started = 0,
  InProgress = 1,
  AiSuggested = 2,
  Completed = 3,
  Failed = 4
}

export const JobStagesStyleMap: LabelStyleMap = {
  [JobStage.Created]: { label: 'Created', className: 'status-progress' },
  [JobStage.ResourceTypeDetection]: { label: 'Resource Type Detection', className: 'status-progress' },
  [JobStage.FieldMapping]: { label: 'Field Mapping', className: 'status-progress' },
  [JobStage.DataValidation]: { label: 'Data Validation', className: 'status-progress' },
  [JobStage.FhirTransformation]: { label: 'FHIR Transformation', className: 'status-progress' },
  [JobStage.Completed]: { label: 'Completed', className: 'status-completed' }
};

export const JobStatusStyleMap: LabelStyleMap = {
  [JobStatus.Started]: { label: 'Started', className: 'status-progress' },
  [JobStatus.InProgress]: { label: 'In Progress', className: 'status-progress' },
  [JobStatus.AiSuggested]: { label: 'AI Suggested', className: 'status-review' },
  [JobStatus.Completed]: { label: 'Completed', className: 'status-completed' },
  [JobStatus.Failed]: { label: 'Failed', className: 'status-failed' }
};