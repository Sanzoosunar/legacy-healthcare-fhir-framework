using System;
using System.Collections.Generic;
using System.Text;

namespace LegacyHealthcareFHIR.Core.Models.Legacy;

public enum CsvReadErrorType
{
    EmptyFile,
    MissingHeader,
    InvalidCsv,
    ReadFailure
}
public class CsvReadError
{
    public CsvReadErrorType ErrorType { get; set; }

    public string? Details { get; set; }
}
