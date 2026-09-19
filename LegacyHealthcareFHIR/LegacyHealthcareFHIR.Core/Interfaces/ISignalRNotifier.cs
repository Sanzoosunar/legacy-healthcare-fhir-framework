namespace LegacyHealthcareFHIR.Core.Interfaces;

public interface ISignalRNotifier
{
    Task SendAsync(
        string eventName,
        object data);
}