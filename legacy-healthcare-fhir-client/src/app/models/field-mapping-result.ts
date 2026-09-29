export interface FieldMappingResult {
    configurationId: number;
    isApproved: boolean;
    mappings: FieldMappingItemResult[];
}

export interface FieldMappingItemResult {
    mappingId: number;
    sourceField: string;
    normalizedField: string;
    aiConfidence?: number;
    aiExplanation?: string;
}