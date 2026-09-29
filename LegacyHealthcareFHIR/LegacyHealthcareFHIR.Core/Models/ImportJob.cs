using LegacyHealthcareFHIR.Core.Enums;
namespace LegacyHealthcareFHIR.Core.Models;

public class ImportJob
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public int HospitalId { get; set; }

    public string OriginalFileName { get; set; }

    public string StoredFileName { get; set; }
    public string InputFormat { get; set; }

    public int? ResourceTypeDetectionId { get; set; }
    public ResourceTypeDetection? ResourceTypeDetection { get; set; }
    public JobStatus Status { get; set; } = JobStatus.Started;
    public int? MappingConfigurationId { get; set; }
    public MappingConfiguration? MappingConfiguration { get; set; }
    public JobStage JobStage { get; set; } = JobStage.ResourceTypeDetection;
  
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public DateTime? CompletedAtUtc { get; set; }

    public string? OutputFileName {  get; set; }
}
