using LegacyHealthcareFHIR.Core.Interfaces;
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
        string eventName,
        object data)
    {
        await _hubContext.Clients.All.SendAsync(
            eventName,
            data);
    }
}