import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { DashboardDto } from '../models/dashboard-dto';
import { backendUrl } from '../environment';

@Injectable({
  providedIn: 'root'
})
export class DashboardService {

  private http = inject(HttpClient);
  private baseUrl = `${backendUrl}/api/dashboard`;

  getDashboard(): Observable<DashboardDto> {
    return this.http.get<DashboardDto>(this.baseUrl);
  }
}
