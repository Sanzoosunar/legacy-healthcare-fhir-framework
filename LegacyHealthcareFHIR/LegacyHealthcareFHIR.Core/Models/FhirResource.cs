using System;
using System.Collections.Generic;
using System.Text;

namespace LegacyHealthcareFHIR.Core.Models;

public class FhirResource
{
    public int Id { get; set; }

    public int HospitalId { get; set; }

    public Guid ImportJobId { get; set; }

    public required string ResourceType { get; set; }

    public required string ResourceId { get; set; }

    public required string FhirJson { get; set; }

    public bool IsValid { get; set; }

    public string? ValidationMessage { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}