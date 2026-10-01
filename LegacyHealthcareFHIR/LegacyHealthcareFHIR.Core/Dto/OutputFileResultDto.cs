namespace LegacyHealthcareFHIR.Core.Dto;
public class OutputFileResultDto
{
    public Stream FileStream { get; set; } = Stream.Null;
    public string FileName { get; set; } = string.Empty;
}
