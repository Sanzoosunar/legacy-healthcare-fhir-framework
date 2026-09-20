using LegacyHealthcareFHIR.Core.Enums;
namespace LegacyHealthcareFHIR.Core.Models;

public class ImportJob
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public int HospitalId { get; set; }

    public string OriginalFileName { get; set; }

    public string StoredFileName { get; set; }
    public string InputFormat { get; set; }

    public FhirResourceType? ResourceType { get; set; }
    public JobStatus Status { get; set; } = JobStatus.Started;
    public JobStage JobStage { get; set; } = JobStage.ResourceTypeDetection;
    public int ProgressPercentage { get; set; } = 0;

    public int TotalRecords { get; set; }

    public int SuccessfulRecords { get; set; }

    public int FailedRecords { get; set; }

    public string? ErrorMessage { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public DateTime? StartedAtUtc { get; set; }

    public DateTime? CompletedAtUtc { get; set; }
}
