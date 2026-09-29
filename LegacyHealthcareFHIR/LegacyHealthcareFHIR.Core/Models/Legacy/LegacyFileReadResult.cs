namespace LegacyHealthcareFHIR.Core.Models.Legacy;

public class LegacyFileReadResult
{
    public bool IsSuccess { get; set; }

    public List<LegacyRecord> Records { get; set; } = new();

    public List<string> Errors { get; set; } = new();
}