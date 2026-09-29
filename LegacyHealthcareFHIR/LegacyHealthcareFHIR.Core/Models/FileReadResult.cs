using LegacyHealthcareFHIR.Core.Models.Legacy;


namespace LegacyHealthcareFHIR.Core.Models;
public class FileReadResult
{
    public bool Success { get; set; }
    public List<string> Errors { get; set; } = new();
    public List<string> Headers { get; set; } = new();
    public List<LegacyRecord> Records { get; set; } = new();
}