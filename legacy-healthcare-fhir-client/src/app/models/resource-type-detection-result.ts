import { FhirResourceType } from "../enums/resource-type";

export interface ResourceTypeDetectionResult {
    resourceType: FhirResourceType;
    isApproved: boolean;
    aiConfidence?: number;
    resourceTypeText?: string;
}