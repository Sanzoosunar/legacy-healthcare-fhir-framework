
namespace LegacyHealthcareFHIR.Core.Dto;

public class DashboardDto
{
    public int TotalJobs { get; set; }
    public int TotaFailedJobs { get; set; }
    public int TotalInProgressJobs { get; set; }
    public int TotalNeedReviewJobs { get; set; }
}