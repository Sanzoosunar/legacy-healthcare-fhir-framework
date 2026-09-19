namespace LegacyHealthcareFHIR.Core.Models.Legacy;

public class CsvReadResult
{
    public bool IsSuccess { get; set; }

    public List<LegacyRecord> Records { get; set; } = new();

    public List<CsvReadError> Errors { get; set; } = new();
}