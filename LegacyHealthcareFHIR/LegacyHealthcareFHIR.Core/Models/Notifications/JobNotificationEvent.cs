using LegacyHealthcareFHIR.Core.Enums;

namespace LegacyHealthcareFHIR.Core.Models.Notifications;

public class JobNotificationEvent
{
    public JobNotificationEventType EventType { get; set; }

    public Guid JobId { get; set; }

    public JobStatus Status { get; set; }

    public int ProgressPercentage { get; set; }

    public string? Stage { get; set; }

    public object? Data { get; set; }
}