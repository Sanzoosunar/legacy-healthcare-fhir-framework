using LegacyHealthcareFHIR.Core.Enums;
using LegacyHealthcareFHIR.Core.Interfaces;
using LegacyHealthcareFHIR.Core.Models;
using LegacyHealthcareFHIR.Core.Models.Import;
using LegacyHealthcareFHIR.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;


namespace LegacyHealthcareFHIR.Web.Controllers;

[ApiController]
[Route("api/imports")]
public class ImportsController : ControllerBase
{
    private readonly ImportsService _importsService;
    private readonly IBackgroundTaskQueue _queue;

    public ImportsController(ImportsService importsService, IBackgroundTaskQueue queue)
    {
        _importsService = importsService;
        _queue = queue;
    }

    [HttpPost("{jobId:guid}/resource-type/approve")]
    public async Task<IActionResult> ApproveResourceType(Guid jobId, [FromBody] ApproveResourceTypeRequest request)
    {
        var success = await _importsService.ApproveResourceTypeAsync(jobId, request.ResourceType);

        return Ok(new { success });
    }

    [HttpPost]
    public async Task<IActionResult> Create(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("File is required.");
        }

        var hospitalId = 1;

        await using var stream = file.OpenReadStream();

        var job = await _importsService.CreateImportJob(stream, file.FileName, hospitalId, "CSV");

        return Accepted(new
        {
            jobId = job.Id
        });
    }


    [HttpPost("rerun")]
    public async Task<IActionResult> ReRunJob(BackgroundTaskMessage message)
    {
        await _queue.EnqueueAsync(message.Stage, message.JobId);
        return Ok("Job is running in background!!");
    }

    [HttpPost("{jobId:guid}/field-mapping/approve")]
    public async Task<IActionResult> ApproveFieldMapping(Guid jobId, [FromBody] ApproveFieldMappingRequest request)
    {
        await _importsService.ApproveFieldMappingAsync(jobId, request);
        return Ok();
    }
}