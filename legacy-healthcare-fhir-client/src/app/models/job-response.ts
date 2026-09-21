import { FhirResourceType } from "../enums/resource-type";

export interface CreateJobResponse {
    jobId: string;
}
export interface ApproveResourceTypeRequest {
    resourceType: FhirResourceType;
}

export interface ApproveResourceTypeResponse {
    success: boolean;
}

export interface ReRunJobRequest {
    jobId: string;
    stage: string;
}

export interface ApproveFieldMappingRequest {
    mappings: ApproveFieldMappingItemRequest[];
}

export interface ApproveFieldMappingItemRequest {
    mappingId: number;
    normalizedField?: string;
}

export interface ApproveFieldMappingResponse {
    success: boolean;
}