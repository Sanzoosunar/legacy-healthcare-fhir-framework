import { Component } from '@angular/core';
import { JobStage, JobStatus } from '../enums/job-enums';
import { JobBridgeService } from '../services/job-bridge.service';
import { NgTemplateOutlet } from '@angular/common';
import { JobDataService } from '../services/job-data.service';
import { FhirResourceType } from '../enums/resource-type';
import { SignalRConnectionService } from '../services/signal-r-connection.service';
import { JobNotificationEvent } from '../models/job';
import { ResourceTypeDetectionResult } from '../models/resource-type-detection-result';
import { FieldMappingResult } from '../models/field-mapping-result';

@Component({
  selector: 'app-job-progress',
  imports: [NgTemplateOutlet],
  templateUrl: './job-progress.component.html',
  styleUrl: './job-progress.component.css'
})
export class JobProgressComponent {
  public stages = [
    { stage: JobStage.ResourceTypeDetection, name: 'Resource Type Detection' },
    { stage: JobStage.FieldMapping, name: 'Field Mapping' },
    { stage: JobStage.DataValidation, name: 'Data Validation' },
    { stage: JobStage.FhirTransformation, name: 'FHIR Transformation' },
    { stage: JobStage.Completed, name: 'Completed' }
  ];

  constructor(public jobBridgeService: JobBridgeService, private jobDataService: JobDataService,
    private signalRConnectionService: SignalRConnectionService
  ) {
    this.jobDataService.getNormalizedFields().subscribe({
      next: response => {
        this.jobBridgeService.setNormalizedFields(response);
      }
    });



    this.signalRConnectionService.jobUpdated$.subscribe(event => {
      this.onJobUpdated(event);
    });
  }


  public getNormalizedFields(): string[] {
    const resourceType = this.jobBridgeService.getSelectedResourceType();
    const normalizedFields = this.jobBridgeService.getNormalizedFields();

    if (resourceType == null || !normalizedFields) {
      return [];
    }

    return normalizedFields[FhirResourceType[resourceType]] ?? [];
  }


  public getStageStatusText(stage: JobStage): string {
    const status = this.getStageStatus(stage);

    if (stage === JobStage.ResourceTypeDetection && status === 'AiSuggested') {
      return 'AI analyzing your data...';
    }

    return status;
  }

  public getStageStatus(stage: JobStage): string {
    const currentStage = this.jobBridgeService.getJobStage();
    const currentStatus = this.jobBridgeService.getJobStatus();

    if (stage < currentStage) {
      return 'Completed';
    }

    if (stage > currentStage) {
      return 'Waiting';
    }

    return JobStatus[currentStatus];
  }

  public getStageIcon(stage: JobStage): string {
    const currentStage = this.jobBridgeService.getJobStage();
    const currentStatus = this.jobBridgeService.getJobStatus();

    if (stage < currentStage) {
      return '✓';
    }

    if (stage > currentStage) {
      return '○';
    }

    if (currentStatus === JobStatus.Failed) {
      return '✕';
    }

    if (currentStatus === JobStatus.Completed) {
      return '✓';
    }

    return '●';
  }

  public getStageClass(stage: JobStage): string {
    const status = this.getStageStatus(stage);

    switch (status) {
      case 'Completed':
        return 'text-success';

      case 'Failed':
        return 'text-danger';

      case 'InProgress':
      case 'AiSuggested':
        return 'text-primary';

      default:
        return 'text-secondary';
    }
  }

  public showResourceTypeApproval(currentStage: JobStage): boolean {
    return currentStage == JobStage.ResourceTypeDetection && this.jobBridgeService.getJobStage() === JobStage.ResourceTypeDetection &&
      this.jobBridgeService.getJobStatus() === JobStatus.AiSuggested;
  }

  public resourceTypeChanged(event: Event): void {
    const select = event.target as HTMLSelectElement;
    const resourceType = FhirResourceType[select.value as keyof typeof FhirResourceType];

    this.jobBridgeService.setSelectedResourceType(resourceType);
  }

  public approveResourceType(): void {
    const jobId = this.jobBridgeService.getJobId();
    if (!jobId) return;

    this.jobDataService.approveResourceType(jobId, this.jobBridgeService.getSelectedResourceType()!).subscribe({
      next: response => {
        if (response.success) {
          this.jobBridgeService.setJobStage(JobStage.FieldMapping);
          this.jobBridgeService.setJobStatus(JobStatus.InProgress);
          return;
        }

        this.jobBridgeService.setJobStatus(JobStatus.Failed);
      },
      error: () => {
        this.jobBridgeService.setJobStatus(JobStatus.Failed);
      }
    });
  }


  public showResourceTypeAiSuggested(): boolean {
    const result = this.jobBridgeService.getResourceTypeDetection();

    return result?.aiConfidence != null && !result.isApproved;
  }

  public getResourceTypeAiConfidence(): string {
    const confidence = this.jobBridgeService.getResourceTypeDetection()?.aiConfidence;

    return `${(confidence! * 100).toFixed(2)}%`;
  }


  public showFieldMappingApproval(currentStage: JobStage): boolean {
    return currentStage === JobStage.FieldMapping &&
      this.jobBridgeService.getJobStage() === JobStage.FieldMapping &&
      this.jobBridgeService.getJobStatus() === JobStatus.AiSuggested;
  }

  public showFieldMappingAiSuggested(aiConfidence?: number): boolean {
    const result = this.jobBridgeService.getFieldMapping();

    return aiConfidence != null && result?.isApproved === false;
  }

  public getFieldMappingAiConfidence(aiConfidence?: number): string {
    return `${(aiConfidence! * 100).toFixed(2)}%`;
  }

  public approveFieldMapping(): void {
    const jobId = this.jobBridgeService.getJobId();
    const fieldMapping = this.jobBridgeService.getFieldMapping();

    if (!jobId || !fieldMapping) return;

    const request = {
      mappings: fieldMapping.mappings.map(mapping => ({
        mappingId: mapping.mappingId,
        normalizedField: mapping.normalizedField
      }))
    };

    this.jobDataService.approveFieldMapping(jobId, request).subscribe({
      next: response => {
        if (response.success) {
          this.jobBridgeService.setJobStage(JobStage.DataValidation);
          this.jobBridgeService.setJobStatus(JobStatus.InProgress);
          return;
        }

        this.jobBridgeService.setJobStatus(JobStatus.Failed);
      },
      error: () => {
        this.jobBridgeService.setJobStatus(JobStatus.Failed);
      }
    });
  }

  private onJobUpdated(event: JobNotificationEvent): void {
    if (event.jobId !== this.jobBridgeService.getJobId()) {
      return;
    }

    if (event.stage === JobStage.ResourceTypeDetection && event.data) {
      const result = event.data as ResourceTypeDetectionResult;
      this.jobBridgeService.setResourceTypeDetection(result);
      this.jobBridgeService.setSelectedResourceType(result.resourceType);
    }

    else if (event.stage === JobStage.FieldMapping && event.data) {
      const result = event.data as FieldMappingResult;
      this.jobBridgeService.setFieldMapping(result);
    }


    if (event.status === JobStatus.Failed) {
      this.jobBridgeService.setErrorMessages((event.data as string[]) ?? []);
    } else {
      this.jobBridgeService.clearErrorMessages();
    }

    this.jobBridgeService.setJobStage(event.stage);
    this.jobBridgeService.setJobStatus(event.status);
  }

  public showJobError(stage: JobStage): boolean {
    return stage === this.jobBridgeService.getJobStage() &&
      this.jobBridgeService.getJobStatus() === JobStatus.Failed;
  }
}
