using LegacyHealthcareFHIR.Core.Enums;
using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using System.Text;

namespace LegacyHealthcareFHIR.Core.Models;

public class MappingConfiguration
{
    public int Id { get; set; }
    public int HospitalId { get; set; }
    public ResourceType ResourceType { get; set; }
    public MappingStatus Status { get; set; } = MappingStatus.Draft;
}
