using LegacyHealthcareFHIR.Core.Models.Import;
using LegacyHealthcareFHIR.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace LegacyHealthcareFHIR.Web.Controllers;

[ApiController]
[Route("api/imports")]
public class ImportsController : ControllerBase
{
    private readonly ImportsService _importsService;

    public ImportsController(ImportsService importsService)
    {
        _importsService = importsService;
    }

    [HttpPost("{jobId:guid}/resource-type/approve")]
    public async Task<IActionResult> ApproveResourceType(Guid jobId, [FromBody] ApproveResourceTypeRequest request)
    {
        var success = await _importsService.ApproveResourceTypeAsync(jobId, request.DetectionId, request.ResourceType);

        return Ok(new { success });
    }
}