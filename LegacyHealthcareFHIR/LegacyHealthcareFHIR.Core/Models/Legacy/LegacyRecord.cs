using System;
using System.Collections.Generic;
using System.Text;
namespace LegacyHealthcareFHIR.Core.Models.Legacy;

public class LegacyRecord
{
    public Dictionary<string, string?> Fields { get; set; } = new();
}