import { Component, Input } from '@angular/core';
import { DatePipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { JobResponseDto } from '../models/dashboard-dto';
import { JobStagesStyleMap, JobStatusStyleMap } from '../enums/job-enums';

@Component({
  selector: 'app-jobs-table',
  imports: [RouterLink, DatePipe],
  templateUrl: './jobs-table.component.html',
  styleUrl: './jobs-table.component.css'
})
export class JobsTableComponent {
  @Input() jobs: JobResponseDto[] = [];

  readonly jobStages = JobStagesStyleMap;
  readonly jobStatuses = JobStatusStyleMap;
}