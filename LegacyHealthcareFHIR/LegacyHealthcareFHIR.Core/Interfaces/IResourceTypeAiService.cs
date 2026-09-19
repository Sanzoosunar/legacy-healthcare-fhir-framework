using LegacyHealthcareFHIR.Core.Enums;
using LegacyHealthcareFHIR.Core.Models.Import;

namespace LegacyHealthcareFHIR.Core.Interfaces;

public interface IResourceTypeAiService
{
    Task<ResourceTypeDetectionResult> DetectAsync(
        SourceFileData sourceFileData);
}