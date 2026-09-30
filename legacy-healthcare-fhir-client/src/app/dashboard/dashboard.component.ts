import { Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { DashboardService } from '../services/dashboard.service';
import { DashboardDto } from '../models/dashboard-dto';
import { takeUntil } from 'rxjs';
import { DestroyComponent } from '../destroy.component';
import { JobStagesStyleMap, JobStatusStyleMap } from '../enums/job-enums';
import { JobsTableComponent } from '../jobs-table/jobs-table.component';
import { LoadingComponent } from '../loading/loading.component';

@Component({
  selector: 'app-dashboard',
  imports: [RouterLink, JobsTableComponent, LoadingComponent],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css'
})
export class DashboardComponent extends DestroyComponent {
  private dashboardService = inject(DashboardService);
  dashboard!: DashboardDto;

  public isLoading = true;
  errorMessage = '';

  readonly jobStages = JobStagesStyleMap;
  readonly jobStatuses = JobStatusStyleMap;
  ngOnInit(): void {
    this.loadDashboard();
  }

  loadDashboard(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.dashboardService.getDashboard().pipe(takeUntil(this.destroy)).subscribe({
      next: data => {
        this.dashboard = data;
        this.isLoading = false;
      },
      error: () => {
        this.errorMessage = 'Unable to load dashboard. Please try again.';
        this.isLoading = false;
      }
    });
  }
}
