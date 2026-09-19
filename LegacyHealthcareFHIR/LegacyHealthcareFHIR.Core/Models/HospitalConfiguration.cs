using System;
using System.Collections.Generic;
using System.Text;

namespace LegacyHealthcareFHIR.Core.Models;

public class HospitalConfiguration
{
    public int Id { get; set; }

    public int HospitalId { get; set; }

    public required string SourceSystemName { get; set; }

    public required string InputFormat { get; set; }

    public bool EnableAiValidation { get; set; } = true;

    public bool EnableAiMapping { get; set; } = true;

    public bool EnableNotifications { get; set; } = false;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAtUtc { get; set; }
}
