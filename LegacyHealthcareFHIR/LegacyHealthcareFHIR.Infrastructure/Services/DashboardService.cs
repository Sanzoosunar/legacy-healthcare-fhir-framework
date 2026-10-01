using LegacyHealthcareFHIR.Core.Dto;
using LegacyHealthcareFHIR.Core.Enums;
using LegacyHealthcareFHIR.Core.Mapping;
using LegacyHealthcareFHIR.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LegacyHealthcareFHIR.Infrastructure.Services;
public class DashboardService(IJobRepository _jobRepository)
{
	public async Task<DashboardDto> GetDashboard(int hospitalId)
	{
        var jobs = await _jobRepository.GetJobsByHospitalId(hospitalId);

        var statusCounts = jobs.GroupBy(job => job.Status).Select(group => new { Status = group.Key, Count = group.Count() });

        var recentJobs = jobs.OrderByDescending(job => job.CreatedAtUtc).Take(5);

        return new DashboardDto
        {
            TotalJobs = statusCounts.Sum(item => item.Count),
            TotaFailedJobs = statusCounts.Where(item => item.Status == JobStatus.Failed).Sum(item => item.Count),
            TotalInProgressJobs = statusCounts.Where(item => item.Status == JobStatus.InProgress).Sum(item => item.Count),
            TotalNeedReviewJobs = statusCounts.Where(item => item.Status == JobStatus.AiSuggested).Sum(item => item.Count),
            RecentJobs = recentJobs.Select(JobDataConverter.MapToJobResponseDto).ToList()
        };
    }
}