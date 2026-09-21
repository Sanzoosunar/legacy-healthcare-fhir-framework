import { JobStage, JobStatus } from "../enums/job-enums";

export interface ImportJob {
    id: string;
    hospitalId: number;
    originalFileName: string;
    storedFileName: string;
    inputFormat: string;
    resourceTypeDetectionId?: number;
    status: JobStatus;
    mappingConfigurationId?: number;
    jobStage: JobStage;
    createdAtUtc: string;
    completedAtUtc?: string;
}


export interface JobNotificationEvent {
    jobId: string;
    stage: JobStage;
    status: JobStatus;
    data?: unknown;
}

export type NormalizedFieldsResponse = Record<string, string[]>;