import { JobStage, JobStatus } from "../enums/job-enums";
import { FhirResourceType } from "../enums/resource-type";

export interface DashboardDto {
    totalJobs: number;
    totaFailedJobs: number;
    totalNeedReviewJobs: number;
    totalInProgressJobs: number;
    recentJobs: Array<JobResponseDto>;
}

export interface JobResponseDto {
    jobId: string;
    originalFileName: string;
    resourceType: FhirResourceType | null;
    jobStage: JobStage;
    jobStatus: JobStatus;
    createdAt: string;
}