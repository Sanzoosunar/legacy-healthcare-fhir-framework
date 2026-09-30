
using LegacyHealthcareFHIR.Core.Enums;

namespace LegacyHealthcareFHIR.Core.Dto;
public class JobResponseDto
{
    public Guid JobId { get; set; }
    public string OriginalFileName { get; set; }
    public FhirResourceType? ResourceType { get; set; }
    public JobStage JobStage { get; set; }
    public JobStatus JobStatus { get; set; }
    public string CreatedAt { get; set; }
}
