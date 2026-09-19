using LegacyHealthcareFHIR.Core.Models;
using LegacyHealthcareFHIR.Core.Models.Common;
using LegacyHealthcareFHIR.Core.Models.Legacy;
using LegacyHealthcareFHIR.Core.Models.Mapping;
using LegacyHealthcareFHIR.Core.Models.Normalized;

namespace LegacyHealthcareFHIR.Core.Interfaces;

public interface IPatientMappingService
{
    ProcessResult<List<PatientData>, MappingError> Map(
        List<LegacyRecord> records,
        List<FieldMapping> fieldMappings);
}