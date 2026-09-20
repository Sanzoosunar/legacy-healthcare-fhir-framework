namespace LegacyHealthcareFHIR.Core.Interfaces;

public interface IAiService
{
    Task<string> GetResponseAsync(string prompt);
}