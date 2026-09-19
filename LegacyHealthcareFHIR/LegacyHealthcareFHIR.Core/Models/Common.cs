namespace LegacyHealthcareFHIR.Core.Models.Common;

public class ProcessResult<TData, TError>
{
    public bool IsSuccess { get; set; }

    public TData Data { get; set; } = default!;

    public List<TError> Errors { get; set; } = new();
}