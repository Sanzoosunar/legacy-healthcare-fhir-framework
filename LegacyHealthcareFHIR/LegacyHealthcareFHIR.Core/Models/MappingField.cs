using System;
using System.Collections.Generic;
using System.Text;

namespace LegacyHealthcareFHIR.Core.Models;

public class MappingField
{
    public int Id { get; set; }

    public int MappingProfileId { get; set; }

    public required string SourceField { get; set; }

    public required string TargetField { get; set; }

    public decimal? AiConfidence { get; set; }

    public string? AiExplanation { get; set; }

    public bool IsApproved { get; set; } = false;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public DateTime? ApprovedAtUtc { get; set; }
}
