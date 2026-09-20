using LegacyHealthcareFHIR.Core.Enums;

namespace LegacyHealthcareFHIR.Core.Models;

public class BackgroundTaskMessage
{
    public Guid JobId { get; set; }
    public JobStage Stage { get; set; }
}