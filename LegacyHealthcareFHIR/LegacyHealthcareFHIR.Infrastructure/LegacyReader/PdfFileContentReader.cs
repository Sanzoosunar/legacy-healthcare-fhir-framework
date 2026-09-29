using CsvHelper;
using LegacyHealthcareFHIR.Core.Interfaces;
using LegacyHealthcareFHIR.Core.Models;
using LegacyHealthcareFHIR.Core.Models.Legacy;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace LegacyHealthcareFHIR.Infrastructure.LegacyReader;

public class PdfFileContentReader: IFileContentReader
{
    public string Extension => "PDF";
    public FileReadResult Read(Stream stream)
    {
        throw new Exception("Not implemented");
    }
}