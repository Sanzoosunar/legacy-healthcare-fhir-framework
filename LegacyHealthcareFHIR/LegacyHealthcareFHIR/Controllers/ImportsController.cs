using Hl7.Fhir.Model;
using LegacyHealthcareFHIR.Core.Dto;
using LegacyHealthcareFHIR.Core.Enums;
using LegacyHealthcareFHIR.Core.Interfaces;
using LegacyHealthcareFHIR.Core.Models;
using LegacyHealthcareFHIR.Core.Models.Import;
using LegacyHealthcareFHIR.Core.Models.Normalized;
using LegacyHealthcareFHIR.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LegacyHealthcareFHIR.Web.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/imports")]
public class ImportsController(ImportsService _importsService,
    IBackgroundTaskQueue _queue,
    CurrentUser _currentUser) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromForm] IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("File is required.");
        }

        await using var stream = file.OpenReadStream();
        var job = await _importsService.CreateImportJob(stream, file.FileName, _currentUser.HospitalId, _currentUser.UserId);

        return Accepted(new
        {
            jobId = job.Id
        });
    }

    [HttpPost("{jobId:guid}/resource-type/approve")]
    public async Task<IActionResult> ApproveResourceType(Guid jobId, [FromBody] ApproveResourceTypeRequest request)
    {
        var success = await _importsService.ApproveResourceTypeAsync(jobId, request.ResourceType);

        return Ok(new { success });
    }


    [HttpPost("rerun")]
    public async Task<IActionResult> ReRunJob([FromBody] BackgroundTaskMessage message)
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

    [HttpGet("normalized-fields")]
    public IActionResult GetNormalizedFields()
    {
        var fields = new Dictionary<FhirResourceType, List<string>>
        {
            [FhirResourceType.Patient] = typeof(PatientData).GetProperties().Select(x => x.Name).ToList(),
            [FhirResourceType.Encounter] = typeof(EncounterData).GetProperties().Select(x => x.Name).ToList(),
            [FhirResourceType.Observation] = typeof(ObservationData).GetProperties().Select(x => x.Name).ToList(),
        };

        return Ok(fields);
    }

    [HttpGet("jobs")]
    public async Task<ActionResult<List<JobResponseDto>>> GetJobs()
    {
        var jobs = await _importsService.GetJobsByHospitalId(_currentUser.HospitalId);
        return Ok(jobs);
    }
}