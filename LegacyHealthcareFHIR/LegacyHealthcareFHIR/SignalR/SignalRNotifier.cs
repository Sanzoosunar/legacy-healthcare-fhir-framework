using LegacyHealthcareFHIR.Core.Enums;
using LegacyHealthcareFHIR.Core.Interfaces;
using LegacyHealthcareFHIR.Core.Models.Notifications;
using Microsoft.AspNetCore.SignalR;

namespace LegacyHealthcareFHIR.Web.SignalR;

public class SignalRNotifier : ISignalRNotifier
{
    private readonly IHubContext<SignalRHub> _hubContext;

    public SignalRNotifier(
        IHubContext<SignalRHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task SendAsync(
        JobNotificationEvent notificationEvent)
    {
        await _hubContext.Clients.All.SendAsync("JobUpdated", notificationEvent);
    }

    public async Task SendAsync(Guid jobId, JobStage stage, JobStatus status, object? data = null)
    {
        await SendAsync(new JobNotificationEvent
        {
            JobId = jobId,
            Stage = stage,
            Status = status,
            Data = data
        });
    }
}