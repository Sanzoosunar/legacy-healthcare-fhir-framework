using LegacyHealthcareFHIR.Core.Enums;
using LegacyHealthcareFHIR.Core.Models.Notifications;

namespace LegacyHealthcareFHIR.Core.Interfaces;

public interface ISignalRNotifier
{
    Task SendAsync(JobNotificationEvent notificationEvent);
    Task SendAsync(Guid jobId, JobStage stage, JobStatus status, object? data = null);

}