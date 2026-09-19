using LegacyHealthcareFHIR.Core.Enums;
using LegacyHealthcareFHIR.Core.Models.Detection;
using LegacyHealthcareFHIR.Core.Models.Import;

namespace LegacyHealthcareFHIR.Core.Interfaces;

public interface IResourceTypeAiService
{
    Task<ResourceTypeDetectionAiResult> DetectAsync(
        SourceFileData sourceFileData);
}