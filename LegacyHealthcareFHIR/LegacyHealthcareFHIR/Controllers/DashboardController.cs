using LegacyHealthcareFHIR.Core.Dto;
using LegacyHealthcareFHIR.Core.Models;
using LegacyHealthcareFHIR.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LegacyHealthcareFHIR.Web.Controllers;

[Authorize]
[ApiController]
[Route("api/dashboard")]
public class DashboardController(
    DashboardService dashboardService,
    CurrentUser currentUser) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<DashboardDto>> Get()
    {
        var dashboard = await dashboardService.GetDashboard(currentUser.HospitalId);
        return Ok(dashboard);
    }
}
