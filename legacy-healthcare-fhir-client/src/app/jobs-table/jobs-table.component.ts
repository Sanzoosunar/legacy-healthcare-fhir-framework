import { Component, Input } from '@angular/core';
import { DatePipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { JobResponseDto } from '../models/dashboard-dto';
import { JobStage, JobStagesStyleMap, JobStatus, JobStatusStyleMap } from '../enums/job-enums';
import { JobDownloadComponent } from '../job-download/job-download.component';

@Component({
  selector: 'app-jobs-table',
  imports: [DatePipe, JobDownloadComponent],
  templateUrl: './jobs-table.component.html',
  styleUrl: './jobs-table.component.css'
})
export class JobsTableComponent {
  @Input() jobs: JobResponseDto[] = [];

  readonly jobStages = JobStagesStyleMap;
  readonly jobStatuses = JobStatusStyleMap;

  public showDownloadButton(stage: JobStage, status: JobStatus): boolean {
    return stage == JobStage.Completed && status == JobStatus.Completed;
  }
}