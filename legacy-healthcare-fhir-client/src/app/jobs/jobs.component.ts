import { Component, inject } from '@angular/core';
import { JobDataService } from '../services/job-data.service';
import { JobResponseDto } from '../models/dashboard-dto';
import { DestroyComponent } from '../destroy.component';
import { takeUntil } from 'rxjs';
import { JobsTableComponent } from '../jobs-table/jobs-table.component';
import { LoadingComponent } from '../loading/loading.component';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-jobs',
  imports: [RouterLink, JobsTableComponent, LoadingComponent],
  templateUrl: './jobs.component.html',
  styleUrl: './jobs.component.css'
})
export class JobsComponent extends DestroyComponent {
  jobDataService = inject(JobDataService)
  jobs: JobResponseDto[] = [];
  isLoading = true;

  constructor() {
    super();
    this.loadJobs();
  }

  public loadJobs() {
    this.isLoading = true;
    this.jobDataService.getJobs()
      .pipe(takeUntil(this.destroy))
      .subscribe((data) => {
        this.isLoading = false;
        this.jobs = data
      })
  }
}
