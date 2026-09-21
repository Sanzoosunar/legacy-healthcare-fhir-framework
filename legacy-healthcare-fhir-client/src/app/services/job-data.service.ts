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
  constructor(private httpClient: HttpClient) {
  }


  public getNormalizedFields(): Observable<NormalizedFieldsResponse> {
    return this.httpClient.get<NormalizedFieldsResponse>(`${backendUrl}api/imports/normalized-fields`);
  }

  public upload(file: File): Observable<CreateJobResponse> {
    const response: CreateJobResponse = {
      jobId: '7a83e2c4-1d9f-4e7a-b2c1-9f8e6d3a4b2c'
    };

    return of(response).pipe(delay(1000));
  }

  public approveResourceType(jobId: string, resourceType: FhirResourceType): Observable<ApproveResourceTypeResponse> {
    const response: ApproveResourceTypeResponse = {
      success: true
    };

    return of(response).pipe(delay(1000));
  }

  public reRunJob(request: ReRunJobRequest): Observable<string> {
    return of('Job is running in background!!').pipe(delay(1000));
  }

  public approveFieldMapping(jobId: string, request: ApproveFieldMappingRequest): Observable<ApproveFieldMappingResponse> {
    const response: ApproveFieldMappingResponse = {
      success: true
    };

    return of(response).pipe(delay(1000));
  }
}
