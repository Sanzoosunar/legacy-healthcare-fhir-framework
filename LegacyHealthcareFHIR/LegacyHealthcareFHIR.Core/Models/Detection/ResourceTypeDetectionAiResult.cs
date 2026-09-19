using LegacyHealthcareFHIR.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace LegacyHealthcareFHIR.Core.Models.Detection
{
    public class ResourceTypeDetectionAiResult
    {
        public FhirResourceType ResourceType { get; set; }
        public decimal AiConfidence { get; set; }
    }
}
