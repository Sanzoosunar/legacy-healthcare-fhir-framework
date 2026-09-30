using LegacyHealthcareFHIR.Core.Dto;

namespace LegacyHealthcareFHIR.Infrastructure.Services;
public class DashboardService
{
	public async Task<DashboardDto> GetDashboard(int hospitalId)
	{
		await Task.CompletedTask;
		return new DashboardDto
		{
			TotalJobs = 23,
			TotaFailedJobs = 4,
			TotalInProgressJobs = 3,
			TotalNeedReviewJobs = 7
		};
	}
}