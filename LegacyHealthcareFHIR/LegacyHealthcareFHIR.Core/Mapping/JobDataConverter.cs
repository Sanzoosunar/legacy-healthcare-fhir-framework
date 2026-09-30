using LegacyHealthcareFHIR.Core.Dto;
using LegacyHealthcareFHIR.Core.Models;

namespace LegacyHealthcareFHIR.Core.Mapping;

public class JobDataConverter
{
    public static JobResponseDto MapToJobResponseDto(ImportJob job)
    {
        return new JobResponseDto
        {
            JobId = job.Id,
            OriginalFileName = job.OriginalFileName,
            ResourceType = job.ResourceTypeDetection?.ResourceType,
            JobStage = job.JobStage,
            JobStatus = job.Status,
            CreatedAt = job.CreatedAtUtc.ToString("O")
        };
    }
}