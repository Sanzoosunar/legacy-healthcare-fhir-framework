using System;
using System.Collections.Generic;
using System.Text;

namespace LegacyHealthcareFHIR.Core.Models;

public class MappingProfile
{
    public int Id { get; set; }

    public int HospitalId { get; set; }

    public required string Name { get; set; }

    public required string ResourceType { get; set; }

    public string Status { get; set; } = "Draft";

    public int Version { get; set; } = 1;

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public DateTime? ApprovedAtUtc { get; set; }
}
