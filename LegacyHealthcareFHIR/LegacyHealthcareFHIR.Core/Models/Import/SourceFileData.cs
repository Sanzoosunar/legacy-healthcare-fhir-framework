using LegacyHealthcareFHIR.Core.Models.Legacy;

namespace LegacyHealthcareFHIR.Core.Models.Import;

public class SourceFileData
{
    public int Id { get; set; }
    public Guid ImportJobId { get; set; }
    public List<string> Headers { get; set; } = new();

    public List<LegacyRecord> SampleRecords { get; set; } = new();
}