using LegacyHealthcareFHIR.Core.Enums;
using LegacyHealthcareFHIR.Core.Validation;

namespace LegacyHealthcareFHIR.Tests.Validation;

public class JobStateValidatorTests
{
    [Theory]
    [InlineData(JobStage.Created, JobStatus.Completed, JobStage.ResourceTypeDetection, true)]
    [InlineData(JobStage.ResourceTypeDetection, JobStatus.Failed, JobStage.ResourceTypeDetection, true)]
    [InlineData(JobStage.ResourceTypeDetection, JobStatus.Completed, JobStage.FieldMapping, true)]
    [InlineData(JobStage.FieldMapping, JobStatus.Failed, JobStage.FieldMapping, true)]
    [InlineData(JobStage.FieldMapping, JobStatus.Completed, JobStage.DataValidation, true)]
    [InlineData(JobStage.DataValidation, JobStatus.Failed, JobStage.DataValidation, true)]

    [InlineData(JobStage.Created, JobStatus.Failed, JobStage.ResourceTypeDetection, false)]
    [InlineData(JobStage.ResourceTypeDetection, JobStatus.InProgress, JobStage.FieldMapping, false)]
    [InlineData(JobStage.FieldMapping, JobStatus.InProgress, JobStage.DataValidation, false)]
    [InlineData(JobStage.Created, JobStatus.Completed, JobStage.DataValidation, false)]
    [InlineData(JobStage.ResourceTypeDetection, JobStatus.Completed, JobStage.DataValidation, false)]
    [InlineData(JobStage.DataValidation, JobStatus.Completed, JobStage.DataValidation, false)]
    public void IsValid_ReturnsExpectedResult(JobStage currentStage, JobStatus currentStatus, JobStage targetStage, bool expected)
    {
        var result = JobStateValidator.IsValid(currentStage, currentStatus, targetStage);

        Assert.Equal(expected, result);
    }
}