using LegacyHealthcareFHIR.Core.Dto;
using LegacyHealthcareFHIR.Core.Enums;
using LegacyHealthcareFHIR.Core.Mapping;
using LegacyHealthcareFHIR.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LegacyHealthcareFHIR.Infrastructure.Services;
public class DashboardService(AppDbContext _dbContext)
{
	public async Task<DashboardDto> GetDashboard(int hospitalId)
	{
        var jobsQuery = _dbContext.ImportJobs.AsNoTracking().Where(job => job.HospitalId == hospitalId);

        var statusCounts = await jobsQuery.GroupBy(job => job.Status).Select(group => new { Status = group.Key, Count = group.Count() }).ToListAsync();

        var recentJobs = await jobsQuery.OrderByDescending(job => job.CreatedAtUtc).Take(5).ToListAsync();

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