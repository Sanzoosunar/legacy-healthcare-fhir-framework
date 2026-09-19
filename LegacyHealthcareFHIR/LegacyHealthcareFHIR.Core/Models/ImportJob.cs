using System;
using System.Collections.Generic;
using System.Text;
namespace LegacyHealthcareFHIR.Core.Models;

public class ImportJob
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public int HospitalId { get; set; }

    public required string FileName { get; set; }

    public required string InputFormat { get; set; }

    public required string ResourceType { get; set; }

    public string Status { get; set; } = "Pending";

    public int ProgressPercentage { get; set; } = 0;

    public int TotalRecords { get; set; }

    public int SuccessfulRecords { get; set; }

    public int FailedRecords { get; set; }

    public string? ErrorMessage { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public DateTime? StartedAtUtc { get; set; }

    public DateTime? CompletedAtUtc { get; set; }
}
