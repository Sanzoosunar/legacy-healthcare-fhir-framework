namespace LegacyHealthcareFHIR.Core.Enums;

public enum JobStatus
{
    Started,
    InProgress,
    AiSuggestionWaiting,
    AiSuggested,
    ApprovalPending,
    Approved,
    Completed,
    Failed
}