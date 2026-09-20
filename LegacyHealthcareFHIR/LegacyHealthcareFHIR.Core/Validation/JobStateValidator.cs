using LegacyHealthcareFHIR.Core.Enums;

namespace LegacyHealthcareFHIR.Core.Validation;

public static class JobStateValidator
{
    public static bool IsValid(JobStage currentStage, JobStatus currentStatus, JobStage targetStage)
    {
        return targetStage switch
        {
            JobStage.ResourceTypeDetection =>
                (currentStage == JobStage.Created && currentStatus == JobStatus.Completed) ||
                (currentStage == JobStage.ResourceTypeDetection && currentStatus == JobStatus.Failed),

            JobStage.FieldMapping =>
                (currentStage == JobStage.ResourceTypeDetection && currentStatus == JobStatus.Completed) ||
                (currentStage == JobStage.FieldMapping && currentStatus == JobStatus.Failed),

            JobStage.DataValidation =>
                (currentStage == JobStage.FieldMapping && currentStatus == JobStatus.Completed) ||
                (currentStage == JobStage.DataValidation && currentStatus == JobStatus.Failed),

            _ => false
        };
    }
}