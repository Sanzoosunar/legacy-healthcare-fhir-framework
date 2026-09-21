namespace LegacyHealthcareFHIR.Core.Models.Import;
public class ApproveFieldMappingRequest
{
    public List<ApproveFieldMappingItemRequest> Mappings { get; set; } = new();
}

public class ApproveFieldMappingItemRequest
{
    public int MappingId { get; set; }
    public string? NormalizedField { get; set; }
}
