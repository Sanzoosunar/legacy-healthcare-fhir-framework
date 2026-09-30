import { Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { DashboardService } from '../services/dashboard.service';
import { DashboardDto } from '../models/dashboard-dto';
import { takeUntil } from 'rxjs';
import { DestroyComponent } from '../destroy.component';

@Component({
  selector: 'app-dashboard',
  imports: [RouterLink],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css'
})
export class DashboardComponent extends DestroyComponent {
  private dashboardService = inject(DashboardService);
  dashboard: DashboardDto = {
    totalJobs: 0,
    totaFailedJobs: 0,
    totalInProgressJobs: 0,
    totalNeedReviewJobs: 0
  };

  ngOnInit(): void {
    this.loadDashboard();
  }

  loadDashboard(): void {
    this.dashboardService.getDashboard().pipe(takeUntil(this.destroy))
      .subscribe(data => this.dashboard = data);
  }
}
