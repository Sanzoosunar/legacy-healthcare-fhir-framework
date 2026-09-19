using LegacyHealthcareFHIR.Core.Models.Legacy;

namespace LegacyHealthcareFHIR.Core.Models.Import;

public class SourceFileData
{
    public List<string> Headers { get; set; } = new();

    public List<LegacyRecord> SampleRecords { get; set; } = new();
}