using LegacyHealthcareFHIR.Core.Enums;

namespace LegacyHealthcareFHIR.Core.Models.Notifications;

public class JobNotificationEvent
{
    public Guid JobId { get; set; }
    public JobStage Stage { get; set; }
    public JobStatus Status { get; set; }
    public int ProgressPercentage { get; set; }
    public object? Data { get; set; }
}