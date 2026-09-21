import { Injectable } from '@angular/core';
import { JobStage, JobStatus } from '../enums/job-enums';
import { FhirResourceType } from '../enums/resource-type';
import { ResourceTypeDetectionResult } from '../models/resource-type-detection-result';
import { FieldMappingResult } from '../models/field-mapping-result';
import { NormalizedFieldsResponse } from '../models/job';


@Injectable()
export class JobBridgeService {
  private jobId?: string;
  private jobStage: JobStage = JobStage.Created;
  private jobStatus: JobStatus = JobStatus.InProgress;
  private selectedResourceType?: FhirResourceType = FhirResourceType.Patient;

  public standard = 'FHIR'


  public setJobId(jobId: string): void {
    this.jobId = jobId;
  }

  public getJobId(): string | undefined {
    return this.jobId;
  }

  public setJobStage(jobStage: JobStage): void {
    this.jobStage = jobStage;
  }

  public getJobStage(): JobStage {
    return this.jobStage;
  }

  public setJobStatus(jobStatus: JobStatus): void {
    this.jobStatus = jobStatus;
  }

  public getJobStatus(): JobStatus {
    return this.jobStatus;
  }

  public setSelectedResourceType(resourceType: FhirResourceType) {
    this.selectedResourceType = resourceType;
  }

  public getSelectedResourceType(): FhirResourceType | undefined {
    return this.selectedResourceType;
  }

  private resourceTypeDetection?: ResourceTypeDetectionResult;

  public setResourceTypeDetection(result: ResourceTypeDetectionResult): void {
    this.resourceTypeDetection = result;
    this.resourceTypeDetection.resourceTypeText = FhirResourceType[this.resourceTypeDetection.resourceType]
  }

  public getResourceTypeDetection(): ResourceTypeDetectionResult | undefined {
    return this.resourceTypeDetection;
  }

  private fieldMapping?: FieldMappingResult;

  private fieldMapping1?: FieldMappingResult = {
    configurationId: 1,
    isApproved: false,
    mappings: [
      {
        mappingId: 1,
        sourceField: 'patient_id',
        normalizedField: 'PatientId',
        aiConfidence: 0.9812,
        aiExplanation: 'Matched patient identifier field'
      },
      {
        mappingId: 2,
        sourceField: 'first_name',
        normalizedField: 'FirstName',
        aiConfidence: 0.9541,
        aiExplanation: 'Matched patient first name'
      },
      {
        mappingId: 3,
        sourceField: 'dob',
        normalizedField: 'DateOfBirth',
        aiConfidence: 0.9012,
        aiExplanation: 'Matched patient date of birth'
      }
    ]
  };

  public setFieldMapping(result: FieldMappingResult): void {
    this.fieldMapping = result;
  }

  public getFieldMapping(): FieldMappingResult | undefined {
    return this.fieldMapping;
  }


  private normalizedFields?: NormalizedFieldsResponse;

  public setNormalizedFields(normalizedFields: NormalizedFieldsResponse): void {
    this.normalizedFields = normalizedFields;
  }

  public getNormalizedFields(): NormalizedFieldsResponse | undefined {
    return this.normalizedFields;
  }

  private errorMessages: string[] = [];

  public setErrorMessages(errorMessages: string[]): void {
    this.errorMessages = errorMessages;
  }

  public getErrorMessages(): string[] {
    return this.errorMessages;
  }

  public clearErrorMessages(): void {
    this.errorMessages = [];
  }
}