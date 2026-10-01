import { JobStage, JobStatus } from "./enums/job-enums";

export function showDownloadButton(stage: JobStage, status: JobStatus): boolean {
    return stage == JobStage.Completed && status == JobStatus.Completed;
}