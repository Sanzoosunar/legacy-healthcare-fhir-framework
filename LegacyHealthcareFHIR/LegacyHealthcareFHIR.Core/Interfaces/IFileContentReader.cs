using LegacyHealthcareFHIR.Core.Models;
using LegacyHealthcareFHIR.Core.Models.Legacy;

namespace LegacyHealthcareFHIR.Core.Interfaces;

public interface IFileContentReader
{
    public string Extension { get; }

    public FileReadResult Read(Stream stream);
}