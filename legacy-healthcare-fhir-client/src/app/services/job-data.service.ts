import { Injectable } from '@angular/core';
import { delay, Observable, of } from 'rxjs';
import { ApproveFieldMappingRequest, ApproveFieldMappingResponse, ApproveResourceTypeRequest, ApproveResourceTypeResponse, CreateJobResponse, ReRunJobRequest } from '../models/job-response';
import { backendUrl } from '../environment';
import { HttpClient } from '@angular/common/http';
import { FhirResourceType } from '../enums/resource-type';
import { NormalizedFieldsResponse } from '../models/job';

@Injectable({
  providedIn: 'root'
})
export class JobDataService {

  private readonly baseUrl = `${backendUrl}/api/v1/imports`;

  constructor(private httpClient: HttpClient) {
  }


  public getNormalizedFields(): Observable<NormalizedFieldsResponse> {
    return this.httpClient.get<NormalizedFieldsResponse>(`${this.baseUrl}/normalized-fields`);
  }


  public upload(file: File): Observable<CreateJobResponse> {
    const formData = new FormData();
    formData.append('file', file);

    return this.httpClient.post<CreateJobResponse>(this.baseUrl, formData);
  }

  public approveResourceType(jobId: string, resourceType: FhirResourceType): Observable<ApproveResourceTypeResponse> {
    const request: ApproveResourceTypeRequest = { resourceType: resourceType }
    return this.httpClient.post<ApproveResourceTypeResponse>(`${this.baseUrl}/${jobId}/resource-type/approve`, request);

  }

  public reRunJob(request: ReRunJobRequest): Observable<string> {
    return this.httpClient.post(`${this.baseUrl}/rerun`, request, { responseType: 'text' });
  }

  public approveFieldMapping(jobId: string, request: ApproveFieldMappingRequest): Observable<ApproveFieldMappingResponse> {
    return this.httpClient.post<ApproveFieldMappingResponse>(`${this.baseUrl}/${jobId}/field-mapping/approve`, request);
  }
}
